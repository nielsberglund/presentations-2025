# Expense Report Chatbot

A simple web application that provides a chat interface to interact with an Azure AI Foundry agent for handling expense report inquiries and submissions.

## Features

- Clean, responsive chat interface
- Integration with Azure AI Foundry Agent Service
- Support for expense policy questions
- Expense report submission assistance
- Stateless operation (new thread per message)

## Prerequisites

- Python 3.9 or higher
- Azure subscription with AI Foundry access
- Azure credentials configured (one of the following):
  - Azure CLI (`az login`)
  - Service Principal credentials
  - Managed Identity (when deployed to Azure)

## Installation

1. **Clone or download this repository**

2. **Install Python dependencies**
   ```bash
   cd expense-chatbot
   pip install -r requirements.txt
   ```

3. **Configure environment variables**

   Copy the example environment file and update with your values:
   ```bash
   cp .env.example .env
   ```

   Edit `.env` and set:
   - `AGENT_ID`: Your Azure AI Foundry agent ID
   - `PROJECT_ENDPOINT`: Your Azure AI Foundry project endpoint
   - Azure authentication credentials (if not using Azure CLI)

## Configuration

### Environment Variables

#### Required
- `AGENT_ID`: The ID of your Azure AI Foundry agent (e.g., `asst_2I6lCCSnn6w4Mr62Aj8Mysqc`)
- `PROJECT_ENDPOINT`: Your project endpoint URL (e.g., `https://nielsb-test-1-resource.services.ai.azure.com/api/projects/nielsb-test-1`)

#### Azure Authentication (choose one method)

**Option 1: Azure CLI (Recommended for local development)**
```bash
az login
```
No additional environment variables needed.

**Option 2: Service Principal**
Set these in your `.env` file:
```
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
AZURE_CLIENT_SECRET=your-client-secret
```

**Option 3: Managed Identity**
Used automatically when deployed to Azure App Service or other Azure compute services.

## Running the Application

### Development Server

Start the FastAPI development server:

```bash
python main.py
```

Or using uvicorn directly:

```bash
uvicorn main:app --reload --host 0.0.0.0 --port 8000
```

The application will be available at: `http://localhost:8000`

### Production Deployment

For production, use a production ASGI server:

```bash
uvicorn main:app --host 0.0.0.0 --port 8000 --workers 4
```

## Usage

1. Open your web browser and navigate to `http://localhost:8000`
2. Type your question or request in the text area
3. Click "Send Message" to submit
4. The agent's response will appear on the page

### Example Questions

- "What is the company's expense policy for meals?"
- "How do I submit a travel expense report?"
- "What receipts do I need for reimbursement?"
- "I need to submit an expense for a business lunch"

## Project Structure

```
expense-chatbot/
├── main.py                 # FastAPI application
├── templates/
│   └── chat.html          # Jinja2 chat interface template
├── static/
│   └── styles.css         # CSS styling
├── requirements.txt       # Python dependencies
├── .env.example          # Environment variables template
└── README.md             # This file
```

## Technology Stack

- **FastAPI** (v0.115.6): Modern web framework
- **Azure AI Projects SDK** (v1.0.0+): Azure AI Foundry integration
- **Azure Identity** (v1.19.0): Azure authentication
- **Jinja2** (v3.1.5): HTML templating
- **Uvicorn** (v0.34.0): ASGI server

## How It Works

1. User submits a message through the web form
2. Application creates a new Azure AI thread (stateless mode)
3. User message is added to the thread
4. Agent processes the message and generates a response
5. Response is retrieved and displayed to the user

## Troubleshooting

### Authentication Errors

If you see authentication errors:
- Verify your Azure credentials are correctly configured
- Ensure you have appropriate permissions in Azure AI Foundry
- Try running `az login` if using Azure CLI authentication

### Agent Not Responding

- Check that `AGENT_ID` and `PROJECT_ENDPOINT` are correct
- Verify the agent exists and is deployed in Azure AI Foundry
- Check Azure portal for agent status

### Module Import Errors

- Ensure all dependencies are installed: `pip install -r requirements.txt`
- Verify you're using Python 3.9 or higher: `python --version`

## Security Considerations

- Never commit `.env` file with real credentials to version control
- Use managed identities when deploying to Azure
- Consider implementing rate limiting for production use
- Add authentication/authorization for production deployments

## License

This project is provided as-is for demonstration purposes.

## Support

For issues with:
- Azure AI Foundry: Check [Azure AI documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- FastAPI: See [FastAPI documentation](https://fastapi.tiangolo.com/)
