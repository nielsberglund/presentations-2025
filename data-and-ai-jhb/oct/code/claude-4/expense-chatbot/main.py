import os
from typing import Optional
from fastapi import FastAPI, Request, Form
from fastapi.responses import HTMLResponse
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from azure.ai.projects import AIProjectClient
from azure.identity import DefaultAzureCredential
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Initialize FastAPI app
app = FastAPI(title="Expense Report Chatbot")

# Mount static files and templates
app.mount("/static", StaticFiles(directory="static"), name="static")
templates = Jinja2Templates(directory="templates")

# Azure AI configuration from environment variables
AGENT_ID = os.getenv("AGENT_ID")
PROJECT_ENDPOINT = os.getenv("PROJECT_ENDPOINT")

# Validate required environment variables
if not AGENT_ID or not PROJECT_ENDPOINT:
    raise ValueError("AGENT_ID and PROJECT_ENDPOINT environment variables are required")

# Initialize Azure AI Project Client
# project_client = AIProjectClient(
#     credential=DefaultAzureCredential(),
#     endpoint=PROJECT_ENDPOINT,
# )
try:
    project_client = AIProjectClient(
        credential=DefaultAzureCredential(),
        endpoint=PROJECT_ENDPOINT)

    print("Azure AI Project Client initialized successfully")

    # agent = project_client.agents.get_agent("asst_LtGz0IcvsISO19v0a4jWia9Y")

except Exception as e:
    print(f"Error initializing Azure AI Project Client: {e}")
    project_client = None

@app.get("/", response_class=HTMLResponse)
async def home(request: Request):
    """Render the chat interface."""
    return templates.TemplateResponse(
        "chat.html",
        {
            "request": request,
            "conversation": [],
            "error": None,
        }
    )


@app.post("/chat", response_class=HTMLResponse)
async def chat(request: Request, message: str = Form(...)):
    """
    Handle chat messages and interact with Azure AI Foundry agent.
    Creates a new thread for each message (stateless mode).
    """
    conversation = []
    error_message = None

    try:
        # Add user message to conversation history
        conversation.append({"role": "user", "content": message})

        # Create a new thread (stateless - new thread per request)
        thread = project_client.agents.threads.create()

        # Create message in the thread
        project_client.agents.messages.create(
            thread_id=thread.id,
            role="user",
            content=message
        )

        # Run the agent and wait for completion
        run = project_client.agents.runs.create_and_process(
            thread_id=thread.id,
            assistant_id=AGENT_ID
        )

        # Check if run completed successfully
        if run.status == "completed":
            # Retrieve messages from the thread
            messages = project_client.agents.messages.list(thread_id=thread.id)

            # Extract the assistant's response (most recent message)
            for msg in messages.data:
                if msg.role == "assistant":
                    # Get the text content from the message
                    if msg.content:
                        for content_item in msg.content:
                            if hasattr(content_item, 'text') and hasattr(content_item.text, 'value'):
                                assistant_response = content_item.text.value
                                conversation.append({"role": "assistant", "content": assistant_response})
                                break
                    break
        else:
            error_message = f"Agent run did not complete successfully. Status: {run.status}"

    except Exception as e:
        error_message = f"Error communicating with agent: {str(e)}"

    return templates.TemplateResponse(
        "chat.html",
        {
            "request": request,
            "conversation": conversation,
            "error": error_message,
        }
    )


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
