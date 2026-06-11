import { useState, useEffect } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import Sidebar from '../Sidebar/Sidebar';
import TopBar from '../TopBar/TopBar';
import BottomNav from '../../BottomNav/BottomNav';
import styles from './AppLayout.module.scss';

export default function AppLayout() {
  const [sidebarExpanded, setSidebarExpanded] = useState(true);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const navigate = useNavigate();

  useEffect(() => {
    const handleResize = () => setIsMobile(window.innerWidth < 768);
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return (
    <div className={styles.layout}>
      <Sidebar expanded={sidebarExpanded} onToggle={() => setSidebarExpanded(!sidebarExpanded)} />
      <div className={styles.main} data-sidebar-expanded={sidebarExpanded}>
        <TopBar onToggleSidebar={() => setSidebarExpanded(!sidebarExpanded)} />
        <div className={styles.content}>
          <Outlet />
        </div>
      </div>
      {isMobile && <BottomNav activeTab="" onTabChange={(id) => navigate('/' + id)} />}
    </div>
  );
}
