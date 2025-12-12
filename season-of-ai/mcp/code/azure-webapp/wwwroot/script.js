const chatContainer = document.getElementById('chat-container');
const userInput = document.getElementById('user-input');
const sendBtn = document.getElementById('send-btn');

// Auto-resize textarea
userInput.addEventListener('input', function() {
    this.style.height = 'auto';
    this.style.height = (this.scrollHeight) + 'px';
    
    if (this.value.trim().length > 0) {
        sendBtn.removeAttribute('disabled');
    } else {
        sendBtn.setAttribute('disabled', 'true');
    }
});

// Handle Enter key to submit (Shift+Enter for new line)
userInput.addEventListener('keydown', function(e) {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        if (this.value.trim().length > 0) {
            sendMessage();
        }
    }
});

sendBtn.addEventListener('click', sendMessage);

async function sendMessage() {
    const message = userInput.value.trim();
    if (!message) return;

    // Clear input
    userInput.value = '';
    userInput.style.height = 'auto';
    sendBtn.setAttribute('disabled', 'true');

    // Add user message
    appendMessage('user', message);

    // Add loading indicator
    const loadingId = appendLoading();

    try {
        const response = await fetch('/api/chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ message: message })
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();
        
        // Remove loading and add AI response
        removeMessage(loadingId);
        appendMessage('ai', data.message);

    } catch (error) {
        console.error('Error:', error);
        removeMessage(loadingId);
        appendMessage('ai', 'Sorry, I encountered an error processing your request. Please try again.');
    }
}

function appendMessage(role, text) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${role}-message`;
    
    const avatar = role === 'user' ? 'You' : 'AI';
    
    // Format text (simple replacement for newlines to <br>)
    const formattedText = text.replace(/\n/g, '<br>');

    messageDiv.innerHTML = `
        <div class="avatar">${avatar}</div>
        <div class="content">
            <p>${formattedText}</p>
        </div>
    `;
    
    chatContainer.appendChild(messageDiv);
    scrollToBottom();
    return messageDiv.id = 'msg-' + Date.now();
}

function appendLoading() {
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message ai-message';
    const id = 'loading-' + Date.now();
    messageDiv.id = id;
    
    messageDiv.innerHTML = `
        <div class="avatar">AI</div>
        <div class="content">
            <div class="typing-indicator">
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
            </div>
        </div>
    `;
    
    chatContainer.appendChild(messageDiv);
    scrollToBottom();
    return id;
}

function removeMessage(id) {
    const element = document.getElementById(id);
    if (element) {
        element.remove();
    }
}

function scrollToBottom() {
    chatContainer.scrollTop = chatContainer.scrollHeight;
}
