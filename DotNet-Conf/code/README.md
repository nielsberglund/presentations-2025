# MS SQL MCP

Clone the MSSQL MCP Server repo:
`git clone https://github.com/Azure-Samples/SQL-AI-samples.git`

CD into the the Node project:
`cd SQL-AI-samples/MssqlMcp/Node`

Install and build dependencies:
`npm install`

After installation find the `index.js` file inside a newly created `dist` folder and copy the fully qualified path:
`W:\\OneDrive\\testcode\\mcptests\\SQL-AI-samples\\MssqlMcp\\Node\\dist\\index.js`

/Users/niels.berglund/repos/niels-pvt/SQL-AI-samples/MssqlMcp/Node/dist/index.js

/Users/niels.berglund/repos/niels-pvt/SQL-AI-samples/MssqlMcp/dotnet/MssqlMcp/bin/Debug/net8.0/MssqlMcp.exe

For Claude Desktop edit the config file





Data Source=localhost;User ID=sa;Password=Password1!;Pooling=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30


"CONNECTION_STRING": "Server=.;Database=MCPTestDB;Trusted_Connection=True;TrustServerCertificate=True; User ID=sa; Password=Password1!  "


## check the MCP server
npx @modelcontextprotocol/inspector --verbose

Transport type: STDIO
Command: node
Arguments:
<path to index.js>
/Users/niels.berglund/repos/niels-pvt/SQL-AI-samples/MssqlMcp/Node/dist/index.js
/Users/niels.berglund/repos/niels-pvt/sql-mcp-test-2/code/MCPServer/dist/index.js
/Users/nielsb/repos/presentations/presentations-2025/MssqlMcp/Node/dist/index.js

Environment variables:

