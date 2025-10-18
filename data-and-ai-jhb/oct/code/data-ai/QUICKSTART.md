# Quick Start Guide

Get your Expense Report Chatbot running in 5 minutes!

## Prerequisites

- Python 3.8+
- Azure AI Foundry project with a deployed agent
- Azure CLI installed (for authentication)

## Step-by-Step Setup

### 1. Install Dependencies

```bash
pip install -r requirements.txt
```

### 2. Configure Environment

```bash
# Copy the example environment file
cp .env.example .env
```

Edit `.env` with your Azure AI configuration:

```env
AZURE_AI_PROJECT_ENDPOINT=https://your-project-name.cognitiveservices.azure.com/
AZURE_AI_AGENT_ID=asst_abc123xyz456
```

**Where to find these values:**

- **AZURE_AI_PROJECT_ENDPOINT:**
  1. Go to [Azure AI Foundry](https://ai.azure.com)
  2. Open your project
  3. Go to Settings → Project details
  4. Copy the endpoint URL

- **AZURE_AI_AGENT_ID:**
  1. In Azure AI Foundry, go to Agents
  2. Select your agent
  3. Copy the Agent ID

### 3. Authenticate with Azure

```bash
az login
```

Follow the browser prompts to authenticate.

### 4. Run the Application

#### Option A: Using the run script (macOS/Linux)

```bash
./run.sh
```

#### Option B: Using Python directly

```bash
python app.py
```

#### Option C: Using Uvicorn with auto-reload

```bash
uvicorn app:app --reload
```

### 5. Access the Application

Open your browser and go to:
```
http://localhost:8000
```

## Test Questions

Try asking your chatbot:

- "What expenses can I claim?"
- "How do I submit an expense report?"
- "What documentation do I need for travel expenses?"
- "Are meal expenses reimbursable?"

## Troubleshooting

### Error: "DefaultAzureCredential failed to retrieve a token"

**Solution:** Make sure you're logged in to Azure CLI:
```bash
az login
```

### Error: "AZURE_AI_PROJECT_ENDPOINT and AZURE_AI_AGENT_ID must be set"

**Solution:** Check that:
1. Your `.env` file exists
2. It contains both required variables
3. The values are correct (no quotes needed)

### Error: "Connection refused" or "Cannot connect"

**Solution:** Verify your Azure AI project endpoint is correct and your agent is deployed.

### Agent not responding correctly

**Solution:**
1. Check that your agent is deployed in Azure AI Foundry
2. Verify the agent ID is correct
3. Ensure your agent has the necessary knowledge base or instructions

## Project Structure

```
expense-chatbot/
├── app.py                 # Main application
├── requirements.txt       # Python dependencies
├── .env                   # Your configuration (create this)
├── .env.example          # Configuration template
├── run.sh                # Startup script
├── templates/
│   └── chat.html         # Chat interface
├── README.md             # Full documentation
├── ARCHITECTURE.md       # Technical details
└── QUICKSTART.md        # This file
```

## Next Steps

- **Customize the UI:** Edit `templates/chat.html`
- **Update agent instructions:** Modify your agent in Azure AI Foundry
- **Add authentication:** Implement user login
- **Deploy to production:** See README.md for deployment options

## Need Help?

- **Azure AI Foundry Docs:** https://learn.microsoft.com/azure/ai-foundry/
- **FastAPI Docs:** https://fastapi.tiangolo.com/
- **Azure SDK Docs:** https://learn.microsoft.com/python/api/overview/azure/

## Development Tips

**Enable auto-reload during development:**
```bash
uvicorn app:app --reload --host 0.0.0.0 --port 8000
```

**Check application health:**
```bash
curl http://localhost:8000/health
```

**View logs:**
The application logs to stdout, so watch your terminal for messages.

## Common Commands

```bash
# Install dependencies
pip install -r requirements.txt

# Run with auto-reload
uvicorn app:app --reload

# Run on different port
uvicorn app:app --port 3000

# Run with multiple workers (production)
uvicorn app:app --workers 4

# Check Azure login status
az account show

# Login to Azure
az login
```

That's it! You should now have a working expense report chatbot.
