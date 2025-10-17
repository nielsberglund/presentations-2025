# Expense Report Chatbot

A real-time web chatbot application that integrates with Azure AI Foundry Agent Service to help users with expense policies and expense report submissions.

## Features

- Real-time chat interface using WebSocket
- Azure AI Foundry Agent integration (Agent-test-1)
- Modern, responsive UI
- Persistent conversation threads
- Support for expense policy queries and report submission
- Auto-reconnection on disconnect
- Typing indicators

## Prerequisites

- Python 3.9 or later
- Azure AI Foundry project with an active agent
- Azure CLI installed and configured
- Valid Azure subscription

## Setup Instructions

### 1. Clone or Navigate to the Project

```bash
cd expense-chatbot
```

### 2. Create a Virtual Environment

```bash
python -m venv venv

# On Windows
venv\Scripts\activate

# On macOS/Linux
source venv/bin/activate
```

### 3. Install Dependencies

```bash
pip install -r requirements.txt
```

### 4. Configure Azure Authentication

Make sure you're logged in to Azure CLI:

```bash
az login
```

This is required because the application uses `DefaultAzureCredential()` for authentication.

### 5. Set Up Environment Variables

1. Copy the example environment file:
   ```bash
   cp .env.example .env
   ```

2. Edit `.env` and update with your Azure AI Foundry details:
   ```
   AZURE_SUBSCRIPTION_ID=your-subscription-id-here
   AZURE_RESOURCE_GROUP_NAME=your-resource-group-name
   AZURE_PROJECT_NAME=nielsb-test-1
   AGENT_ID=asst_LtGz0IcvsISO19v0a4jWia9Y
   ```

   To find these values:
   - **Subscription ID**: Go to Azure Portal > Subscriptions, or run `az account show --query id -o tsv`
   - **Resource Group Name**: Go to Azure Portal > Resource Groups, or check your AI Foundry project
   - **Project Name**: Found in Azure AI Foundry portal under your project settings
   - **Agent ID**: Found in Azure AI Foundry under Agents section

### 6. Run the Application

```bash
python app.py
```

Or using uvicorn directly:

```bash
uvicorn app:app --reload --host 0.0.0.0 --port 8000
```

### 7. Access the Application

Open your browser and navigate to:
```
http://localhost:8000
```

## Project Structure

```
expense-chatbot/
├── app.py                 # FastAPI backend with WebSocket support
├── requirements.txt       # Python dependencies
├── .env                   # Environment variables (not in git)
├── .env.example          # Environment variables template
├── README.md             # This file
└── static/
    ├── index.html        # Chat interface HTML
    ├── styles.css        # UI styling
    └── script.js         # WebSocket client logic
```

## How It Works

1. **Backend (FastAPI)**:
   - Manages WebSocket connections for real-time communication
   - Creates a new conversation thread for each connection
   - Forwards user messages to the Azure AI Foundry agent
   - Polls for agent responses and streams them back to the client

2. **Frontend (HTML/JS)**:
   - Establishes WebSocket connection to the backend
   - Provides a clean chat interface
   - Displays user and assistant messages
   - Shows typing indicators during agent processing
   - Auto-reconnects on connection loss

3. **Azure AI Foundry Agent**:
   - Agent ID: `asst_LtGz0IcvsISO19v0a4jWia9Y`
   - Name: Agent-test-1
   - Purpose: Expense report assistant
   - Capabilities: Answers questions about expense policies and helps with expense report submission

## API Endpoints

- `GET /` - Serves the main chat interface
- `GET /health` - Health check endpoint
- `WebSocket /ws` - WebSocket endpoint for real-time chat

## Usage Examples

Try asking the agent:
- "What is the expense policy for meals?"
- "How do I submit an expense report?"
- "What receipts do I need to keep?"
- "What's the reimbursement limit for hotel stays?"

## Troubleshooting

### Authentication Issues

If you see authentication errors:
1. Ensure you're logged in: `az login`
2. Verify your Azure subscription is active
3. Check that you have access to the Azure AI Foundry project

### Connection Issues

If the WebSocket connection fails:
1. Check that the backend is running on port 8000
2. Verify no firewall is blocking the connection
3. Look at the browser console for detailed error messages

### Agent Not Responding

If the agent doesn't respond:
1. Verify the `AZURE_SUBSCRIPTION_ID`, `AZURE_RESOURCE_GROUP_NAME`, and `AZURE_PROJECT_NAME` are correct
2. Check that the `AGENT_ID` matches your agent
3. Ensure the agent is active in Azure AI Foundry portal
4. Check the backend logs for detailed error messages

## Development

To run in development mode with auto-reload:

```bash
uvicorn app:app --reload --host 0.0.0.0 --port 8000
```

## Security Notes

- The application uses `DefaultAzureCredential()` which works with Azure CLI authentication
- No API keys are hardcoded in the application
- Environment variables are used for configuration
- `.env` file should never be committed to version control

## Contributing

Feel free to submit issues and enhancement requests!

## License

MIT License - feel free to use this project for your own purposes.
