// Session state
let threadId = null;
let isProcessing = false;

// DOM elements
const chatMessages = document.getElementById('chatMessages');
const messageInput = document.getElementById('messageInput');
const sendButton = document.getElementById('sendButton');
const typingIndicator = document.getElementById('typingIndicator');
const examplePrompts = document.getElementById('examplePrompts');

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    enableInput();
    setupExamplePrompts();
});

// Enable input elements
function enableInput() {
    messageInput.disabled = false;
    sendButton.disabled = false;
    messageInput.focus();
}

// Disable input elements
function disableInput() {
    messageInput.disabled = true;
    sendButton.disabled = true;
}

// Add message to chat
function addMessage(type, content) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type === 'bot' ? 'bot-message' : type === 'error' ? 'error-message' : 'user-message'}`;

    const contentDiv = document.createElement('div');
    contentDiv.className = 'message-content';

    // Convert line breaks to <br> tags and preserve formatting
    const formattedContent = content.replace(/\n/g, '<br>');
    contentDiv.innerHTML = formattedContent;

    messageDiv.appendChild(contentDiv);
    chatMessages.appendChild(messageDiv);

    // Scroll to bottom
    scrollToBottom();
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

// Scroll chat to bottom
function scrollToBottom() {
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// Setup example prompts
function setupExamplePrompts() {
    const promptButtons = document.querySelectorAll('.example-prompt');

    promptButtons.forEach(button => {
        button.addEventListener('click', () => {
            const prompt = button.getAttribute('data-prompt');
            messageInput.value = prompt;
            adjustTextareaHeight();
            messageInput.focus();

            // Hide example prompts after first use
            hideExamplePrompts();
        });
    });
}

// Hide example prompts
function hideExamplePrompts() {
    if (examplePrompts) {
        examplePrompts.classList.add('hidden');
    }
}

// Send message via HTTP POST
async function sendMessage() {
    const message = messageInput.value.trim();

    if (!message || isProcessing) {
        return;
    }

    // Hide example prompts when first message is sent
    hideExamplePrompts();

    // Add user message to chat
    addMessage('user', message);

    // Clear input
    messageInput.value = '';
    adjustTextareaHeight();

    // Disable input while processing
    isProcessing = true;
    disableInput();
    showTypingIndicator();

    try {
        // Send POST request to API
        const response = await fetch('/api/chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                message: message,
                thread_id: threadId
            })
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.detail || 'Failed to get response');
        }

        const data = await response.json();

        // Store thread ID for subsequent messages
        threadId = data.thread_id;

        // Hide typing indicator and add bot response
        hideTypingIndicator();
        addMessage('bot', data.response);

    } catch (error) {
        console.error('Error sending message:', error);
        hideTypingIndicator();
        addMessage('error', `Error: ${error.message}`);
    } finally {
        // Re-enable input
        isProcessing = false;
        enableInput();
    }
}

// Auto-adjust textarea height
function adjustTextareaHeight() {
    messageInput.style.height = 'auto';
    messageInput.style.height = messageInput.scrollHeight + 'px';
}

// Event listeners
sendButton.addEventListener('click', sendMessage);

messageInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        sendMessage();
    }
});

messageInput.addEventListener('input', adjustTextareaHeight);
