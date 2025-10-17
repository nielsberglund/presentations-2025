const chatMessages = document.getElementById('chat-messages');
const chatForm = document.getElementById('chat-form');
const messageInput = document.getElementById('message-input');
const sendButton = document.getElementById('send-button');
const loading = document.getElementById('loading');

function addMessage(content, isUser) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${isUser ? 'user' : 'agent'}`;

    const contentDiv = document.createElement('div');
    contentDiv.className = 'message-content';
    contentDiv.textContent = content;

    messageDiv.appendChild(contentDiv);
    chatMessages.appendChild(messageDiv);

    // Scroll to bottom
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

chatForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const message = messageInput.value.trim();
    if (!message) return;

    // Add user message to chat
    addMessage(message, true);

    // Clear input and disable form
    messageInput.value = '';
    sendButton.disabled = true;
    messageInput.disabled = true;
    loading.classList.add('active');

    try {
        // Send message to backend
        const response = await fetch('/api/chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ message: message })
        });

        if (!response.ok) {
            throw new Error('Failed to get response from agent');
        }

        const data = await response.json();

        // Add agent response to chat
        addMessage(data.response, false);

    } catch (error) {
        console.error('Error:', error);
        addMessage('Sorry, I encountered an error. Please try again.', false);
    } finally {
        // Re-enable form
        sendButton.disabled = false;
        messageInput.disabled = false;
        loading.classList.remove('active');
        messageInput.focus();
    }
});

// Focus input on load
messageInput.focus();
