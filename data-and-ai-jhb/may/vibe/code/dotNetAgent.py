import os
import httpx

from langchain_openai import AzureChatOpenAI
from langchain.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain_core.tools import tool
from langchain.agents import AgentExecutor, create_openai_tools_agent


# Initialize the model
headers = {
    "Ocp-Apim-Subscription-Key": os.getenv("AZURE_OPENAI_SUBSCRIPTION_KEY"),
    "Content-Type": "application/json"
}
 
httpx_client = httpx.Client(headers=headers, verify=True)

llm = AzureChatOpenAI(azure_deployment="gpt-4o-002",
                      azure_endpoint=os.getenv("AZURE_OPENAI_ENDPOINT"),
                      openai_api_key=os.getenv("AZURE_OPENAI_API_KEY"),
                      api_version="2024-02-15-preview",
                      temperature=0.0,
                      http_client=httpx_client)

@tool
def write_contents_to_file(filepath: str, content: str) -> str:
  """Writes text contents to a specified file path or overwrites existing file contents. Creates the file if it doesn't exist already.

  Args:
    filepath: The file path of the file to write.
    content: The contents to write to file.

  Returns:
    str: A message indicating the file was written to.
  """
  print("Writing to", filepath)

  if os.path.dirname(filepath):
    os.makedirs(os.path.dirname(filepath), exist_ok=True)

  with open(filepath, "w", encoding="utf-8") as f:
      f.write(content)

  return f"Content was written to {filepath}."


def run_agent(input_text):
  
  # Define tools
  tools = [write_contents_to_file]

  # Build initial prompt
  prompt = ChatPromptTemplate.from_messages([
      ("system", "You are an expert in .net and c#."),
      ("human", "{input_text}"),
      MessagesPlaceholder(variable_name='agent_scratchpad')
  ])

  # Initialize the agent with the tools
  llm_with_tools = create_openai_tools_agent(llm,
                                            tools,
                                            prompt)

  # Initialize the agent executor (adds a memory component to the agent)
  agent_executor = AgentExecutor(agent=llm_with_tools,
                                tools=tools,
                                handle_parsing_errors=True
                                )

  response = agent_executor.invoke({"input_text": input_text})

  return response

# Use the agent executor

input_text = """"
Write me a .net 8.0 ASP.NET Core WEB API sports car project that has a project structure of models,controllers, repositories, interfaces with CRUD endpoints.
Make sure the endpoints are named Create,Update, Delete and getAllCars.
Make sure there is a sql db connection to a local Database ExoticCars and table tb_sportsCars using dapper in the controller for the CRUD Methods.
Make sure the model structure is Id, Brand , Model , Year , Horsepower, TopSpeed, Price.
Make sure the Interface is made up of the GetAllCars, GetCarById, CreateCar, UpdateCar, DeleteCar and compareCars by Id methods.
Make sure the the visual studio solution file included and with the csproj file.
Make sure the project uses swagger.
Make sure this api runs in a container.
Write all necessary files to disk in folder ExoticCarsTest.
"""

# Run the agent to generate the code.
result = run_agent(input_text)
print(result)

# Save the prompt to a file to refer back to
def append_text_with_line(file_path, text_to_append):
    with open(file_path, "a") as file:  # Open the file in append mode
        file.write(text_to_append + "\n")  # Append the text with a newline
        file.write("\n")  # Add an empty line

# Make sure these are forward slashes and not back slashes to depict a file path
file_path = "C:/Users/kalreeng/langchain-tutorial/intermediate/ExoticCarsDemo/SavedPrompts.txt"
append_text_with_line(file_path, input_text)