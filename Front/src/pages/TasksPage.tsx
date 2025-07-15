import { useEffect, useState } from 'react';
import Tabs from '../components/Tabs.tsx';
import TaskList from '../components/TaskList.tsx';
import { Period } from '../helpers/Interfaces.ts';

export default function TasksPage({ period }: { period?: Period }) {
  const getPageTitle = () => {
    switch (period) {
      case 'today': return 'Today\'s Tasks';
      case 'tomorrow': return 'Tomorrow\'s Tasks';
      case 'this-week': return 'This Week\'s Tasks';
      case 'completed': return 'Completed Tasks';
      case 'uncompleted': return 'Uncompleted Tasks';
      default: return 'All Tasks';
    }
  };

  return (
    <div>
      <h1>{getPageTitle()}</h1>
      <Tabs activeFilter={period} />
      <TaskList period={period} />
    </div>
  );
}
