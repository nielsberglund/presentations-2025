# Application Architecture

## Overview

This application is a simple expense report chatbot that uses Azure AI Foundry Agent Service with a FastAPI backend and HTML-based frontend.

## Key Components

### 1. Backend (app.py)

**Framework:** FastAPI 0.115.5

**Azure SDK Integration:**
- **Package:** `azure-ai-projects==1.0.0b10`
- **Authentication:** `DefaultAzureCredential` from `azure-identity`
- **Client Pattern:** `project_client.agents.<resource>.<action>()`

**Key Methods Used:**
```python
# Client initialization
project_client = AIProjectClient(
    endpoint=AZURE_AI_PROJECT_ENDPOINT,
    credential=DefaultAzureCredential()
)

# Thread management
thread = project_client.agents.threads.create()

# Message creation
project_client.agents.messages.create(
    thread_id=thread_id,
    role="user",
    content=message
)

# Agent execution
run = project_client.agents.runs.create_and_process(
    thread_id=thread_id,
    agent_id=AZURE_AI_AGENT_ID
)

# Message retrieval
messages_response = project_client.agents.messages.list(thread_id=thread_id)
```

### 2. Frontend (templates/chat.html)

**Template Engine:** Jinja2 3.1.4

**Features:**
- Modern gradient design
- Responsive layout
- Message role differentiation (user vs assistant)
- Auto-scroll to latest message
- Form-based submission (no WebSockets)

### 3. Configuration

**Environment Variables (loaded via python-dotenv):**
- `AZURE_AI_PROJECT_ENDPOINT`: Azure AI Foundry project endpoint
- `AZURE_AI_AGENT_ID`: Deployed agent identifier

## Request Flow

```
User Browser
    |
    | 1. GET /
    v
FastAPI App
    |
    | 2. Render chat.html
    v
User Browser (displays chat interface)
    |
    | 3. POST /chat (message + session_id)
    v
FastAPI App
    |
    | 4. Get/Create thread for session
    v
Azure AI Project Client
    |
    | 5. Create message in thread
    v
Azure AI Agent Service
    |
    | 6. create_and_process (runs agent)
    v
Azure AI Agent Service (processes message)
    |
    | 7. List all messages in thread
    v
FastAPI App
    |
    | 8. Format messages and render chat.html
    v
User Browser (displays updated chat)
```

## Session Management

**Current Implementation:**
- In-memory dictionary: `conversation_threads[session_id] = thread_id`
- Simple session_id passed via hidden form field
- Thread persists for the session

**Production Recommendations:**
- Use Redis or similar for distributed sessions
- Implement session timeout and cleanup
- Store thread IDs in database for persistence
- Add user authentication and tie sessions to users

## Error Handling

1. **Startup Validation:**
   - Checks for required environment variables
   - Raises `ValueError` if missing

2. **Runtime Errors:**
   - `AzureError`: Catches Azure-specific errors (HTTP 500)
   - General `Exception`: Catches all other errors (HTTP 500)

3. **User Feedback:**
   - Error details included in HTTP response
   - Consider adding user-friendly error messages in production

## Security Considerations

**Current:**
- Uses `DefaultAzureCredential` (supports multiple auth methods)
- Environment variables for configuration
- `.gitignore` prevents `.env` from being committed

**Production Additions:**
- Add rate limiting (e.g., slowapi)
- Implement user authentication
- Add CORS configuration if needed
- Sanitize and validate user inputs
- Implement HTTPS/TLS
- Add request logging and monitoring

## Scalability

**Current Limitations:**
- In-memory session storage (not distributed)
- Synchronous request handling
- Single-threaded by default

**Scaling Strategy:**
- Deploy with multiple Uvicorn workers
- Use Redis for session storage
- Consider async/await patterns if needed
- Deploy to Azure App Service or Container Apps with auto-scaling

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| fastapi | 0.115.5 | Web framework |
| uvicorn[standard] | 0.34.0 | ASGI server |
| python-dotenv | 1.0.1 | Environment configuration |
| azure-ai-projects | 1.0.0b10 | Azure AI SDK |
| azure-identity | 1.19.0 | Azure authentication |
| jinja2 | 3.1.4 | Template engine |
| python-multipart | 0.0.20 | Form data parsing |

## Azure AI Agent Requirements

The agent deployed in Azure AI Foundry should:

1. Have access to expense policy documents (uploaded files or vectorized knowledge)
2. Be instructed to help with expense-related queries
3. Optionally have tools configured for:
   - Document search
   - Form submission
   - Integration with expense management systems

## Testing Locally

```bash
# Install dependencies
pip install -r requirements.txt

# Configure environment
cp .env.example .env
# Edit .env with your values

# Authenticate with Azure
az login

# Run the application
python app.py

# Access at http://localhost:8000
```

## Deployment Options

### Azure App Service
- Python runtime
- Enable Managed Identity
- Set environment variables in configuration
- Auto-scaling support

### Azure Container Apps
- Containerized deployment
- Better for microservices
- Built-in HTTPS and scaling

### Docker
- Platform-independent
- Easy local testing
- Can deploy to any container platform

## Monitoring and Observability

**Recommended Additions:**
- Application Insights integration
- Structured logging
- Request tracing
- Performance metrics
- Agent interaction logging

## Future Enhancements

1. **User Authentication:** Add OAuth2 or Azure AD authentication
2. **Message History:** Persist conversations to database
3. **File Uploads:** Support expense receipt uploads
4. **Streaming Responses:** Add server-sent events for real-time updates
5. **Multi-language Support:** Internationalization
6. **Admin Panel:** Manage sessions and view analytics
7. **Export Functionality:** Export conversations or expense reports
