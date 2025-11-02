
---

### SECTION 1: INTRODUCTION (3-4 minutes)

**Slide 1: Title Slide**
- Title + Tagline
- Your name, MVP status
- Event logo
- QR code to GitHub repo

**Slide 2: The Problem**
- Title: "The MCP Tutorial Problem"
- Visual: Split screen showing
  - Left: 100+ tutorials on "How to Build an MCP Server"
  - Right: Nearly zero on "How to Use MCP in Production"
- Quote bubble: "Great! I built an MCP server. Now what?"

**Slide 3: What We're Actually Building Today**
- Screenshot mockup of the event management web app
- Key features listed:
  - Chat interface for database operations
  - Natural language → SQL operations
  - Real business logic (capacity checks, conflicts, validation)
  - Reporting and analytics
- Tagline: "Production-ready, not demo-ware"

---

### SECTION 2: MCP FUNDAMENTALS (5-7 minutes)

**Slide 4: What is MCP? (The 60-Second Version)**
- Simple diagram showing:
  - Your Application (C# Web App)
  - ↔ MCP Protocol
  - ↔ MCP Server (MSSQL)
  - ↔ Resource (SQL Server Database)
- Key point: "Standard protocol for AI-to-tool communication"
- Analogy: "Think REST API, but designed specifically for AI agents"

**Slide 5: MCP Architecture Components**
- Three key pieces with icons:
  1. **MCP Host/Client** (Your .NET application)
  2. **MCP Server** (MSSQL MCP Server - already exists!)
  3. **Transport Layer** (stdio, HTTP, SSE)
- Highlight: "We're building #1, using #2 that already exists"

**Slide 6: Why MCP Matters for Enterprise Apps**
- Bullet points:
  - ✅ Standardized AI-to-tool integration
  - ✅ Security and permission boundaries
  - ✅ Context management built-in
  - ✅ Tool discovery and invocation
  - ✅ Reusable servers across different apps
- Bottom line: "Don't reinvent the wheel for every AI integration"

**Slide 7: The MSSQL MCP Server**
- What it provides:
  - Database schema inspection
  - CRUD operations
  - Query execution
  - Table creation and management
- Screenshot/reference to the Azure SQL blog post
- Key insight: "Microsoft built this. We're going to use it."

**Slide 8: Our Architecture Today**
```
┌─────────────────────────────────────┐
│   ASP.NET Core Web App              │
│   ├── Blazor/React UI (Chat)        │
│   ├── C# Backend                    │
│   └── Azure AI Integration          │
└──────────────┬──────────────────────┘
               │
               ↓ (MCP Protocol via stdio)
┌──────────────┴──────────────────────┐
│   MSSQL MCP Server (Node.js)        │
└──────────────┬──────────────────────┘
               │
               ↓
┌──────────────┴──────────────────────┐
│   SQL Server Database                │
│   (Event Management Schema)          │
└─────────────────────────────────────┘
```
- "All communication happens through Azure AI + MCP"

---

### SECTION 3: LET'S BUILD IT (30-35 minutes - MOSTLY CODE)

**Slide 9: Demo Roadmap**
- Title: "Susan's Journey - Our Coding Path"
- Numbered steps (matching what we'll code):
  1. Setting up the solution & MCP connection
  2. Building the chat interface
  3. Schema creation through conversation
  4. Adding speakers and sessions
  5. Handling registration logic
  6. Generating reports
- "Follow along: [GitHub URL]"

**[Transition to Visual Studio / VS Code]**

---

### CODING DEMOS (30-35 minutes)

**Demo 1: Project Setup & MCP Connection (5-7 min)**
- Show the project structure
- Configure MSSQL MCP Server connection
- Demonstrate MCP server discovery and tool listing
- Test basic connectivity

**Demo 2: Chat Interface + Azure AI Integration (6-8 min)**
- Build the chat UI component
- Wire up Azure AI Foundry connection
- Show how to pass MCP tools to Azure AI
- First conversation: "What tables exist in the database?"

**Demo 3: Susan's First Task - Database Creation (5-6 min)**
- Use the actual prompt from the PDF
- Show Azure AI calling MCP tools
- Walk through the C# code handling the responses
- Verify tables created successfully

**Demo 4: Data Operations - Speakers & Sessions (6-8 min)**
- Insert speakers through conversation
- Add sessions with capacity validation
- Show error handling (e.g., room capacity exceeded)
- Demonstrate the C# → Azure AI → MCP → SQL flow

**Demo 5: Complex Logic - Attendee Registration (4-5 min)**
- Register attendees for sessions
- Handle business rules (capacity checks)
- Show validation and user feedback
- Demonstrate state management

**Demo 6: Reporting & Analytics (3-4 min)**
- Generate post-event popularity report
- Show different query patterns
- Demonstrate how to format AI responses

---

### SECTION 4: WRAP-UP (4-5 minutes)

**Slide 10: What We Actually Built**
- Quick stats:
  - Lines of C# code: ~XXX
  - MCP tool calls: ~XX types
  - Zero SQL written by user
  - Production-ready patterns ✓
- Side-by-side: User's natural language → Actual SQL executed

**Slide 11: Key Takeaways**
1. **Integration > Creation**: Use existing MCP servers
2. **Azure AI as Orchestrator**: Let it handle tool selection and invocation
3. **Business Logic Matters**: Validation, error handling, UX are still yours
4. **Prompt Engineering**: Clear, specific instructions to Azure AI
5. **Debugging**: Know how to trace AI → MCP → Database

**Slide 12: Beyond the Demo**
- Production considerations:
  - Authentication & authorization
  - Rate limiting & cost management
  - Logging and observability
  - Testing AI-powered features
  - Fallback strategies
- "This works. Now make it bulletproof."

**Slide 13: Resources**
- GitHub repo (with QR code)
- Azure SQL blog post (Susan's journey)
- MCP documentation
- Azure AI Foundry docs
- Your contact info / social media
- "Questions?"

**Slide 14: Thank You**
- Your contact details
- QR code to slides
- Event hashtags
- "Let's ship some AI features! 🚀"

---

## Slide Design Notes:

- **Minimal text**: You're coding for 30+ minutes, slides are just signposts
- **Code-friendly**: Dark theme, readable fonts
- **Visual hierarchy**: Use icons, diagrams over bullet points where possible
- **Consistent branding**: Include .NET Conf / Data & AI Community Day logos
- **Technical but accessible**: This is for developers who want to build

---

## Backup Slides (If Needed):

**Backup 1: Troubleshooting Common Issues**
- MCP server connection failures
- Azure AI rate limits
- Prompt engineering tips

**Backup 2: Alternative Architectures**
- Using HTTP transport instead of stdio
- Multiple MCP servers
- Hybrid approaches

**Backup 3: Cost Considerations**
- Azure AI pricing
- Token usage optimization
- Caching strategies

---

Would you like me to:
1. Create detailed speaker notes for any of these slides?
2. Suggest specific code examples to prepare beforehand vs. live code?
3. Design a timeline breakdown showing exactly how many minutes per section?
4. Help with the GitHub repo structure for your demo code?