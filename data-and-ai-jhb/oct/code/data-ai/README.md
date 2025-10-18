# Expense Report Chatbot

A simple web-based chatbot application that interfaces with Azure AI Foundry Agent Service to help users with expense policies and expense report submissions.

## Features

- Clean, modern chat interface
- Integration with Azure AI Foundry Agent Service
- Session-based conversation management
- HTML-based responses (no WebSockets required)
- Expense policy inquiries
- Expense report submission assistance

## Prerequisites

- Python 3.8 or higher
- Azure AI Foundry project with a configured agent
- Azure credentials configured (Azure CLI, Service Principal, or Managed Identity)

## Installation

1. **Clone or navigate to the project directory**

2. **Install dependencies**

```bash
pip install -r requirements.txt
```

3. **Configure environment variables**

Copy the `.env.example` file to `.env`:

```bash
cp .env.example .env
```

Edit `.env` and add your Azure AI configuration:

```env
AZURE_AI_PROJECT_ENDPOINT=https://your-project-endpoint.cognitiveservices.azure.com/
AZURE_AI_AGENT_ID=your-agent-id-here
```

To find these values:
- **AZURE_AI_PROJECT_ENDPOINT**: Available in your Azure AI Foundry project settings
- **AZURE_AI_AGENT_ID**: The ID of your deployed agent in Azure AI Foundry

4. **Set up Azure authentication**

This application uses `DefaultAzureCredential` which supports multiple authentication methods:

- **Azure CLI**: Run `az login` before starting the application
- **Environment Variables**: Set `AZURE_TENANT_ID`, `AZURE_CLIENT_ID`, and `AZURE_CLIENT_SECRET`
- **Managed Identity**: Automatically works when deployed to Azure services

For local development, the easiest method is Azure CLI:

```bash
az login
```

## Running the Application

### Development Mode

```bash
python app.py
```

Or using uvicorn directly:

```bash
uvicorn app:app --reload --host 0.0.0.0 --port 8000
```

### Production Mode

```bash
uvicorn app:app --host 0.0.0.0 --port 8000 --workers 4
```

## Usage

1. Open your browser and navigate to `http://localhost:8000`
2. Type your question or request in the chat input
3. The agent will respond based on the configured expense policies and capabilities

### Example Questions

- "What expenses are eligible for reimbursement?"
- "How do I submit an expense report?"
- "What documentation do I need for travel expenses?"
- "Can I claim meal expenses?"

## Project Structure

```
.
├── app.py                  # Main FastAPI application
├── requirements.txt        # Python dependencies
├── .env.example           # Environment variable template
├── .env                   # Your local configuration (not in git)
├── templates/
│   └── chat.html          # Chat interface template
└── README.md              # This file
```

## API Endpoints

### `GET /`
Renders the chat interface

### `POST /chat`
Handles chat message submission

**Form Parameters:**
- `message` (required): User's message
- `session_id` (optional): Session identifier for conversation continuity

**Returns:** HTML response with chat history

### `GET /health`
Health check endpoint

**Returns:** JSON with application status

## Technology Stack

- **FastAPI 0.115.5**: Modern Python web framework
- **Azure AI Projects SDK 1.0.0b10**: Azure AI Foundry integration
- **Jinja2 3.1.4**: Template engine for HTML rendering
- **Azure Identity 1.19.0**: Azure authentication
- **Uvicorn 0.34.0**: ASGI server

## Architecture

The application follows this flow:

1. User submits a message via the HTML form
2. FastAPI receives the POST request
3. A thread is created (or retrieved) for the session
4. The user's message is added to the thread
5. The Azure AI agent processes the message using `create_and_process()`
6. All messages are retrieved from the thread
7. The updated chat history is rendered and returned as HTML

## Configuration

### Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `AZURE_AI_PROJECT_ENDPOINT` | Yes | Your Azure AI Foundry project endpoint URL |
| `AZURE_AI_AGENT_ID` | Yes | The ID of your deployed agent |

### Azure AI Agent Setup

Your Azure AI Foundry agent should be configured with:
- Knowledge about expense policies (via file upload or instructions)
- Instructions for handling expense report submissions
- Any relevant tools or integrations

## Error Handling

The application includes error handling for:
- Missing environment variables (raises `ValueError` on startup)
- Azure service errors (returns HTTP 500 with error details)
- General exceptions (returns HTTP 500 with error message)

## Session Management

Sessions are stored in-memory using a simple dictionary. For production use, consider:
- Redis for distributed session storage
- Database for persistent conversation history
- Session expiration and cleanup mechanisms

## Security Considerations

- Never commit your `.env` file to version control
- Use Azure Managed Identity when deploying to Azure
- Implement rate limiting for production deployments
- Add authentication/authorization for user access
- Sanitize user inputs if storing in a database

## Deployment

### Azure App Service

1. Create an Azure App Service (Python runtime)
2. Enable Managed Identity
3. Grant the Managed Identity access to your Azure AI Foundry project
4. Set environment variables in App Service configuration
5. Deploy using Git, Azure CLI, or VS Code

### Docker

Create a `Dockerfile`:

```dockerfile
FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

EXPOSE 8000

CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8000"]
```

Build and run:

```bash
docker build -t expense-chatbot .
docker run -p 8000:8000 --env-file .env expense-chatbot
```

## Troubleshooting

### "DefaultAzureCredential failed to retrieve a token"

- Ensure you're logged in via `az login`
- Verify your Azure credentials have access to the AI Foundry project
- Check that environment variables are set correctly

### "AZURE_AI_PROJECT_ENDPOINT and AZURE_AI_AGENT_ID must be set"

- Verify your `.env` file exists and contains the required variables
- Ensure the `.env` file is in the same directory as `app.py`

### Agent not responding

- Verify the agent ID is correct
- Check that the agent is deployed and running in Azure AI Foundry
- Review Azure AI Foundry logs for errors

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
