"""
Expense Report Chatbot - FastAPI Application
Integrates with Azure AI Foundry Agent Service
"""

import os
from fastapi import FastAPI, HTTPException
from fastapi.responses import FileResponse
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
from azure.ai.projects import AIProjectClient
from azure.identity import DefaultAzureCredential
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Initialize FastAPI app
app = FastAPI(title="Expense Report Chatbot")

# Mount static files directory
app.mount("/static", StaticFiles(directory="static"), name="static")

# Configuration from environment variables
PROJECT_ENDPOINT = os.getenv("PROJECT_ENDPOINT")
AGENT_ID = os.getenv("AGENT_ID")

if not PROJECT_ENDPOINT or not AGENT_ID:
    raise ValueError("PROJECT_ENDPOINT and AGENT_ID must be set in environment variables")

# Initialize Azure AI Project Client
project_client = AIProjectClient(
    endpoint=PROJECT_ENDPOINT,
    credential=DefaultAzureCredential()
)

# Global thread ID (simplified for demo - in production, use session management)
global_thread_id = None


class ChatRequest(BaseModel):
    """Request model for chat messages"""
    message: str


class ChatResponse(BaseModel):
    """Response model for chat messages"""
    response: str
    thread_id: str


def initialize_thread():
    """Initialize a conversation thread with the agent"""
    global global_thread_id
    if global_thread_id is None:
        thread = project_client.agents.threads.create()
        global_thread_id = thread.id
        print(f"Created new thread: {global_thread_id}")
    return global_thread_id


@app.get("/")
async def get_homepage():
    """Serve the chatbot HTML interface"""
    return FileResponse("static/index.html")


@app.post("/api/chat", response_model=ChatResponse)
async def chat(request: ChatRequest):
    """
    Handle chat messages with the Azure AI Foundry agent

    Args:
        request: ChatRequest containing the user's message

    Returns:
        ChatResponse with the agent's response and thread ID
    """
    try:
        # Ensure thread exists
        thread_id = initialize_thread()

        # Create message in the thread
        message = project_client.agents.messages.create(
            thread_id=thread_id,
            role="user",
            content=request.message
        )
        print(f"Created message: {message['id']}")

        # Run the agent on the thread
        run = project_client.agents.runs.create_and_process(
            thread_id=thread_id,
            agent_id=AGENT_ID
        )
        print(f"Run completed with status: {run.status}")

        # Check if run failed
        if run.status == "failed":
            print(f"Run failed: {run.last_error}")
            raise HTTPException(
                status_code=500,
                detail=f"Agent run failed: {run.last_error}"
            )

        # Retrieve messages from the thread
        messages = project_client.agents.messages.list(thread_id=thread_id)

        # Get the last assistant message
        # messages.list() returns an ItemPaged iterator, not a list with .data
        assistant_response = None
        for msg in messages:
            if msg.role == "assistant":
                # Extract text content from the message
                if msg.content and len(msg.content) > 0:
                    for content_item in msg.content:
                        if hasattr(content_item, 'text') and hasattr(content_item.text, 'value'):
                            assistant_response = content_item.text.value
                            break
                if assistant_response:
                    break

        if not assistant_response:
            assistant_response = "I received your message but couldn't generate a response. Please try again."

        return ChatResponse(
            response=assistant_response,
            thread_id=thread_id
        )

    except Exception as e:
        print(f"Error in chat endpoint: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"An error occurred: {str(e)}"
        )


@app.get("/api/health")
async def health_check():
    """Health check endpoint"""
    return {
        "status": "healthy",
        "agent_id": AGENT_ID,
        "endpoint": PROJECT_ENDPOINT
    }


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
