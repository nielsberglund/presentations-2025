import os
from typing import List, Dict
from dotenv import load_dotenv
from fastapi import FastAPI, Request, Form, HTTPException
from fastapi.responses import HTMLResponse
from fastapi.templating import Jinja2Templates
from azure.ai.projects import AIProjectClient
from azure.identity import DefaultAzureCredential
from azure.core.exceptions import AzureError

# Load environment variables
load_dotenv()

# Initialize FastAPI app
app = FastAPI(title="Expense Report Chatbot")

# Configure templates
templates = Jinja2Templates(directory="templates")

# Azure AI configuration
AZURE_AI_PROJECT_ENDPOINT = os.getenv("AZURE_AI_PROJECT_ENDPOINT")
AZURE_AI_AGENT_ID = os.getenv("AZURE_AI_AGENT_ID")

if not AZURE_AI_PROJECT_ENDPOINT or not AZURE_AI_AGENT_ID:
    raise ValueError("AZURE_AI_PROJECT_ENDPOINT and AZURE_AI_AGENT_ID must be set in .env file")

# Initialize Azure AI Project Client
project_client = AIProjectClient(
    endpoint=AZURE_AI_PROJECT_ENDPOINT,
    credential=DefaultAzureCredential()
)

# Store conversation threads in memory (in production, use a database)
conversation_threads: Dict[str, str] = {}


@app.get("/", response_class=HTMLResponse)
async def home(request: Request):
    """Render the chat interface."""
    return templates.TemplateResponse(
        request=request,
        name="chat.html",
        context={"messages": []}
    )


@app.post("/chat", response_class=HTMLResponse)
async def chat(request: Request, message: str = Form(...), session_id: str = Form(default="default")):
    """
    Handle chat messages and interact with Azure AI agent.

    Args:
        request: FastAPI request object
        message: User message from form
        session_id: Session identifier for maintaining conversation context

    Returns:
        HTML response with chat history
    """
    try:
        # Get or create thread for this session
        if session_id not in conversation_threads:
            thread = project_client.agents.threads.create()
            conversation_threads[session_id] = thread.id

        thread_id = conversation_threads[session_id]

        # Create user message in the thread
        project_client.agents.messages.create(
            thread_id=thread_id,
            role="user",
            content=message
        )

        # Run the agent to process the message
        run = project_client.agents.runs.create_and_process(
            thread_id=thread_id,
            agent_id=AZURE_AI_AGENT_ID
        )

        # Get all messages from the thread
        messages_response = project_client.agents.messages.list(thread_id=thread_id)

        # Convert messages to a list and reverse to get chronological order
        messages_list = list(messages_response)
        messages_list.reverse()

        # Format messages for display
        formatted_messages = []
        for msg in messages_list:
            # Extract text content from message
            content_text = ""
            if hasattr(msg, 'content') and msg.content:
                for content_item in msg.content:
                    if hasattr(content_item, 'text') and hasattr(content_item.text, 'value'):
                        content_text += content_item.text.value

            formatted_messages.append({
                "role": msg.role,
                "content": content_text
            })

        return templates.TemplateResponse(
            request=request,
            name="chat.html",
            context={
                "messages": formatted_messages,
                "session_id": session_id
            }
        )

    except AzureError as e:
        raise HTTPException(status_code=500, detail=f"Azure AI Error: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error: {str(e)}")


@app.get("/health")
async def health_check():
    """Health check endpoint."""
    return {"status": "healthy", "agent_configured": bool(AZURE_AI_AGENT_ID)}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
