import Tabs from '../components/Tabs';
import TaskList from '../components/TaskList';

export default function TasksPage({ filter = 'all' }) {
  const getPageTitle = () => {
    switch (filter) {
      case 'today': return 'Today\'s Tasks';
      case 'tomorrow': return 'Tomorrow\'s Tasks';
      case 'this-week': return 'This Week\'s Tasks';
      case 'done': return 'Done Tasks';
      case 'all':
      default: return 'All Tasks';
    }
  };

  return (
    <div>
      <h1>{getPageTitle()}</h1>
      <Tabs activeFilter={filter} />
      <TaskList filter={filter} />
    </div>
  );
}
