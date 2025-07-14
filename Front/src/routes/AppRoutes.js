import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from '../components/Layout';
import MainPage from '../pages/MainPage';
import TasksPage from '../pages/TasksPage';

export default function AppRoutes() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<MainPage />} />
          <Route path="/tasks" element={<TasksPage />} />
          <Route path="/tasks/today" element={<TasksPage filter="today" />} />
          <Route path="/tasks/tomorrow" element={<TasksPage filter="tomorrow" />} />
          <Route path="/tasks/all" element={<TasksPage filter="all" />} />
          <Route path="/tasks/this-week" element={<TasksPage filter="this-week" />} />
          <Route path="/tasks/done" element={<TasksPage filter="done" />} />
        </Routes>
      </Layout>
    </Router>
  );
}
