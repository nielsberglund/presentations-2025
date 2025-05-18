document.addEventListener('DOMContentLoaded', () => {
    const taskInput = document.getElementById('taskInput');
    const addButton = document.getElementById('addButton');
    const taskList = document.getElementById('taskList');

    // Load tasks from local storage
    loadTasks();

    addButton.addEventListener('click', addTask);
    taskInput.addEventListener('keypress', function(event) {
        if (event.key === 'Enter') {
            addTask();
        }
    });

    function addTask() {
        const taskText = taskInput.value.trim();
        if (taskText === '') {
            alert('Please enter a task.');
            return;
        }

        const listItem = createTaskElement(taskText);
        taskList.appendChild(listItem);
        saveTask(taskText);
        taskInput.value = '';
    }

    function createTaskElement(taskText) {
        const listItem = document.createElement('li');
        
        const taskSpan = document.createElement('span');
        taskSpan.textContent = taskText;

        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete';
        deleteButton.classList.add('delete-button');
        deleteButton.addEventListener('click', () => {
            taskList.removeChild(listItem);
            removeTask(taskText);
        });

        listItem.appendChild(taskSpan);
        listItem.appendChild(deleteButton);
        return listItem;
    }

    function saveTask(taskText) {
        let tasks = getTasksFromLocalStorage();
        tasks.push(taskText);
        localStorage.setItem('tasks', JSON.stringify(tasks));
    }

    function loadTasks() {
        let tasks = getTasksFromLocalStorage();
        tasks.forEach(taskText => {
            const listItem = createTaskElement(taskText);
            taskList.appendChild(listItem);
        });
    }

    function removeTask(taskText) {
        let tasks = getTasksFromLocalStorage();
        tasks = tasks.filter(task => task !== taskText);
        localStorage.setItem('tasks', JSON.stringify(tasks));
    }

    function getTasksFromLocalStorage() {
        const tasks = localStorage.getItem('tasks');
        return tasks ? JSON.parse(tasks) : [];
    }
});
