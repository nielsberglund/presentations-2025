import os
import asyncio
import logging
from typing import Dict, Optional
from fastapi import FastAPI, HTTPException
from fastapi.staticfiles import StaticFiles
from fastapi.responses import HTMLResponse, JSONResponse
from pydantic import BaseModel
from azure.ai.projects import AIProjectClient
from azure.identity import DefaultAzureCredential
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Initialize FastAPI app
app = FastAPI(title="Expense Report Chatbot")

# Mount static files
app.mount("/static", StaticFiles(directory="static"), name="static")

# Azure AI Foundry configuration
AZURE_PROJECT_ENDPOINT = os.getenv("AZURE_PROJECT_ENDPOINT")
AZURE_AGENT_ID = os.getenv("AZURE_AGENT_ID")

if not AZURE_PROJECT_ENDPOINT or not AZURE_AGENT_ID:
    raise ValueError("AZURE_PROJECT_ENDPOINT and AZURE_AGENT_ID must be set in environment variables")

# Initialize Azure AI Project Client
try:
    project_client = AIProjectClient(
        endpoint=AZURE_PROJECT_ENDPOINT,
        credential=DefaultAzureCredential()
    )
    logger.info("Azure AI Project Client initialized successfully")
except Exception as e:
    logger.error(f"Failed to initialize Azure AI Project Client: {e}")
    raise

# Store active threads for each session
active_threads: Dict[str, str] = {}

# Request/Response models
class ChatRequest(BaseModel):
    message: str
    thread_id: Optional[str] = None

class ChatResponse(BaseModel):
    response: str
    thread_id: str


@app.get("/")
async def get():
    """Serve the main page"""
    with open("static/index.html") as f:
        return HTMLResponse(content=f.read())


@app.post("/api/chat", response_model=ChatResponse)
async def chat(request: ChatRequest):
    """Handle chat messages via HTTP POST"""
    try:
        user_message = request.message.strip()

        if not user_message:
            raise HTTPException(status_code=400, detail="Message cannot be empty")

        # Get or create thread
        thread_id = request.thread_id
        if not thread_id:
            # Create a new thread for this conversation
            thread = project_client.agents.threads.create()
            thread_id = thread.id
            logger.info(f"Created new thread: {thread_id}")
        else:
            logger.info(f"Using existing thread: {thread_id}")

        logger.info(f"Received message: {user_message}")

        # Create a message in the thread
        project_client.agents.messages.create(
            thread_id=thread_id,
            role="user",
            content=user_message
        )

        # Create and run the agent
        run = project_client.agents.runs.create_and_process(
            thread_id=thread_id,
            agent_id=AZURE_AGENT_ID
        )

        # Poll for completion
        while run.status in ["queued", "in_progress", "requires_action"]:
            await asyncio.sleep(1)
            run = project_client.agents.runs.get(
                thread_id=thread_id,
                run_id=run.id
            )

        if run.status == "completed":
            # Retrieve messages
            messages = project_client.agents.messages.list(thread_id=thread_id)

            # Get the latest assistant message
            for message in messages:
                if message.role == "assistant":
                    # Extract text content
                    content_text = ""
                    for content_item in message.content:
                        if hasattr(content_item, 'text'):
                            content_text = content_item.text.value
                            break

                    if content_text:
                        return ChatResponse(
                            response=content_text,
                            thread_id=thread_id
                        )

            # If no assistant message found
            raise HTTPException(status_code=500, detail="No response from agent")
        else:
            error_msg = f"Run failed with status: {run.status}"
            if run.last_error:
                error_msg += f" - {run.last_error}"
            logger.error(error_msg)
            raise HTTPException(status_code=500, detail="Agent processing failed")

    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error processing message: {e}")
        raise HTTPException(status_code=500, detail=f"An error occurred: {str(e)}")


@app.get("/health")
async def health_check():
    """Health check endpoint"""
    return {"status": "healthy", "agent_id": AZURE_AGENT_ID}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
