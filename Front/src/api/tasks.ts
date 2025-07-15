import { Period } from "../helpers/Interfaces";

const API_BASE_URL = '/api/todo-list'; 
export const getTasks = async ({period, isCompleted}: {period?: Period, isCompleted?: boolean}) => {
  try {
    let url = `${API_BASE_URL}/task/all`;
    
    if (period && period !== 'completed') {
      url += `/${period}`;
    }    
    const queryParams = new URLSearchParams();

    if (isCompleted !== undefined) {
      queryParams.append('isCompleted', isCompleted.toString());
    }
    
    if (queryParams.toString()) {
      url += `?${queryParams.toString()}`;
    }
    
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const tasks = await response.json();
    return tasks;
  } catch (error) {
    console.error('Error fetching tasks:', error);
    throw error;
  }
};

export const createTask = async (taskData: {
  title: string;
  description?: string;
  priority: string;
  dueDate: string;
}) => {
  try {
    // Convert priority string to number
    const priorityMap = {
      'Low': 0,
      'Medium': 1,
      'High': 2
    };

    const now = new Date().toISOString();
    
    // Debug: Log the priority mapping
    console.log('Original priority:', taskData.priority);
    console.log('Mapped priority:', priorityMap[taskData.priority]);
    
    const requestBody = {
      title: taskData.title,
      description: taskData.description || '',
      createdAt: now,
      updatedAt: now,
      dueDate: new Date(taskData.dueDate).toISOString(),
      priority: priorityMap[taskData.priority] !== undefined ? priorityMap[taskData.priority] : 1,
      isCompleted: false
    };
    
    console.log('Request body:', requestBody);
    
    const response = await fetch(`${API_BASE_URL}/task`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const newTask = await response.json();
    return newTask;
  } catch (error) {
    console.error('Error creating task:', error);
    throw error;
  }
};

export const updateTask = async (taskId: string, taskData: {
  title: string;
  description?: string;
  priority: string;
  dueDate: string;
}, originalTask: any) => {
  try {
    // Convert priority string to number
    const priorityMap = {
      'Low': 0,
      'Medium': 1,
      'High': 2
    };

    const now = new Date().toISOString();
    
    // Debug: Log the priority mapping
    console.log('Update - Original priority:', taskData.priority);
    console.log('Update - Mapped priority:', priorityMap[taskData.priority]);
    
    const requestBody = {
      id: parseInt(taskId),
      title: taskData.title,
      description: taskData.description || '',
      createdAt: originalTask.createdAt, // Keep original creation date
      updatedAt: now,
      dueDate: new Date(taskData.dueDate).toISOString(),
      priority: priorityMap[taskData.priority] !== undefined ? priorityMap[taskData.priority] : 1,
      isCompleted: originalTask.isCompleted || false // Keep original completion status
    };
    
    console.log('Update request body:', requestBody);
    
    const response = await fetch(`${API_BASE_URL}/task/${taskId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const updatedTask = await response.json();
    return updatedTask;
  } catch (error) {
    console.error('Error updating task:', error);
    throw error;
  }
};
