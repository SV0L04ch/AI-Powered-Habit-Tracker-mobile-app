import { useEffect, useMemo, useState } from 'react';
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage/LoginPage';
import RegisterPage from './pages/RegisterPage/RegisterPage';
import HabitsPage from './pages/HabitsPage/HabitsPage';
import HabitDetailPage from './pages/HabitDetailPage';
import CreateHabitPage from './pages/CreateHabitPage/CreateHabitPage';
import PersonalInsightsPage from './pages/PersonalInsightsPage/PersonalInsightsPage';
import CityInsightsPage from './pages/CityInsightsPage/CityInsightsPage';
import ProfilePage from './pages/ProfilePage/ProfilePage';
import BottomNav from './components/BottomNav/BottomNav';
import GuestRoute from './components/Guards/GuestRoute';
import ProtectedAuth from './components/Guards/ProtectedAuth';
import useThemeStore from './store/useThemeStore';
import './styles/main.scss';

function App() {
  const location = useLocation();
  const navigate = useNavigate();
  const hydrateTheme = useThemeStore((state) => state.hydrateTheme);
  const [showSplash, setShowSplash] = useState(() => !sessionStorage.getItem('pwa-splash-seen'));

  const activeTab = useMemo(() => {
    const currentPath = location.pathname.replace(/^\//, '') || 'habits';
    if (currentPath.startsWith('insights/personal')) return 'insights/personal';
    if (currentPath.startsWith('insights/city')) return 'insights/city';
    if (currentPath.startsWith('profile')) return 'profile';
    return 'habits';
  }, [location.pathname]);

  useEffect(() => {
    hydrateTheme();
  }, [hydrateTheme]);

  useEffect(() => {
    if (!showSplash) return;
    const timeout = window.setTimeout(() => {
      sessionStorage.setItem('pwa-splash-seen', 'true');
      setShowSplash(false);
    }, 1280);
    return () => window.clearTimeout(timeout);
  }, [showSplash]);

  const handleTabChange = (tabId) => {
    navigate(`/${tabId}`);
  };

  const isAuthRoute = location.pathname === '/login' || location.pathname === '/register';

  return (
    <div className="container">
      {showSplash && (
        <div className="splash-screen" data-testid="pwa-splash-screen">
          <div className="splash-card" data-testid="pwa-splash-card">
            <div className="splash-mark" />
          </div>
        </div>
      )}

      <div className="route-shell" key={location.pathname}>
        <Routes>
          <Route
            path="/login"
            element={
              <GuestRoute>
                <LoginPage />
              </GuestRoute>
            }
          />
          <Route
            path="/register"
            element={
              <GuestRoute>
                <RegisterPage />
              </GuestRoute>
            }
          />
          <Route
            path="/habits"
            element={
              <ProtectedAuth>
                <HabitsPage />
              </ProtectedAuth>
            }
          />
          <Route
            path="/habits/:id"
            element={
              <ProtectedAuth>
                <HabitDetailPage />
              </ProtectedAuth>
            }
          />
          <Route
            path="/insights/personal"
            element={
              <ProtectedAuth>
                <PersonalInsightsPage />
              </ProtectedAuth>
            }
          />
          <Route
            path="/habits/new"
            element={
              <ProtectedAuth>
                <CreateHabitPage />
              </ProtectedAuth>
            }
          />
          <Route
            path="/insights/city"
            element={
              <ProtectedAuth>
                <CityInsightsPage />
              </ProtectedAuth>
            }
          />
          <Route
            path="/profile"
            element={
              <ProtectedAuth>
                <ProfilePage />
              </ProtectedAuth>
            }
          />
          <Route path="/" element={<Navigate to="/habits" replace />} />
        </Routes>
      </div>

      {!isAuthRoute && <BottomNav activeTab={activeTab} onTabChange={handleTabChange} />}
    </div>
  );
}

export default App;
