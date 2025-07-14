import { useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { getTasks } from '../api/tasks';

export default function Tabs({ activeFilter = 'all' }) {
  const location = useLocation();
  
  const tabs = [
    { key: 'today', label: 'Today', path: '/tasks/today' },
    { key: 'tomorrow', label: 'Tomorrow', path: '/tasks/tomorrow' },
    { key: 'this-week', label: 'This Week', path: '/tasks/this-week' },
    { key: 'all', label: 'All', path: '/tasks/all' },
    { key: 'done', label: 'Done', path: '/tasks/done' }
  ];

  useEffect(() => {
    getTasks().then(tasks => {
      console.log('Fetched tasks:', tasks);
    });
  }, []);


  const isActive = (tabPath) => {
    return location.pathname === tabPath || 
           (location.pathname === '/tasks' && tabPath === '/tasks/all');
  };

  return (
    <div style={{
      display: 'flex',
      borderBottom: '2px solid #ecf0f1',
      marginBottom: '20px',
      flexWrap: 'wrap'
    }}>
      {tabs.map((tab) => (
        <Link
          key={tab.key}
          to={tab.path}
          style={{
            padding: '12px 20px',
            textDecoration: 'none',
            color: isActive(tab.path) ? '#3498db' : '#7f8c8d',
            fontWeight: isActive(tab.path) ? 'bold' : 'normal',
            borderBottom: isActive(tab.path) ? '3px solid #3498db' : '3px solid transparent',
            transition: 'all 0.3s ease',
            cursor: 'pointer'
          }}
          onMouseEnter={(e) => {
            if (!isActive(tab.path)) {
              e.target.style.color = '#2c3e50';
            }
          }}
          onMouseLeave={(e) => {
            if (!isActive(tab.path)) {
              e.target.style.color = '#7f8c8d';
            }
          }}
        >
          {tab.label}
        </Link>
      ))}
    </div>
  );
}
