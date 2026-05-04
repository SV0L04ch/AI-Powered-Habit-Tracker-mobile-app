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
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/habits" element={<HabitsPage />} />
        <Route path="/habits/:id" element={<HabitDetailPage />} />
        <Route path="/insights/personal" element={<PersonalInsightsPage />} />
        <Route path="/habits/new" element={<CreateHabitPage />} />
        <Route path="/insights/city" element={<CityInsightsPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/" element={<Navigate to="/habits" replace />} />
      </Routes>
      <BottomNav activeTab={activeTab} onTabChange={handleTabChange}/>
    </div>
  );
}

export default App;