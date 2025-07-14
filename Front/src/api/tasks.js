const API_BASE_URL = '/api/todo-list'; // Using proxy, no need for full URL

export const getTasks = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/task/all`);
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
