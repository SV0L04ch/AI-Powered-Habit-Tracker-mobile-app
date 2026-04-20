import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HabitsPage from './pages/HabitsPage';
import HabitDetailPage from './pages/HabitDetailPage';
import CreateHabitPage from './pages/CreateHabitPage';
import PersonalInsightsPage from './pages/PersonalInsightsPage';
import CityInsightsPage from './pages/CityInsightsPage';
import ProfilePage from './pages/ProfilePage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/habits" element={<HabitsPage />} />
        <Route path="/habits/new" element={<CreateHabitPage />} />
        <Route path="/habits/:id" element={<HabitDetailPage />} />
        <Route path="/insights/personal" element={<PersonalInsightsPage />} />
        <Route path="/insights/city" element={<CityInsightsPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/" element={<Navigate to="/habits" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;