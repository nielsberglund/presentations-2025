document.addEventListener('DOMContentLoaded', () => {
    const darkModeToggle = document.getElementById('darkmode-toggle');
    
    // Check for saved theme preference or use preferred color scheme
    const savedTheme = localStorage.getItem('theme');
    const prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    // Apply the saved theme or use the preferred color scheme
    if (savedTheme === 'dark' || (!savedTheme && prefersDarkScheme)) {
        document.body.classList.add('dark-mode');
        darkModeToggle.checked = true;
    }
    
    // Toggle dark/light mode when the switch is clicked
    darkModeToggle.addEventListener('change', () => {
        if (darkModeToggle.checked) {
            document.body.classList.add('dark-mode');
            localStorage.setItem('theme', 'dark');
        } else {
            document.body.classList.remove('dark-mode');
            localStorage.setItem('theme', 'light');
        }
    });
    
    // Add click listener to CTA button
    const ctaButton = document.querySelector('.cta-button');
    ctaButton.addEventListener('click', () => {
        alert('Thank you for your interest! This is where the action would happen.');
    });
});