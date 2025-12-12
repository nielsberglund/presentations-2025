# watch-mcp.ps1
$logFile = ".\mcp-traffic.jsonl"

Write-Host "Waiting for MCP traffic on $logFile..." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop." -ForegroundColor DarkGray

# Wait until the file is created by your interceptor
while (-not (Test-Path $logFile)) { Start-Sleep -Milliseconds 500 }

# Tail the file in real-time
Get-Content $logFile -Wait -Tail 0 | ForEach-Object {
    if ([string]::IsNullOrWhiteSpace($_)) { return }
    
    try {
        # 1. Parse the log entry (Timestamp, Direction, Message)
        $entry = $_ | ConvertFrom-Json
        
        # 2. Determine Color and Prefix based on direction
        if ($entry.direction -eq "CLIENT_REQ") {
            $color = "Green"      # Green for requests going OUT to server
            $prefix = "--> CLIENT REQUEST"
        } else {
            $color = "Cyan"       # Cyan for responses coming IN from server
            $prefix = "<-- SERVER RESPONSE"
        }

        # 3. Format the Timestamp
        $time = [DateTime]::Parse($entry.timestamp).ToString("HH:mm:ss")

        # 4. Print Header
        Write-Host "[$time] $prefix" -ForegroundColor $color
        
        # 5. Pretty-Print the inner content
        # The 'message' is usually a JSON string string, so we parse it again to format it
        try {
            if ($entry.message.Trim().StartsWith("{")) {
                $innerJson = $entry.message | ConvertFrom-Json
                $pretty = $innerJson | ConvertTo-Json -Depth 10
                Write-Host $pretty -ForegroundColor Gray
            } else {
                # If it's not JSON (e.g. debug text), just print it
                Write-Host $entry.message -ForegroundColor Gray
            }
        } catch {
            Write-Host $entry.message -ForegroundColor Gray
        }
        
        # Separator for readability
        Write-Host "--------------------------------------------------" -ForegroundColor DarkGray

    } catch {
        # Ignore partial writes or errors to keep the stream moving
    }
}