# Expense Report Chatbot

A simple web application chatbot that integrates with Azure AI Foundry Agent Service. This chatbot helps users with expense policies and expense report submissions.

## Features

- Simple HTTP-based chat interface
- Integration with Azure AI Foundry Agent Service
- Clean and modern UI with responsive design
- Typing indicators during processing
- Persistent conversation threads across requests

## Prerequisites

- Python 3.8 or higher
- Azure AI Foundry project with an agent deployed
- Azure credentials configured (Azure CLI, environment variables, or managed identity)

## Installation

1. **Clone or download this repository**

2. **Install Python dependencies:**

```bash
pip install -r requirements.txt
```

3. **Configure environment variables:**

Copy `.env.example` to `.env` and fill in your Azure details:

```bash
cp .env.example .env
```

Edit `.env` with your configuration:

```env
AZURE_PROJECT_ENDPOINT=https://your-project.api.azureml.ms
AZURE_AGENT_ID=asst_LtGz0IcvsISO19v0a4jWia9Y
```

### Authentication

The application uses `DefaultAzureCredential` from Azure Identity, which supports multiple authentication methods in the following order:

1. **Environment variables** - Set these if using service principal:
   - `AZURE_TENANT_ID`
   - `AZURE_CLIENT_ID`
   - `AZURE_CLIENT_SECRET`

2. **Managed Identity** - Automatically used when running in Azure (App Service, Container Apps, etc.)

3. **Azure CLI** - Run `az login` to authenticate locally

4. **Visual Studio Code** - Sign in through VS Code's Azure extension

For local development, the easiest method is to use Azure CLI:

```bash
az login
```

## Running the Application

Start the FastAPI server:

```bash
python main.py
```

Or use uvicorn directly:

```bash
uvicorn main:app --host 0.0.0.0 --port 8000 --reload
```

The application will be available at:
- Web interface: http://localhost:8000
- Health check: http://localhost:8000/health
- Chat API endpoint: http://localhost:8000/api/chat

## Project Structure

```
.
├── main.py              # FastAPI application with HTTP API and Azure AI integration
├── requirements.txt     # Python dependencies
├── .env.example        # Environment variables template
├── .env                # Your environment variables (not committed)
├── README.md           # This file
└── static/
    ├── index.html      # Chat interface HTML
    ├── styles.css      # Styling
    └── app.js          # HTTP client logic with fetch API
```

## How It Works

1. **Page load** - The frontend loads and is ready to accept user input
2. **First message** - A new Azure AI agent thread is created for the conversation
3. **Message flow**:
   - User sends a message via HTTP POST to `/api/chat`
   - Backend creates a message in the Azure AI thread
   - Backend creates and runs the agent
   - Backend polls for completion and retrieves the response
   - Response is returned in the HTTP response along with the thread ID
4. **Subsequent messages** - The frontend includes the thread ID to continue the conversation
5. **Session persistence** - Thread ID is stored in the browser session for conversation continuity

## API Endpoints

### `GET /`
Serves the main chat interface

### `GET /health`
Health check endpoint that returns:
```json
{
    "status": "healthy",
    "agent_id": "asst_LtGz0IcvsISO19v0a4jWia9Y"
}
```

### `POST /api/chat`
HTTP endpoint for chat messages

**Request body:**
```json
{
    "message": "What are the expense policies?",
    "thread_id": "thread_abc123" // Optional, omit for first message
}
```

**Response:**
```json
{
    "response": "Agent response text",
    "thread_id": "thread_abc123"
}
```

## Configuration Options

### Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `AZURE_PROJECT_ENDPOINT` | Yes | Your Azure AI Foundry project endpoint |
| `AZURE_AGENT_ID` | Yes | The ID of your deployed agent |
| `AZURE_TENANT_ID` | No | Azure AD tenant ID (for service principal auth) |
| `AZURE_CLIENT_ID` | No | Azure AD client ID (for service principal auth) |
| `AZURE_CLIENT_SECRET` | No | Azure AD client secret (for service principal auth) |

## Deployment

### Azure App Service

1. Create an App Service with Python runtime
2. Configure environment variables in App Service settings
3. Enable Managed Identity for the App Service
4. Grant the Managed Identity access to your Azure AI Foundry project
5. Deploy using Git, ZIP, or container

### Azure Container Apps

1. Build Docker image:
```bash
docker build -t expense-chatbot .
```

2. Push to Azure Container Registry
3. Deploy to Container Apps with environment variables
4. Enable Managed Identity and grant appropriate permissions

### Local Development with Docker

Create a `Dockerfile`:
```dockerfile
FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

EXPOSE 8000

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
```

Build and run:
```bash
docker build -t expense-chatbot .
docker run -p 8000:8000 --env-file .env expense-chatbot
```

## Troubleshooting

### Connection Issues

- Verify your `AZURE_PROJECT_ENDPOINT` is correct
- Ensure you're authenticated (run `az login`)
- Check that your Azure credentials have access to the AI Foundry project

### Agent Not Responding

- Verify `AZURE_AGENT_ID` matches your deployed agent
- Check the agent is properly configured in Azure AI Foundry portal
- Review FastAPI logs for detailed error messages

### Slow Response Times

- The application polls for agent completion every 1 second
- Response times depend on the agent's processing time
- Consider implementing streaming responses for better UX in production

## Future Enhancements

- Persist conversation threads across sessions
- Add user authentication
- Implement streaming responses for faster feedback
- Add file upload support for expense receipts
- Support multiple concurrent users with session management
- Add conversation history storage

## License

MIT License

## Support

For issues related to:
- **This application**: Check the logs in the FastAPI console
- **Azure AI Foundry**: Visit [Azure AI documentation](https://learn.microsoft.com/azure/ai-foundry/)
- **Authentication**: Review [Azure Identity documentation](https://learn.microsoft.com/python/api/overview/azure/identity-readme)
