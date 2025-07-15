import Clock from '../components/Clock.tsx';

export default function MainPage() {
  return (
    <div style={{ textAlign: 'center' }}>
      <h1>Welcome to TodoList</h1>
      <p style={{ fontSize: '1.1rem', color: '#7f8c8d', marginBottom: '40px' }}>
        Manage your tasks efficiently and stay organized
      </p>
      <Clock />
    </div>
  );
}
