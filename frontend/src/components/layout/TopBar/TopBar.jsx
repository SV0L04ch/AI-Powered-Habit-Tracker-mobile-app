import { useNavigate, useLocation } from 'react-router-dom';
import useThemeStore from '../../../store/useThemeStore';
import useAuthStore from '../../../store/useAuthStore';
import styles from './TopBar.module.scss';

export default function TopBar({ onToggleSidebar }) {
  const navigate = useNavigate();
  const location = useLocation();
  const { theme, toggleTheme } = useThemeStore();
  const profile = useAuthStore((s) => s.profile);

  const getBreadcrumb = () => {
    const path = location.pathname;
    if (path === '/dashboard') return 'Dashboard';
    if (path === '/habits') return 'Habits';
    if (path === '/habits/new') return 'New Habit';
    if (path.startsWith('/habits/')) return 'Habit Detail';
    if (path === '/schedule') return 'Schedule';
    if (path === '/templates') return 'Templates';
    if (path === '/insights/personal') return 'Personal Insights';
    if (path === '/insights/city') return 'City Insights';
    if (path === '/social/feed') return 'Social Feed';
    if (path === '/social/friends') return 'Friends';
    if (path === '/social/challenges') return 'Challenges';
    if (path === '/gamification') return 'Gamification';
    if (path === '/journal') return 'Journal';
    if (path === '/economics') return 'Economics';
    if (path === '/webhooks') return 'Webhooks';
    if (path === '/profile') return 'Profile';
    return '';
  };

  return (
    <header className={styles.topbar}>
      <div className={styles.left}>
        <button className={styles.menuBtn} onClick={onToggleSidebar}>
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <line x1="3" y1="12" x2="21" y2="12"/>
            <line x1="3" y1="6" x2="21" y2="6"/>
            <line x1="3" y1="18" x2="21" y2="18"/>
          </svg>
        </button>
        <div className={styles.breadcrumb}>{getBreadcrumb()}</div>
      </div>
      <div className={styles.right}>
        <button className={styles.iconBtn} onClick={() => alert('Search coming soon!')} title="Search (Ctrl+K)">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/>
          </svg>
        </button>
        <button className={styles.iconBtn} onClick={toggleTheme} title="Toggle theme">
          {theme === 'dark'
            ? <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="4"/><path d="M12 2v2"/><path d="M12 20v2"/><path d="m4.93 4.93 1.41 1.41"/><path d="m17.66 17.66 1.41 1.41"/><path d="M2 12h2"/><path d="M20 12h2"/><path d="m6.34 17.66-1.41 1.41"/><path d="m19.07 4.93-1.41 1.41"/></svg>
            : <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z"/></svg>
          }
        </button>
        <div className={styles.avatar} onClick={() => navigate('/profile')}>
          {profile?.name?.[0]?.toUpperCase() || profile?.email?.[0]?.toUpperCase() || 'U'}
        </div>
      </div>
    </header>
  );
}
