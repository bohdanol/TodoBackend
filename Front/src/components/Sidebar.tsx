import { Link, useLocation } from 'react-router-dom';
import { useState, useEffect } from 'react';

export default function Sidebar() {
  const location = useLocation();
  const [isTasksExpanded, setIsTasksExpanded] = useState(false);

  useEffect(() => {
    if (location.pathname.startsWith('/tasks')) {
      setIsTasksExpanded(true);
    }
  }, [location.pathname]);

  const toggleTasks = () => {
    setIsTasksExpanded(!isTasksExpanded);
  };

  const isActive = (path) => {
    if (path === '/' && location.pathname === '/') return true;
    if (path === '/tasks' && location.pathname === '/tasks') return true;
    
    // For paths with query parameters, check both pathname and search
    if (path.includes('?')) {
      const [pathPathname, pathSearch] = path.split('?');
      return location.pathname === pathPathname && location.search === `?${pathSearch}`;
    }
    
    // For paths without query parameters, just check pathname
    return location.pathname === path;
  };

  const isAnyTaskActive = () => {
    return location.pathname.startsWith('/tasks');
  };

  return (
    <div style={{
      width: '250px',
      height: '100vh',
      backgroundColor: '#2c3e50',
      color: 'white',
      padding: '20px',
      boxSizing: 'border-box',
      position: 'fixed',
      left: 0,
      top: 0,
      overflowY: 'auto'
    }}>
      <div style={{
        fontSize: '1.5rem',
        fontWeight: 'bold',
        marginBottom: '30px',
        textAlign: 'left',
        borderBottom: '2px solid #34495e',
        paddingBottom: '15px'
      }}>
        TodoList
      </div>

      <nav>
        <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
          <li style={{ marginBottom: '10px' }}>
            <Link 
              to="/" 
              style={{
                color: 'white',
                textDecoration: 'none',
                display: 'block',
                padding: '10px 15px',
                borderRadius: '5px',
                transition: 'background-color 0.3s',
                backgroundColor: isActive('/') ? '#3498db' : 'transparent',
                textAlign: 'left'
              }}
              onMouseEnter={(e) => {
                if (!isActive('/')) {
                  (e.target as HTMLElement).style.backgroundColor = '#34495e';
                }
              }}
              onMouseLeave={(e) => {
                if (!isActive('/')) {
                  (e.target as HTMLElement).style.backgroundColor = 'transparent';
                }
              }}
            >
              🏠 Main Page
            </Link>
          </li>

          <li>
            <div 
              onClick={toggleTasks}
              style={{
                color: 'white',
                textDecoration: 'none',
                display: 'block',
                padding: '10px 15px',
                borderRadius: '5px',
                cursor: 'pointer',
                transition: 'background-color 0.3s',
                backgroundColor: isAnyTaskActive() ? '#3498db' : (isTasksExpanded ? '#34495e' : 'transparent'),
                textAlign: 'left'
              }}
              onMouseEnter={(e) => {
                if (!isAnyTaskActive()) {
                  (e.target as HTMLElement).style.backgroundColor = '#34495e';
                }
              }}
              onMouseLeave={(e) => {
                if (!isAnyTaskActive()) {
                  (e.target as HTMLElement).style.backgroundColor = isTasksExpanded ? '#34495e' : 'transparent';
                }
              }}
            >
              📋 Tasks {isTasksExpanded ? '▼' : '▶'}
            </div>
            
            {isTasksExpanded && (
              <ul style={{ 
                listStyle: 'none', 
                padding: 0, 
                margin: '5px 0 10px 20px',
                paddingLeft: '10px'
              }}>
                <li style={{ marginBottom: '5px' }}>
                  <Link 
                    to="/tasks/today" 
                    style={{
                      color: isActive('/tasks/today') ? '#3498db' : '#bdc3c7',
                      textDecoration: 'none',
                      display: 'block',
                      padding: '8px 10px',
                      borderRadius: '3px',
                      fontSize: '0.9rem',
                      transition: 'background-color 0.3s',
                      backgroundColor: isActive('/tasks/today') ? '#34495e' : 'transparent',
                      fontWeight: isActive('/tasks/today') ? 'bold' : 'normal',
                      textAlign: 'left'
                    }}
                    onMouseEnter={(e) => {
                      if (!isActive('/tasks/today')) {
                        (e.target as HTMLElement).style.backgroundColor = '#34495e';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!isActive('/tasks/today')) {
                        (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      }
                    }}
                  >
                    📅 Today
                  </Link>
                </li>
                <li style={{ marginBottom: '5px' }}>
                  <Link 
                    to="/tasks/tomorrow" 
                    style={{
                      color: isActive('/tasks/tomorrow') ? '#3498db' : '#bdc3c7',
                      textDecoration: 'none',
                      display: 'block',
                      padding: '8px 10px',
                      borderRadius: '3px',
                      fontSize: '0.9rem',
                      transition: 'background-color 0.3s',
                      backgroundColor: isActive('/tasks/tomorrow') ? '#34495e' : 'transparent',
                      fontWeight: isActive('/tasks/tomorrow') ? 'bold' : 'normal',
                      textAlign: 'left'
                    }}
                    onMouseEnter={(e) => {
                      if (!isActive('/tasks/tomorrow')) {
                        (e.target as HTMLElement).style.backgroundColor = '#34495e';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!isActive('/tasks/tomorrow')) {
                        (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      }
                    }}
                  >
                    🌅 Tomorrow
                  </Link>
                </li>
                <li style={{ marginBottom: '5px' }}>
                  <Link 
                    to="/tasks/this-week" 
                    style={{
                      color: isActive('/tasks/this-week') ? '#3498db' : '#bdc3c7',
                      textDecoration: 'none',
                      display: 'block',
                      padding: '8px 10px',
                      borderRadius: '3px',
                      fontSize: '0.9rem',
                      transition: 'background-color 0.3s',
                      backgroundColor: isActive('/tasks/this-week') ? '#34495e' : 'transparent',
                      fontWeight: isActive('/tasks/this-week') ? 'bold' : 'normal',
                      textAlign: 'left'
                    }}
                    onMouseEnter={(e) => {
                      if (!isActive('/tasks/this-week')) {
                        (e.target as HTMLElement).style.backgroundColor = '#34495e';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!isActive('/tasks/this-week')) {
                        (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      }
                    }}
                  >
                    📆 This Week
                  </Link>
                </li>
                                <li style={{ marginBottom: '5px' }}>
                  <Link 
                    to="/tasks/all" 
                    style={{
                      color: isActive('/tasks/all') || (location.pathname === '/tasks') ? '#3498db' : '#bdc3c7',
                      textDecoration: 'none',
                      display: 'block',
                      padding: '8px 10px',
                      borderRadius: '3px',
                      fontSize: '0.9rem',
                      transition: 'background-color 0.3s',
                      backgroundColor: isActive('/tasks/all') || (location.pathname === '/tasks') ? '#34495e' : 'transparent',
                      fontWeight: isActive('/tasks/all') || (location.pathname === '/tasks') ? 'bold' : 'normal',
                      textAlign: 'left'
                    }}
                    onMouseEnter={(e) => {
                      if (!isActive('/tasks/all') && location.pathname !== '/tasks') {
                        (e.target as HTMLElement).style.backgroundColor = '#34495e';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!isActive('/tasks/all') && location.pathname !== '/tasks') {
                        (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      }
                    }}
                  >
                    📝 All
                  </Link>
                </li>
                <li style={{ marginBottom: '5px' }}>
                  <Link 
                    to="/tasks/completed?isCompleted=true" 
                    style={{
                      color: isActive('/tasks/completed?isCompleted=true') ? '#3498db' : '#bdc3c7',
                      textDecoration: 'none',
                      display: 'block',
                      padding: '8px 10px',
                      borderRadius: '3px',
                      fontSize: '0.9rem',
                      transition: 'background-color 0.3s',
                      backgroundColor: isActive('/tasks/completed?isCompleted=true') ? '#34495e' : 'transparent',
                      fontWeight: isActive('/tasks/completed?isCompleted=true') ? 'bold' : 'normal',
                      textAlign: 'left'
                    }}
                    onMouseEnter={(e) => {
                      if (!isActive('/tasks/completed?isCompleted=true')) {
                        (e.target as HTMLElement).style.backgroundColor = '#34495e';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!isActive('/tasks/completed?isCompleted=true')) {
                          (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      }
                    }}
                  >
                    ⏰ Completed
                  </Link>
                </li>
                <li style={{ marginBottom: '5px' }}>
                  <Link 
                    to="/tasks/completed?isCompleted=false" 
                    style={{
                      color: isActive('/tasks/completed?isCompleted=false') ? '#3498db' : '#bdc3c7',
                      textDecoration: 'none',
                      display: 'block',
                      padding: '8px 10px',
                      borderRadius: '3px',
                      fontSize: '0.9rem',
                      transition: 'background-color 0.3s',
                      backgroundColor: isActive('/tasks/completed?isCompleted=false') ? '#34495e' : 'transparent',
                      fontWeight: isActive('/tasks/completed?isCompleted=false') ? 'bold' : 'normal',
                      textAlign: 'left'
                    }}
                    onMouseEnter={(e) => {
                      if (!isActive('/tasks/completed?isCompleted=false')) {
                        (e.target as HTMLElement).style.backgroundColor = '#34495e';
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (!isActive('/tasks/completed?isCompleted=false')) {
                        (e.target as HTMLElement).style.backgroundColor = 'transparent';
                      }
                    }}
                  >
                    ❌ Uncompleted
                  </Link>
                </li>
              </ul>
            )}
          </li>
        </ul>
      </nav>
    </div>
  );
}
