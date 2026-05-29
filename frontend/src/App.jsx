import { useState, useEffect } from 'react';
import { Routes, Route, Navigate, useNavigate, useLocation } from 'react-router-dom';
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
import './styles/main.scss'


function App() {
  const location = useLocation()
  const navigate = useNavigate()
  const tabFromPath = location.pathname.replace('/', '') || 'habits'
  const [activeTab, setActiveTab] = useState(tabFromPath)
  
  // Получаем тему из store
  const { theme } = useThemeStore();

  // Применяем класс темы к body
  useEffect(() => {
    if (theme === 'dark') {
      document.body.classList.add('dark-theme');
      document.body.classList.remove('light-theme');
    } else {
      document.body.classList.add('light-theme');
      document.body.classList.remove('dark-theme');
    }
  }, [theme]);

  useEffect(() => {
    setActiveTab(tabFromPath)
  }, [tabFromPath])
  

  const handleTabChange = (tabId) => {
    setActiveTab(tabId)
    navigate(tabId || '/habits')
  }

  return (
    <div className="container">
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/habits" element={<ProtectedAuth><HabitsPage /></ProtectedAuth>} />
        <Route path="/habits/:id" element={<ProtectedAuth><HabitDetailPage /></ProtectedAuth> } />
        <Route path="/insights/personal" element={<ProtectedAuth><PersonalInsightsPage /></ProtectedAuth>} />
        <Route path="/habits/new" element={<ProtectedAuth><CreateHabitPage /></ProtectedAuth>} />
        <Route path="/insights/city" element={<ProtectedAuth><CityInsightsPage /></ProtectedAuth>} />
        <Route path="/profile" element={<ProtectedAuth><ProfilePage /></ProtectedAuth>} />
        <Route path="/" element={<Navigate to="/habits" replace />} />
      </Routes>
      {location.pathname !== '/login' && location.pathname !== '/register' && (<BottomNav activeTab={activeTab} onTabChange={handleTabChange}/>)}
    </div>
  );
}

export default App;