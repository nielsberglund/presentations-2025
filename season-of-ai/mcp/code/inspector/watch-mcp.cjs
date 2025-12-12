// watch-mcp.js (Universal Version)
const fs = require('fs');
const path = require('path');

const LOG_FILE = path.join(__dirname, 'mcp-traffic.jsonl');

// ANSI Color Codes
const RESET = '\x1b[0m';
const GREEN = '\x1b[32m'; // Requests
const CYAN = '\x1b[36m';  // Responses
const YELLOW = '\x1b[33m';
const GRAY = '\x1b[90m';

console.log(`${YELLOW}Waiting for MCP traffic on ${LOG_FILE}...${RESET}`);
console.log(`${GRAY}Press Ctrl+C to stop.${RESET}`);

// Ensure file exists
if (!fs.existsSync(LOG_FILE)) {
    fs.writeFileSync(LOG_FILE, '');
}

// Track how much of the file we have read
let currSize = fs.statSync(LOG_FILE).size;

// Watch the file for changes (polls every 100ms for responsiveness)
fs.watchFile(LOG_FILE, { interval: 100 }, (curr, prev) => {
    // If file grew, read the new part
    if (curr.size > currSize) {
        const stream = fs.createReadStream(LOG_FILE, {
            start: currSize,
            end: curr.size
        });

        stream.on('data', (chunk) => {
            const newContent = chunk.toString();
            // Handle cases where multiple lines come in one chunk
            const lines = newContent.split('\n');
            lines.forEach(line => {
                if (line.trim()) processLine(line);
            });
        });

        currSize = curr.size;
    }
    // Handle log rotation (file shrunk or deleted) if you restart the interceptor
    else if (curr.size < currSize) {
        currSize = 0; // Reset position
    }
});

function processLine(line) {
    try {
        const entry = JSON.parse(line);
        const isRequest = entry.direction === 'CLIENT_REQ';
        
        const color = isRequest ? GREEN : CYAN;
        const prefix = isRequest ? '--> CLIENT REQUEST' : '<-- SERVER RESPONSE';
        
        // Format Timestamp (simple HH:MM:SS)
        const time = new Date(entry.timestamp).toLocaleTimeString('en-US', { hour12: false });

        // 1. Print Header
        console.log(`${color}[${time}] ${prefix}${RESET}`);

        // 2. Pretty Print Body
        try {
            if (entry.message.trim().startsWith('{')) {
                const innerJson = JSON.parse(entry.message);
                console.log(GRAY + JSON.stringify(innerJson, null, 2) + RESET);
            } else {
                console.log(GRAY + entry.message + RESET);
            }
        } catch (e) {
            console.log(GRAY + entry.message + RESET);
        }
        
        console.log(`${GRAY}--------------------------------------------------${RESET}`);

    } catch (err) {
        // Ignore partial lines
    }
}