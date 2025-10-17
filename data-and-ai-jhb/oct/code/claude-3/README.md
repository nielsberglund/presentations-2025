# Expense Report Chatbot

A simple web application that integrates with Azure AI Foundry Agent Service to provide an interactive chatbot for expense reporting and policy inquiries.

## Features

- Clean, modern web interface for chatting with the AI agent
- Integration with Azure AI Foundry Agent Service
- Real-time responses from the expense report agent (Agent-test-1)
- Handles questions about expense policies
- Assists with submitting expense reports
- Built with FastAPI and Python

## Prerequisites

- Python 3.9 or higher
- Azure subscription with access to Azure AI Foundry
- Azure CLI installed and configured
- An existing Azure AI Foundry agent (Agent-test-1)

## Installation

1. Clone or download this repository

2. Create a virtual environment:
```bash
python -m venv venv
```

3. Activate the virtual environment:
   - Windows:
     ```bash
     venv\Scripts\activate
     ```
   - Linux/Mac:
     ```bash
     source venv/bin/activate
     ```

4. Install dependencies:
```bash
pip install -r requirements.txt
```

## Configuration

1. Copy the example environment file:
```bash
cp .env.example .env
```

2. Edit `.env` and add your Azure AI Foundry configuration:
```env
PROJECT_ENDPOINT=https://your-account.services.ai.azure.com/api/projects/your-project
AGENT_ID=asst_LtGz0IcvsISO19v0a4jWia9Y
```

### Getting Your Configuration Values

**PROJECT_ENDPOINT:**
- Go to your Azure AI Foundry portal
- Navigate to your project
- The endpoint format is: `https://<your-ai-services-account>.services.ai.azure.com/api/projects/<your-project-name>`

**AGENT_ID:**
- Your agent ID has been provided: `asst_LtGz0IcvsISO19v0a4jWia9Y`

## Authentication

This application uses **Azure DefaultAzureCredential** for authentication, which supports multiple authentication methods in the following order:

1. Environment variables
2. Managed Identity
3. Azure CLI credentials
4. Azure PowerShell
5. Interactive browser

### Setup Azure CLI Authentication (Recommended for local development)

```bash
az login
```

Make sure your Azure account has the appropriate role assignments for the Azure AI Foundry project.

## Running the Application

1. Ensure your virtual environment is activated

2. Start the FastAPI server:
```bash
python app.py
```

Or use uvicorn directly:
```bash
uvicorn app:app --reload --host 0.0.0.0 --port 8000
```

3. Open your browser and navigate to:
```
http://localhost:8000
```

## Usage

1. The chatbot interface will load with a welcome message
2. Type your question or request in the input box at the bottom
3. Press "Send" or hit Enter to submit your message
4. The agent will process your request and respond with relevant information

### Example Questions

- "What is the company's expense policy for travel?"
- "How do I submit an expense report?"
- "What are the reimbursement limits for meals?"
- "Can I expense parking fees?"
- "I need to submit a report for my recent business trip"

## API Endpoints

- `GET /` - Serves the chatbot HTML interface
- `POST /api/chat` - Sends a message to the agent and receives a response
  - Request body: `{"message": "your message here"}`
  - Response: `{"response": "agent response", "thread_id": "thread-id"}`
- `GET /api/health` - Health check endpoint

## Architecture

The application consists of:

1. **FastAPI Backend** (`app.py`):
   - Serves the HTML interface
   - Handles API requests to `/api/chat`
   - Manages communication with Azure AI Foundry Agent Service
   - Maintains a conversation thread with the agent

2. **HTML/CSS/JavaScript Frontend** (embedded in `app.py`):
   - Modern, responsive chat interface
   - Real-time message display
   - Loading indicators
   - Smooth animations

3. **Azure AI Foundry Integration**:
   - Uses `azure-ai-projects` SDK (version 1.0.0)
   - Authenticates with `DefaultAzureCredential`
   - Creates and manages conversation threads
   - Sends messages and retrieves agent responses

## Dependencies

- **fastapi** (0.115.13) - Modern web framework for building APIs
- **uvicorn** (0.34.0) - ASGI server for running FastAPI
- **python-dotenv** (1.0.1) - Environment variable management
- **azure-ai-projects** (1.0.0) - Azure AI Foundry SDK
- **azure-identity** (1.25.1) - Azure authentication library
- **pydantic** (2.10.6) - Data validation using Python type hints

## Troubleshooting

### Authentication Errors

If you encounter authentication errors:
1. Ensure you're logged in with Azure CLI: `az login`
2. Verify your account has access to the Azure AI Foundry project
3. Check that your PROJECT_ENDPOINT is correct

### Agent Not Responding

If the agent doesn't respond:
1. Verify the AGENT_ID is correct
2. Check that the agent exists in your Azure AI Foundry project
3. Review the console output for error messages

### Connection Errors

If you can't connect to Azure:
1. Verify your internet connection
2. Check that the PROJECT_ENDPOINT is accessible
3. Ensure no firewall is blocking the connection

## Production Considerations

This is a simplified demo application. For production use, consider:

1. **Session Management**: Implement proper session management to maintain separate threads per user
2. **Error Handling**: Add more robust error handling and retry logic
3. **Logging**: Implement structured logging for monitoring
4. **Security**: Add authentication and authorization for the web interface
5. **Rate Limiting**: Implement rate limiting to prevent abuse
6. **Caching**: Cache responses where appropriate
7. **Monitoring**: Add application insights and monitoring
8. **Secrets Management**: Use Azure Key Vault for storing sensitive configuration

## License

This project is provided as a sample application for demonstration purposes.

## Support

For issues related to:
- **Azure AI Foundry**: Consult the [Azure AI Foundry documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- **FastAPI**: See the [FastAPI documentation](https://fastapi.tiangolo.com/)
- **Azure SDK**: Check the [Azure SDK for Python documentation](https://learn.microsoft.com/en-us/python/api/overview/azure/)
