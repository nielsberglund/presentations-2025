
# agent instructions
You are an AI assistant for corporate expenses.
You answer questions about expenses based on the expenses policy data.
If a user wants to submit an expense claim, you get their email address, a description of the claim, and the amount to be claimed and write the claim details to a text file that the user can download.

What's the maximum I can claim for meals?

Explain the expense policies to me like I was a 5 year old.

I'd like to submit a claim for a meal.

Breakfast $201

Excellent! I see that the html code for the web page is in the app.py file, can you break that out into a separate file in a separate folder like static. Keep the html, the
js, and the css separate from each other.


can you think hard and plan to create a simple web application that is a chatbot calling into an Azure AI Foundry agent. The agent is named Agent-test-1 and the id is
asst_LtGz0IcvsISO19v0a4jWia9Y, the agent is an expense report agent, where the user can ask questions about the Expense policies as well as submitting expense report. I would
like to use Python and FastAPI. Use the latest versions of all packages, and use Context7 MCP server to find the latest documentation. Keep the logic as simple as possible. The application should use the correct latest syntax from the Azure AI Projects SDK (version 1.0.0). All methods should follow the pattern of
  project_client.agents.<resource>.<action>() which is the current standard for the library.Agent ID and endpoint should be read from environment variables. Requests and responses should be HTML, NOT web sockets.

Excellent! I see that the html code for the web page is in the app.py file, can you break that out into a separate file in a separate folder like static. Keep the html, the
js, and the css separate from each other.


https://nielsb-test-1-resource.services.ai.azure.com/api/projects/nielsb-test-1

asst_LtGz0IcvsISO19v0a4jWia9Y

copy .\.env.example .\.env

context7 API
ctx7sk-695c9ab2-bcc9-411a-beb7-035eb195829b

claude mcp add --transport http context7 https://mcp.context7.com/mcp --header "CONTEXT7_API_KEY: ctx7sk-695c9ab2-bcc9-411a-beb7-035eb195829b"

the following calls are using old syntax, please change to latest syntax: project_client.agents.create_thread, project_client.agents.create_message,
project_client.agents.create_and_process_run, project_client.agents.list_messages

