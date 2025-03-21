document.addEventListener('DOMContentLoaded', function() {
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    const body = document.body;
    
    // Check for saved user preference
    const savedDarkMode = localStorage.getItem('darkMode');
    
    // If dark mode was previously enabled, apply it
    if (savedDarkMode === 'enabled') {
        body.classList.add('dark-mode');
        darkModeToggle.checked = true;
    }
    
    // Listen for toggle changes
    darkModeToggle.addEventListener('change', function() {
        if (this.checked) {
            // Dark mode ON
            body.classList.add('dark-mode');
            localStorage.setItem('darkMode', 'enabled');
        } else {
            // Dark mode OFF
            body.classList.remove('dark-mode');
            localStorage.setItem('darkMode', 'disabled');
        }
    });
    
    // Call-to-Action Button Click Event
    const ctaButton = document.querySelector('.cta-button');
    ctaButton.addEventListener('click', function() {
        alert('Thank you for your interest! This is where a sign-up form or further information would appear.');
    });
});