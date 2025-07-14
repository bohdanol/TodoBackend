import { useState, useEffect } from 'react';
import { getTasks } from '../api/tasks';

export default function TaskList({ filter = 'all' }) {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Fetch tasks from API
  useEffect(() => {
    console.log('🔄 TaskList useEffect triggered'); // Debug log
    let isMounted = true; // Flag to prevent state updates if component unmounted
    const abortController = new AbortController(); // For cancelling requests

    const fetchTasks = async () => {
      try {
        console.log('📡 Starting API request...'); // Debug log
        setLoading(true);
        setError(null);
        const fetchedTasks = await getTasks(abortController.signal);
        console.log('✅ API request successful:', fetchedTasks?.length, 'tasks'); // Debug log
        
        // Only update state if component is still mounted
        if (isMounted) {
          setTasks(fetchedTasks);
        }
      } catch (err) {
        // Ignore cancelled requests
        if (err.message === 'Request cancelled' || err.name === 'AbortError') {
          console.log('❌ Request was cancelled/aborted'); // Debug log
          return;
        }
        
        console.error('Failed to fetch tasks:', err);
        // Better error handling with Axios
        const errorMessage = err.response?.data?.message || 
                            err.response?.statusText || 
                            err.message || 
                            'Failed to fetch tasks';
        
        // Only update state if component is still mounted
        if (isMounted) {
          setError(errorMessage);
          // Fallback to mock data if API fails
          setTasks(mockTasks);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchTasks();

    // Cleanup function to prevent memory leaks and cancel ongoing requests
    return () => {
      console.log('🧹 TaskList cleanup - cancelling request'); // Debug log
      isMounted = false;
      abortController.abort(); // Cancel any ongoing request
    };
  }, []);

  // Mock tasks data as fallback
  const mockTasks = [
    {
      id: 1,
      title: 'Complete project proposal',
      description: 'Finalize the project proposal document and submit to management',
      isCompleted: false,
      dueDate: '2025-07-14T17:00:00',
      createdAt: '2025-07-10T09:00:00',
      updatedAt: '2025-07-12T14:30:00',
      priority: 'High',
      subTasks: []
    },
    {
      id: 2,
      title: 'Review code changes',
      description: 'Review pull requests and provide feedback to team members',
      isCompleted: true,
      dueDate: '2025-07-13T12:00:00',
      createdAt: '2025-07-11T10:15:00',
      updatedAt: '2025-07-13T11:45:00',
      priority: 'Medium',
      subTasks: []
    },
    {
      id: 3,
      title: 'Team meeting at 3 PM',
      description: 'Weekly standup meeting to discuss project progress',
      isCompleted: false,
      dueDate: '2025-07-15T15:00:00',
      createdAt: '2025-07-14T08:00:00',
      updatedAt: null,
      priority: 'Low',
      subTasks: []
    },
    {
      id: 4,
      title: 'Update documentation',
      description: 'Update API documentation with latest changes',
      isCompleted: false,
      dueDate: '2025-07-16T18:00:00',
      createdAt: '2025-07-13T16:20:00',
      updatedAt: null,
      priority: 'Medium',
      subTasks: []
    }
  ];

  const getFilteredTasks = () => {
    const today = new Date().toISOString().split('T')[0];
    const tomorrow = new Date(Date.now() + 86400000).toISOString().split('T')[0];
    
    switch (filter) {
      case 'today':
        return tasks.filter(task => task.dueDate.split('T')[0] === today);
      case 'tomorrow':
        return tasks.filter(task => task.dueDate.split('T')[0] === tomorrow);
      case 'this-week':
        return tasks.filter(task => {
          const taskDate = new Date(task.dueDate);
          const now = new Date();
          const weekFromNow = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000);
          return taskDate >= now && taskDate <= weekFromNow;
        });
      case 'done':
        return tasks.filter(task => task.isCompleted);
      case 'all':
      default:
        return tasks;
    }
  };

  const filteredTasks = getFilteredTasks();

  // Loading state
  if (loading) {
    return (
      <div style={{ backgroundColor: 'white', borderRadius: '8px', padding: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)', textAlign: 'center' }}>
        <p style={{ color: '#7f8c8d', fontSize: '1.1rem' }}>Loading tasks...</p>
      </div>
    );
  }

  // Error state
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
    switch (priority) {
      case 'High': return '#e74c3c';
      case 'Medium': return '#f39c12';
      case 'Low': return '#27ae60';
      default: return '#95a5a6';
    }
  };

  const getPriorityIcon = (priority) => {
    switch (priority) {
      case 'High': return '🔴';
      case 'Medium': return '🟡';
      case 'Low': return '🟢';
      default: return '⚪';
    }
  };

  return (
    <div style={{ backgroundColor: 'white', borderRadius: '8px', padding: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
      {filteredTasks.length === 0 ? (
        <p style={{ textAlign: 'center', color: '#7f8c8d', fontSize: '1.1rem' }}>
          No tasks found for {filter === 'all' ? 'this filter' : filter}
        </p>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
          {filteredTasks.map(task => (
            <div key={task.id} style={{
              padding: '16px',
              border: '1px solid #ecf0f1',
              borderRadius: '6px',
              backgroundColor: task.isCompleted ? '#f8f9fa' : 'white',
              transition: 'box-shadow 0.2s ease',
              cursor: 'pointer'
            }}
            onMouseEnter={(e) => e.target.style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)'}
            onMouseLeave={(e) => e.target.style.boxShadow = 'none'}
            >
              {/* Header with checkbox, title, and priority */}
              <div style={{ display: 'flex', alignItems: 'center', marginBottom: '8px' }}>
                <input 
                  type="checkbox" 
                  checked={task.isCompleted}
                  style={{ marginRight: '12px', transform: 'scale(1.3)' }}
                  readOnly
                />
                <h3 style={{
                  margin: 0,
                  fontSize: '1.1rem',
                  fontWeight: '600',
                  textDecoration: task.isCompleted ? 'line-through' : 'none',
                  color: task.isCompleted ? '#7f8c8d' : '#2c3e50',
                  flex: 1
                }}>
                  {task.title}
                </h3>
                <div style={{
                  display: 'flex',
                  alignItems: 'center',
                  fontSize: '0.9rem',
                  color: getPriorityColor(task.priority),
                  fontWeight: 'bold'
                }}>
                  <span style={{ marginRight: '4px' }}>{getPriorityIcon(task.priority)}</span>
                  {task.priority}
                </div>
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
  );
}
