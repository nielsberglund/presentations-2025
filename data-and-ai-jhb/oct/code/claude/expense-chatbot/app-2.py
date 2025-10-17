import os
import asyncio
from typing import Dict, Optional
from fastapi import FastAPI, HTTPException
from fastapi.staticfiles import StaticFiles
from fastapi.responses import FileResponse, JSONResponse
from pydantic import BaseModel
from azure.ai.projects import AIProjectClient
from azure.identity import DefaultAzureCredential
from azure.core.credentials import AccessToken, TokenCredential
from dotenv import load_dotenv
import uuid

# Load environment variables
load_dotenv()

app = FastAPI(title="Expense Report Chatbot")

# Azure AI Foundry configuration
AZURE_SUBSCRIPTION_ID = os.getenv("AZURE_SUBSCRIPTION_ID")
AZURE_RESOURCE_GROUP_NAME = os.getenv("AZURE_RESOURCE_GROUP_NAME")
AZURE_PROJECT_NAME = os.getenv("AZURE_PROJECT_NAME")
AGENT_ID = os.getenv("AGENT_ID", "asst_LtGz0IcvsISO19v0a4jWia9Y")
AZURE_PROJECT_ENDPOINT = os.getenv("AZURE_PROJECT_ENDPOINT")

# Initialize Azure AI Project Client with proper credential scope
try:
    project_client = AIProjectClient(
        credential=DefaultAzureCredential(),
        endpoint="https://nielsb-test-1-resource.services.ai.azure.com/api/projects/nielsb-test-1")

    print("Azure AI Project Client initialized successfully")

    agent = project_client.agents.get_agent("asst_LtGz0IcvsISO19v0a4jWia9Y")

except Exception as e:
    print(f"Error initializing Azure AI Project Client: {e}")
    project_client = None


# Store sessions and threads in memory
sessions: Dict[str, str] = {}  # session_id -> thread_id


class ChatRequest(BaseModel):
    message: str
    session_id: Optional[str] = None


class ChatResponse(BaseModel):
    message: str
    role: str
    session_id: str


@app.post("/api/chat", response_model=ChatResponse)
async def chat(request: ChatRequest):
    """Handle chat messages via HTTP POST"""

    if not project_client:
        raise HTTPException(
            status_code=500,
            detail="Azure AI Project Client not initialized. Please check your configuration."
        )

    # Get or create session
    session_id = request.session_id
    if not session_id:
        session_id = str(uuid.uuid4())

    # Get or create thread for this session
    thread_id = sessions.get(session_id)
    if not thread_id:
        try:
            print(f"Creating new thread for session {session_id}")
            thread = project_client.agents.threads.create()
            thread_id = thread.id
            sessions[session_id] = thread_id
            print(f"Created thread: {thread_id}")
        except Exception as e:
            print(f"Error creating thread: {e}")
            raise HTTPException(status_code=500, detail=f"Error creating thread: {str(e)}")

    try:
        # Create message in thread
        print(f"Creating message in thread {thread_id}")
        message = project_client.agents.messages.create(
            thread_id=thread_id,
            role="user",
            content=request.message
        )
        print(f"Created message in thread {thread_id}")

        # Create and run the agent
        print(f"Starting agent run for thread {thread_id}")
        run = project_client.agents.runs.create_and_process(
            thread_id=thread_id,
            agent_id=agent.id
        )
        print(f"Run created with status: {run.status}")

        # Wait for completion
        max_iterations = 60  # 60 seconds max
        iteration = 0
        while run.status in ["queued", "in_progress", "requires_action"] and iteration < max_iterations:
            await asyncio.sleep(1)
            run = project_client.agents.runs.get(thread_id=thread_id, run_id=run.id)
            print(f"Run status: {run.status}")
            iteration += 1

            #Handle tool calls if needed
            if run.status == "requires_action":
                print("Run requires action - handling tool calls")
            #     tool_outputs = []
            #     for tool_call in run.required_action.items():
            #         if tool_call
            #         # run.required_action.submit_tool_outputs.tool_calls
            #         print(f"Tool call: {tool_call.}")
            #         # Add your tool execution logic here
            #         tool_outputs.append({
            #             "tool_call_id": tool_call.id,
            #             "output": "{}"
            #         })

            #     if tool_outputs:
            #         run = project_client.agents.submit_tool_outputs_to_run(
            #             thread_id=thread_id,
            #             run_id=run.id,
            #             tool_outputs=tool_outputs
            #         )

        if run.status == "completed":
            # Get the latest messages
            messages = project_client.agents.messages.list(thread_id=thread_id)

            # Find the assistant's response
            assistant_messages = [msg for msg in messages if msg.role == "assistant"]

            if assistant_messages:
                latest_message = assistant_messages[0]

                # Extract text content
                response_text = ""
                for content in latest_message.content:
                    if hasattr(content, 'text'):
                        response_text += content.text.value

                return ChatResponse(
                    message=response_text,
                    role="assistant",
                    session_id=session_id
                )
            else:
                raise HTTPException(status_code=500, detail="No response from agent")
        else:
            raise HTTPException(status_code=500, detail=f"Run failed with status: {run.status}")

    except Exception as e:
        print(f"Error processing agent response: {e}")
        raise HTTPException(status_code=500, detail=f"Error: {str(e)}")


@app.post("/api/session/new")
async def new_session():
    """Create a new chat session"""
    session_id = str(uuid.uuid4())
    return JSONResponse(content={"session_id": session_id})


@app.delete("/api/session/{session_id}")
async def delete_session(session_id: str):
    """Delete a chat session"""
    if session_id in sessions:
        del sessions[session_id]
    return JSONResponse(content={"status": "deleted"})


@app.get("/")
async def get_index():
    """Serve the main HTML page"""
    return FileResponse("static/index.html")


@app.get("/health")
async def health_check():
    """Health check endpoint"""
    return {
        "status": "healthy",
        "azure_client_initialized": project_client is not None,
        "active_sessions": len(sessions)
    }


# Mount static files
app.mount("/static", StaticFiles(directory="static"), name="static")


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
