import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from '../components/Layout.tsx';
import MainPage from '../pages/MainPage.tsx';
import TasksPage from '../pages/TasksPage.tsx';

export default function AppRoutes() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<MainPage />} />
          <Route path="/tasks" element={<TasksPage />} />
          <Route path="/tasks/today" element={<TasksPage period="today" />} />
          <Route path="/tasks/tomorrow" element={<TasksPage period="tomorrow" />} />
          <Route path="/tasks/all" element={<TasksPage />} />
          <Route path="/tasks/this-week" element={<TasksPage period="this-week" />} />
          <Route path="/tasks/completed" element={<TasksPage period="completed" />} />
          <Route path="/tasks/uncompleted" element={<TasksPage period="uncompleted" />} />
        </Routes>
      </Layout>
    </Router>
  );
}
