// interceptor.js
const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

// --- CONFIGURATION ---
const SERVER_COMMAND = 'node';
// Double backslashes are correct, but let's be safe and use raw strings or path.join if needed.
const SERVER_ARGS = ['W:\\OneDrive\\testcode\\mcptests\\SQL-AI-samples\\MssqlMcp\\Node\\dist\\index.js'];
const LOG_FILE = path.join(__dirname, 'mcp-traffic.jsonl');
const ERROR_LOG = path.join(__dirname, 'interceptor-error.log');

// Clear logs
fs.writeFileSync(LOG_FILE, '');
fs.writeFileSync(ERROR_LOG, '');

// --- ENV VARS ---
const SERVER_ENV = {
    ...process.env,
    SERVER_NAME: '127.0.0.1',
    DATABASE_NAME: 'MCPTestDB',
    READONLY: 'false',
    SQL_USERNAME: 'sa',
    SQL_PASSWORD: 'Password1!',
    TRUST_SERVER_CERTIFICATE: 'true'
};

function logError(msg) {
    fs.appendFileSync(ERROR_LOG, `[${new Date().toISOString()}] ${msg}\n`);
}

logError("Starting Interceptor...");
logError(`Target Script: ${SERVER_ARGS[0]}`);

try {
    // Check if target file actually exists before spawning
    if (!fs.existsSync(SERVER_ARGS[0])) {
        throw new Error(`Target file not found: ${SERVER_ARGS[0]}`);
    }

    const mcpServer = spawn(SERVER_COMMAND, SERVER_ARGS, {
        env: SERVER_ENV,
        stdio: ['pipe', 'pipe', 'pipe'] // Explicitly define pipes
    });

    logError(`Process spawned with PID: ${mcpServer.pid}`);

    // --- WIRING ---

    // 1. Client (DotNet) -> Server
    process.stdin.pipe(mcpServer.stdin);
    
    // Log Input (Optional: Be careful not to break the pipe)
    process.stdin.on('data', (chunk) => {
        fs.appendFileSync(LOG_FILE, JSON.stringify({ 
            timestamp: new Date().toISOString(), 
            direction: 'CLIENT_REQ', 
            message: chunk.toString() 
        }) + '\n');
    });

    // 2. Server -> Client (DotNet)
    mcpServer.stdout.pipe(process.stdout);

    mcpServer.stdout.on('data', (chunk) => {
        fs.appendFileSync(LOG_FILE, JSON.stringify({ 
            timestamp: new Date().toISOString(), 
            direction: 'SERVER_RES', 
            message: chunk.toString() 
        }) + '\n');
    });

    // 3. Capture Stderr (The most important part for debugging!)
    mcpServer.stderr.on('data', (data) => {
        const msg = data.toString();
        logError(`STDERR from Server: ${msg}`);
        // Pass stderr through to .NET (optional, but good for visibility)
        process.stderr.write(data);
    });

    mcpServer.on('error', (err) => {
        logError(`Failed to start subprocess: ${err.message}`);
    });

    mcpServer.on('close', (code) => {
        logError(`Server process exited with code ${code}`);
        process.exit(code);
    });

} catch (err) {
    logError(`CRITICAL INTERCEPTOR ERROR: ${err.message}`);
    process.exit(1);
}