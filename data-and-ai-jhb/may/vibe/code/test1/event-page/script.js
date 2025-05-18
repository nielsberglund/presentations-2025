document.addEventListener('DOMContentLoaded', function() {
    // Theme toggle functionality
    const themeToggleBtn = document.getElementById('theme-toggle-btn');
    const themeIcon = themeToggleBtn.querySelector('i');
    const themeText = themeToggleBtn.querySelector('span');
    
    // Check for saved theme preference or use preferred color scheme
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark' || (!savedTheme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.body.classList.add('dark-mode');
        updateThemeButton(true);
    }
    
    // Toggle theme when button is clicked
    themeToggleBtn.addEventListener('click', function() {
        const isDarkMode = document.body.classList.toggle('dark-mode');
        
        // Update button appearance
        updateThemeButton(isDarkMode);
        
        // Save preference to localStorage
        localStorage.setItem('theme', isDarkMode ? 'dark' : 'light');
    });
    
    // Button animation functionality
    const buttons = document.querySelectorAll('.cta-button');
    
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Prevent animation on anchor links when just navigating on the same page
            if (this.getAttribute('href') && this.getAttribute('href').startsWith('#')) {
                const targetId = this.getAttribute('href').substring(1);
                const targetElement = document.getElementById(targetId);
                
                if (targetElement) {
                    e.preventDefault();
                    targetElement.scrollIntoView({ behavior: 'smooth' });
                }
            }
            
            // Add animation class
            this.classList.add('animate');
            
            // Remove animation class after animation completes
            setTimeout(() => {
                this.classList.remove('animate');
            }, 500);
        });
    });
    
    function updateThemeButton(isDarkMode) {
        if (isDarkMode) {
            themeIcon.className = 'fas fa-sun';
            themeText.textContent = 'Light Mode';
        } else {
            themeIcon.className = 'fas fa-moon';
            themeText.textContent = 'Dark Mode';
        }
    }
    
    // Form submission handling
    const registrationForm = document.getElementById('registration-form');
    if (registrationForm) {
        registrationForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const name = document.getElementById('name').value;
            const email = document.getElementById('email').value;
            
            // In a real application, you would send this data to a server
            // For demo purposes, we'll show an alert
            alert(`Thank you ${name} for registering! We will send updates to ${email}`);
            
            // Reset form
            registrationForm.reset();
        });
    }
});