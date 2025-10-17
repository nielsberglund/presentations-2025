// Session management
let sessionId = null;

// DOM elements
const chatMessages = document.getElementById('chat-messages');
const messageInput = document.getElementById('message-input');
const sendButton = document.getElementById('send-button');
const typingIndicator = document.getElementById('typing-indicator');
const connectionStatus = document.getElementById('connection-status');
const exampleButtons = document.querySelectorAll('.example-btn');

// Initialize on page load
window.addEventListener('load', () => {
    console.log('Application loaded');
    updateConnectionStatus('connected');
    sendButton.disabled = false;

    // Add welcome message
    addMessage("Hello! I'm your Expense Report Assistant. I can help you with expense policies and submitting expense reports. How can I assist you today?", 'assistant');
});

// Update connection status indicator
function updateConnectionStatus(status) {
    connectionStatus.className = `status-indicator ${status}`;

    const statusText = connectionStatus.querySelector('.status-text');
    switch(status) {
        case 'connected':
            statusText.textContent = 'Ready';
            break;
        case 'disconnected':
            statusText.textContent = 'Disconnected';
            break;
        case 'processing':
            statusText.textContent = 'Processing...';
            break;
        default:
            statusText.textContent = 'Ready';
    }
}

// Add message to chat
function addMessage(text, role) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${role}`;

    const bubbleDiv = document.createElement('div');
    bubbleDiv.className = 'message-bubble';
    bubbleDiv.textContent = text;

    messageDiv.appendChild(bubbleDiv);
    chatMessages.appendChild(messageDiv);

    // Scroll to bottom
    scrollToBottom();
}

// Send message
async function sendMessage() {
    const message = messageInput.value.trim();

    if (!message) {
        return;
    }

    // Disable input while processing
    sendButton.disabled = true;
    messageInput.disabled = true;

    // Add user message to chat
    addMessage(message, 'user');

    // Clear input
    messageInput.value = '';
    messageInput.style.height = 'auto';

    // Show typing indicator
    showTypingIndicator();
    updateConnectionStatus('processing');

    try {
        // Send message to API
        const response = await fetch('/api/chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                message: message,
                session_id: sessionId
            })
        });

        hideTypingIndicator();

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.detail || 'Failed to send message');
        }

        const data = await response.json();

        // Store session ID for future messages
        sessionId = data.session_id;

        // Add assistant response to chat
        addMessage(data.message, 'assistant');

        updateConnectionStatus('connected');

    } catch (error) {
        console.error('Error sending message:', error);
        hideTypingIndicator();
        addMessage(`Error: ${error.message}`, 'error');
        updateConnectionStatus('disconnected');
    } finally {
        // Re-enable input
        sendButton.disabled = false;
        messageInput.disabled = false;
        messageInput.focus();
    }
}

// Show typing indicator
function showTypingIndicator() {
    typingIndicator.style.display = 'flex';
    scrollToBottom();
}

// Hide typing indicator
function hideTypingIndicator() {
    typingIndicator.style.display = 'none';
}

// Scroll to bottom of chat
function scrollToBottom() {
    const chatContainer = document.querySelector('.chat-container');
    chatContainer.scrollTop = chatContainer.scrollHeight;
}

// Auto-resize textarea
messageInput.addEventListener('input', () => {
    messageInput.style.height = 'auto';
    messageInput.style.height = Math.min(messageInput.scrollHeight, 120) + 'px';
});

// Send message on button click
sendButton.addEventListener('click', sendMessage);

// Send message on Enter (without Shift)
messageInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        sendMessage();
    }
});

// Handle example button clicks
exampleButtons.forEach(button => {
    button.addEventListener('click', () => {
        const message = button.getAttribute('data-message');
        messageInput.value = message;
        messageInput.focus();
        messageInput.style.height = 'auto';
        messageInput.style.height = messageInput.scrollHeight + 'px';
    });
});
