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
import './styles/main.scss'


function App() {
  const location = useLocation()
  const navigate = useNavigate()
  const tabFromPath = location.pathname.replace('/', '') || 'habits'
  const [activeTab, setActiveTab] = useState(tabFromPath)

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
        <Route path="/login" element={<GuestRoute><LoginPage /></GuestRoute>} />
        <Route path="/register" element={<GuestRoute><RegisterPage /></GuestRoute>} />
        <Route path="/habits" element={<ProtectedAuth><HabitsPage /></ProtectedAuth>} />
        <Route path="/habits/:id" element={<ProtectedAuth><HabitDetailPage /></ProtectedAuth> } />
        <Route path="/insights/personal" element={<ProtectedAuth><PersonalInsightsPage /></ProtectedAuth>} />
        <Route path="/habits/new" element={<ProtectedAuth><CreateHabitPage /></ProtectedAuth>} />
        <Route path="/insights/city" element={<ProtectedAuth><CityInsightsPage /></ProtectedAuth>} />
        <Route path="/profile" element={<ProtectedAuth><ProfilePage /></ProtectedAuth>} />
        <Route path="/" element={<Navigate to="/habits" replace />} />
      </Routes>
      <BottomNav activeTab={activeTab} onTabChange={handleTabChange}/>
    </div>
  );
}

export default App;