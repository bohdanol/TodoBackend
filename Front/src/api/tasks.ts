import { Period } from "../helpers/Interfaces";

const API_BASE_URL = '/api/todo-list'; // Using proxy, no need for full URL

export const getTasks = async ({period, isCompleted}: {period?: Period, isCompleted?: boolean}) => {
  try {
    let url = `${API_BASE_URL}/task/all`;
    
    // Add period to path if it's not 'completed' related
    if (period && period !== 'completed') {
      url += `/${period}`;
    }
    
    // Add query parameters
    const queryParams = new URLSearchParams();
    
    // Add isCompleted parameter if provided
    if (isCompleted !== undefined) {
      queryParams.append('isCompleted', isCompleted.toString());
    }
    
    // If we have query parameters, append them to the URL
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
