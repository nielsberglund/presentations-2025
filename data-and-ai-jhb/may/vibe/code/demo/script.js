document.addEventListener('DOMContentLoaded', function() {
    // Handle theme toggle
    const themeToggleBtn = document.getElementById('theme-toggle-btn');
    const themeIcon = themeToggleBtn.querySelector('i');
    const body = document.body;
    
    // Check for saved theme preference or use device preference
    const savedTheme = localStorage.getItem('theme');
    
    if (savedTheme === 'dark' || (!savedTheme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        body.classList.remove('light-mode');
        body.classList.add('dark-mode');
        themeIcon.classList.remove('fa-moon');
        themeIcon.classList.add('fa-sun');
    }
    
    // Toggle theme when button is clicked
    themeToggleBtn.addEventListener('click', function() {
        if (body.classList.contains('dark-mode')) {
            body.classList.remove('dark-mode');
            body.classList.add('light-mode');
            themeIcon.classList.remove('fa-sun');
            themeIcon.classList.add('fa-moon');
            localStorage.setItem('theme', 'light');
        } else {
            body.classList.remove('light-mode');
            body.classList.add('dark-mode');
            themeIcon.classList.remove('fa-moon');
            themeIcon.classList.add('fa-sun');
            localStorage.setItem('theme', 'dark');
        }
    });
    
    // Load events from data
    loadEvents();

    // Contact form submission handler
    const contactForm = document.getElementById('contact-form');
    
    if (contactForm) {
        contactForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            // Get form values
            const name = document.getElementById('name').value.trim();
            const email = document.getElementById('email').value.trim();
            const message = document.getElementById('message').value.trim();
            
            // For demo purposes - show alert instead of sending data
            alert(`Thank you, ${name}! Your message has been received. We will get back to you at ${email} soon.`);
            
            // Clear the form
            contactForm.reset();
        });
    }

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });
});

// Function to load events data
function loadEvents() {
    const eventsContainer = document.getElementById('events-container');
    
    if (!eventsContainer) return;
    
    // Sample event data - in a real application this would come from an API or database
    const events = [
        {
            id: 1,
            title: 'Introduction to AI Workshop',
            description: 'Learn the basics of artificial intelligence and machine learning in this hands-on workshop.',
            date: 'April 15, 2025',
            time: '10:00 AM - 2:00 PM',
            location: 'Durban Tech Hub',
            image: 'https://images.unsplash.com/photo-1531482615713-2afd69097998?auto=format&fit=crop&w=800&q=80'
        },
        {
            id: 2,
            title: 'Data Science Community Day',
            description: 'Join us for a full day of talks, workshops, and networking focused on data science.',
            date: 'May 8, 2025',
            time: '9:00 AM - 5:00 PM',
            location: 'Durban Innovation Center',
            image: 'https://images.unsplash.com/photo-1551434678-e076c223a692?auto=format&fit=crop&w=800&q=80'
        },
        {
            id: 3,
            title: 'Machine Learning for Beginners',
            description: 'Get started with machine learning algorithms in this beginner-friendly session.',
            date: 'May 22, 2025',
            time: '6:00 PM - 8:00 PM',
            location: 'Online - Zoom Webinar',
            image: 'https://images.unsplash.com/photo-1535378917042-10a22c95931a?auto=format&fit=crop&w=800&q=80'
        },
        {
            id: 4,
            title: 'AI Ethics Panel Discussion',
            description: 'Join industry experts for a discussion on ethical considerations in artificial intelligence.',
            date: 'June 5, 2025',
            time: '3:00 PM - 5:00 PM',
            location: 'Durban University Auditorium',
            image: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=800&q=80'
        },
        {
            id: 5,
            title: 'Python for Data Analysis',
            description: 'Learn how to manipulate, analyze and visualize data using Python libraries.',
            date: 'June 19, 2025',
            time: '10:00 AM - 3:00 PM',
            location: 'Durban Tech Hub',
            image: 'https://images.unsplash.com/photo-1515879218367-8466d910aaa4?auto=format&fit=crop&w=800&q=80'
        },
        {
            id: 6,
            title: 'Deep Learning Symposium',
            description: 'An in-depth exploration of neural networks and deep learning applications.',
            date: 'July 10, 2025',
            time: '9:00 AM - 4:00 PM',
            location: 'Durban Convention Center',
            image: 'https://images.unsplash.com/photo-1620712943543-bcc4688e7485?auto=format&fit=crop&w=800&q=80'
        }
    ];
    
    // Clear loading message
    eventsContainer.innerHTML = '';
    
    // Create event cards and add to container
    events.forEach(event => {
        const eventCard = createEventCard(event);
        eventsContainer.appendChild(eventCard);
    });
}

// Function to create event card
function createEventCard(event) {
    const card = document.createElement('div');
    card.className = 'event-card';
    
    card.innerHTML = `
        <div class="event-image">
            <img src="${event.image}" alt="${event.title}">
        </div>
        <div class="event-details">
            <span class="event-date">${event.date} • ${event.time}</span>
            <h3 class="event-title">${event.title}</h3>
            <p class="event-description">${event.description}</p>
            <div class="event-action">
                <button class="event-register" data-event-id="${event.id}">Register</button>
                <span class="event-location"><i class="fas fa-map-marker-alt"></i> ${event.location}</span>
            </div>
        </div>
    `;
    
    // Add event listener for register button
    const registerBtn = card.querySelector('.event-register');
    registerBtn.addEventListener('click', function() {
        alert(`Thank you for registering for ${event.title}! We'll send details to your email soon.`);
    });
    
    return card;
}