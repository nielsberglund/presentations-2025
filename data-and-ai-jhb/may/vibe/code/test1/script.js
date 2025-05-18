document.addEventListener('DOMContentLoaded', function() {
    const taskInput = document.getElementById('taskInput');
    const addButton = document.getElementById('addButton');
    const taskList = document.getElementById('taskList');
    const emptyListItem = document.querySelector('.empty-list');
    
    // Load tasks from localStorage
    function loadTasks() {
        // Clear current list except for the empty message
        while (taskList.firstChild && !taskList.firstChild.classList.contains('empty-list')) {
            taskList.removeChild(taskList.firstChild);
        }
        
        // Get tasks from localStorage
        const tasks = JSON.parse(localStorage.getItem('tasks')) || [];
        
        if (tasks.length > 0) {
            // Remove empty list message if it exists
            if (emptyListItem && emptyListItem.parentNode === taskList) {
                taskList.removeChild(emptyListItem);
            }
            
            // Add each task to the list
            tasks.forEach(taskText => {
                createTaskElement(taskText);
            });
        } else if (!taskList.querySelector('.empty-list')) {
            // Show empty list message if no tasks and it's not already there
            taskList.appendChild(emptyListItem);
        }
    }
    
    // Save tasks to localStorage
    function saveTasks() {
        const tasks = [];
        // Get all tasks from the list items (excluding the empty-list message)
        const taskElements = taskList.querySelectorAll('li:not(.empty-list)');
        
        taskElements.forEach(taskElement => {
            const taskText = taskElement.querySelector('span').textContent;
            tasks.push(taskText);
        });
        
        localStorage.setItem('tasks', JSON.stringify(tasks));
    }
    
    // Function to create a task element
    function createTaskElement(taskText) {
        // Create list item
        const li = document.createElement('li');
        
        // Create task text span
        const taskSpan = document.createElement('span');
        taskSpan.textContent = taskText;
        
        // Create delete button
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete';
        deleteButton.className = 'delete-btn';
        deleteButton.addEventListener('click', function() {
            taskList.removeChild(li);
            
            // Show empty list message if no tasks remain
            if (taskList.children.length === 0) {
                taskList.appendChild(emptyListItem);
            }
            
            // Update localStorage
            saveTasks();
        });
        
        // Add elements to list item
        li.appendChild(taskSpan);
        li.appendChild(deleteButton);
        
        // Add list item to task list
        taskList.appendChild(li);
    }
    
    // Function to add a new task
    function addTask() {
        const taskText = taskInput.value.trim();
        
        if (taskText === '') {
            alert('Please enter a task!');
            return;
        }
        
        // Remove empty list message if it exists
        if (emptyListItem && emptyListItem.parentNode === taskList) {
            taskList.removeChild(emptyListItem);
        }
        
        // Create and add the task element
        createTaskElement(taskText);
        
        // Save to localStorage
        saveTasks();
        
        // Clear the input
        taskInput.value = '';
        taskInput.focus();
    }
    
    // Add task when button is clicked
    addButton.addEventListener('click', addTask);
    
    // Add task when Enter key is pressed
    taskInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            addTask();
        }
    });
    
    // Load tasks when the page loads
    loadTasks();
});