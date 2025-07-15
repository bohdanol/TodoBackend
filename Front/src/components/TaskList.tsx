import { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { getTasks, createTask, updateTask } from '../api/tasks.ts';
import { Period } from '../helpers/Interfaces.ts';
import TaskModal from './TaskModal.tsx';

export default function TaskList({ period }: { period?: Period }) {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState(null);
  const [isEditMode, setIsEditMode] = useState(false);
  const location = useLocation();

  const fetchTasks = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Extract isCompleted from URL search params if present
      const searchParams = new URLSearchParams(location.search);
      const isCompletedParam = searchParams.get('isCompleted');
      
      let apiParams: any = { period };
      
      // Add isCompleted filter if it's in the URL
      if (isCompletedParam !== null) {
        apiParams.isCompleted = isCompletedParam === 'true';
      }
      
      const fetchedTasks = await getTasks(apiParams);
      setTasks(fetchedTasks);
    } catch (err) {
      console.error('Error fetching tasks:', err);
      setError(err.message || 'Failed to fetch tasks');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTask = async (taskData: any) => {
    if (isEditMode && editingTask) {
      await updateTask(editingTask.id, taskData, editingTask);
    } else {
      await createTask(taskData);
    }
    await fetchTasks(); // Refresh the task list
  };

  const handleModalClose = () => {
    setIsModalOpen(false);
    setIsEditMode(false);
    setEditingTask(null);
  };

  const handleAddNewTask = () => {
    setIsEditMode(false);
    setEditingTask(null);
    setIsModalOpen(true);
  };

  const handleEditTask = (task: any) => {
    setIsEditMode(true);
    setEditingTask(task);
    setIsModalOpen(true);
  };

  useEffect(() => {
    setTasks([]);
    const abortController = new AbortController();

    fetchTasks();

    return () => {
      abortController.abort();
    };
  }, [period, location.search]);

  if (loading) {
    return (
      <div style={{ backgroundColor: 'white', borderRadius: '8px', padding: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)', textAlign: 'center' }}>
        <p style={{ color: '#7f8c8d', fontSize: '1.1rem' }}>Loading tasks...</p>
      </div>
    );
  }

  if (error && tasks.length === 0) {
    return (
      <div style={{ backgroundColor: 'white', borderRadius: '8px', padding: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)', textAlign: 'center' }}>
        <p style={{ color: '#e74c3c', fontSize: '1.1rem' }}>
          Error loading tasks: {error}
        </p>
        <p style={{ color: '#7f8c8d', fontSize: '0.9rem' }}>
          Showing fallback data. Please check your backend connection.
        </p>
      </div>
    );
  }

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getPriorityColor = (priority) => {
    // Handle both string and numeric priority values
    const priorityValue = typeof priority === 'number' ? priority : priority;
    
    if (typeof priority === 'number') {
      switch (priority) {
        case 2: return '#e74c3c'; // High
        case 1: return '#f39c12'; // Medium
        case 0: return '#27ae60'; // Low
        default: return '#95a5a6';
      }
    } else {
      switch (priority) {
        case 'High': return '#e74c3c';
        case 'Medium': return '#f39c12';
        case 'Low': return '#27ae60';
        default: return '#95a5a6';
      }
    }
  };

  const getPriorityIcon = (priority) => {
    // Handle both string and numeric priority values
    if (typeof priority === 'number') {
      switch (priority) {
        case 2: return '🔴'; // High
        case 1: return '🟡'; // Medium
        case 0: return '�'; // Low
        default: return '⚪';
      }
    } else {
      switch (priority) {
        case 'High': return '�🔴';
        case 'Medium': return '🟡';
        case 'Low': return '🟢';
        default: return '⚪';
      }
    }
  };

  const getPriorityText = (priority) => {
    // Convert numeric priority to text for display
    if (typeof priority === 'number') {
      switch (priority) {
        case 2: return 'High';
        case 1: return 'Medium';
        case 0: return 'Low';
        default: return 'Unknown';
      }
    }
    return priority;
  };

  return (
    <>
      <div style={{ backgroundColor: 'white', borderRadius: '8px', padding: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
        {/* Add New Task Button */}
        <div style={{ marginBottom: '20px', display: 'flex', justifyContent: 'flex-end' }}>
          <button
            onClick={handleAddNewTask}
            style={{
              padding: '12px 24px',
              backgroundColor: '#3498db',
              color: 'white',
              border: 'none',
              borderRadius: '6px',
              fontSize: '14px',
              fontWeight: 'bold',
              cursor: 'pointer',
              transition: 'background-color 0.2s',
              display: 'flex',
              alignItems: 'center',
              gap: '8px'
            }}
            onMouseEnter={(e) => {
              (e.target as HTMLElement).style.backgroundColor = '#2980b9';
            }}
            onMouseLeave={(e) => {
              (e.target as HTMLElement).style.backgroundColor = '#3498db';
            }}
          >
            ➕ Add New Task
          </button>
        </div>

        {tasks.length === 0 ? (
          <p style={{ textAlign: 'center', color: '#7f8c8d', fontSize: '1.1rem' }}>
            No tasks found
          </p>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
            {tasks.map(task => (
              <div key={task.id} style={{
                padding: '16px',
                border: '1px solid #ecf0f1',
                borderRadius: '6px',
                backgroundColor: task.isCompleted ? '#f8f9fa' : 'white',
                transition: 'box-shadow 0.2s ease',
                cursor: 'pointer'
              }}
              onMouseEnter={(e) => (e.target as HTMLElement).style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)'}
              onMouseLeave={(e) => (e.target as HTMLElement).style.boxShadow = 'none'}
              >
                {/* Header with checkbox, title, and priority */}
                <div style={{ display: 'flex', alignItems: 'center', marginBottom: '8px' }}>
                  <input 
                    type="checkbox" 
                    checked={task.isCompleted}
                    style={{ marginRight: '12px', transform: 'scale(1.3)' }}
                    readOnly
                  />
                  <h3 
                    onClick={() => handleEditTask(task)}
                    style={{
                      margin: 0,
                      fontSize: '1.1rem',
                      fontWeight: '600',
                      textDecoration: task.isCompleted ? 'line-through' : 'none',
                      color: task.isCompleted ? '#7f8c8d' : '#2c3e50',
                      flex: 1,
                      cursor: 'pointer',
                      transition: 'color 0.2s'
                    }}
                    onMouseEnter={(e) => {
                      if (!task.isCompleted) {
                        (e.target as HTMLElement).style.color = '#3498db';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!task.isCompleted) {
                        (e.target as HTMLElement).style.color = '#2c3e50';
                      }
                    }}
                  >
                    {task.title}
                  </h3>
                  <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    fontSize: '0.9rem',
                    color: getPriorityColor(task.priority),
                    fontWeight: 'bold',
                    marginRight: '12px'
                  }}>
                    <span style={{ marginRight: '4px' }}>{getPriorityIcon(task.priority)}</span>
                    {getPriorityText(task.priority)}
                  </div>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      handleEditTask(task);
                    }}
                    style={{
                      background: 'none',
                      border: 'none',
                      cursor: 'pointer',
                      fontSize: '16px',
                      color: '#7f8c8d',
                      padding: '4px',
                      borderRadius: '3px',
                      transition: 'all 0.2s'
                    }}
                    onMouseEnter={(e) => {
                      (e.target as HTMLElement).style.backgroundColor = '#f8f9fa';
                      (e.target as HTMLElement).style.color = '#3498db';
                    }}
                    onMouseLeave={(e) => {
                      (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      (e.target as HTMLElement).style.color = '#7f8c8d';
                    }}
                    title="Edit task"
                  >
                    ✏️
                  </button>
                </div>

                {/* Description */}
                {task.description && (
                  <p style={{
                    margin: '0 0 12px 0',
                    fontSize: '0.95rem',
                    color: task.isCompleted ? '#95a5a6' : '#5a6c7d',
                    lineHeight: '1.4',
                    paddingLeft: '32px'
                  }}>
                    {task.description}
                  </p>
                )}

                {/* Footer with dates */}
                <div style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  fontSize: '0.85rem',
                  color: '#95a5a6',
                  paddingLeft: '32px'
                }}>
                  <div style={{ display: 'flex', gap: '16px' }}>
                    <span>
                      <strong>Due:</strong> {formatDate(task.dueDate)}
                    </span>
                    <span>
                      <strong>Created:</strong> {formatDate(task.createdAt)}
                    </span>
                    {task.updatedAt && (
                      <span>
                        <strong>Updated:</strong> {formatDate(task.updatedAt)}
                      </span>
                    )}
                  </div>
                  <span style={{ fontSize: '0.8rem' }}>
                    ID: {task.id}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Task Modal */}
      <TaskModal
        isOpen={isModalOpen}
        onClose={handleModalClose}
        onSave={handleCreateTask}
        editTask={editingTask}
        isEditMode={isEditMode}
      />
    </>
  );
}
