# MS SQL MCP

Clone the MSSQL MCP Server repo:
`git clone https://github.com/Azure-Samples/SQL-AI-samples.git`

If you want to use the Node MCP Server and connect to SQL Server with user-name password, you need to replace the file `/Node/src/index.ts` in the cloned repo, with the `index.ts` in the `Node` folder.

## Build Node MCP Server

Install and build dependencies:
`npm install`

After installation find the `index.js` file inside a newly created `dist` folder and copy the fully qualified path. On my machine:

Windows:
`W:\\OneDrive\\testcode\\mcptests\\SQL-AI-samples\\MssqlMcp\\Node\\dist\\index.js`

Mac:
/Users/niels.berglund/repos/niels-pvt/SQL-AI-samples/MssqlMcp/Node/dist/index.js

### Check the MCP Server

Run: `npx @modelcontextprotocol/inspector --verbose`

Set 
Command to node, 
Arguments to the full path to `index.js` (including `index.js`). 

Environments Variables:
SERVER_NAME
DATABASE_NAME
READONLY (set it to false)
SQL_USERNAME
SQL_PASSWORD
TRUST_SERVER_CERTIFICATE (set it to true)

## .NET MCP Server

Read the `README.md` file in the `..\MssqlMcp\dotnet` folder, and follow the instructions. Notice how to define the database connection string is different between the Node and .NET MCP server.






