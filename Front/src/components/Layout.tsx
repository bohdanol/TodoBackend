import Sidebar from './Sidebar.tsx';

export default function Layout({ children }) {
  return (
    <div style={{ display: 'flex', minHeight: '100vh' }}>
      <Sidebar />
      <div style={{
        marginLeft: '250px',
        flex: 1,
        padding: '20px',
        backgroundColor: '#ecf0f1',
        minHeight: '100vh'
      }}>
        {children}
      </div>
    </div>
  );
}
