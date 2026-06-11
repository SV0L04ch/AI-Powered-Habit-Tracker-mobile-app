import { useEffect, useState, Suspense, lazy } from 'react';
import { Route, Routes, useLocation } from 'react-router-dom';
import { AnimatePresence } from 'framer-motion';
import AppLayout from './components/layout/AppLayout/AppLayout';
import LandingLayout from './components/layout/LandingLayout/LandingLayout';
import GuestRoute from './components/Guards/GuestRoute';
import ProtectedAuth from './components/Guards/ProtectedAuth';
import useThemeStore from './store/useThemeStore';
import ErrorBoundary from './components/ErrorBoundary/ErrorBoundary';
import './styles/main.scss';

const LandingPage = lazy(() => import('./pages/LandingPage/LandingPage'));
const FeaturesPage = lazy(() => import('./pages/landing/FeaturesPage'));
const FeatureDetailPage = lazy(() => import('./pages/landing/FeatureDetailPage'));
const PricingPage = lazy(() => import('./pages/landing/PricingPage'));
const AboutPage = lazy(() => import('./pages/landing/AboutPage'));
const LoginPage = lazy(() => import('./pages/LoginPage/LoginPage'));
const RegisterPage = lazy(() => import('./pages/RegisterPage/RegisterPage'));
const DashboardPage = lazy(() => import('./pages/app/DashboardPage'));
const HabitsPage = lazy(() => import('./pages/HabitsPage/HabitsPage'));
const HabitDetailPage = lazy(() => import('./pages/app/HabitDetailPage'));
const CreateHabitPage = lazy(() => import('./pages/CreateHabitPage/CreateHabitPage'));
const SchedulePage = lazy(() => import('./pages/app/SchedulePage'));
const TemplatesPage = lazy(() => import('./pages/app/TemplatesPage'));
const PersonalInsightsPage = lazy(() => import('./pages/app/PersonalInsightsPage'));
const CityInsightsPage = lazy(() => import('./pages/app/CityInsightsPage'));
const SocialFeedPage = lazy(() => import('./pages/app/SocialFeedPage'));
const FriendsPage = lazy(() => import('./pages/app/FriendsPage'));
const ChallengesPage = lazy(() => import('./pages/app/ChallengesPage'));
const GamificationPage = lazy(() => import('./pages/app/GamificationPage'));
const JournalPage = lazy(() => import('./pages/app/JournalPage'));
const EconomicsPage = lazy(() => import('./pages/app/EconomicsPage'));
const WebhooksPage = lazy(() => import('./pages/app/WebhooksPage'));
const ProfilePage = lazy(() => import('./pages/ProfilePage/ProfilePage'));
const NotFoundPage = lazy(() => import('./pages/not-found/NotFoundPage'));

const Loading = () => (
  <div className="page-loader">
    <div className="loader-spinner" />
  </div>
);

function App() {
  const location = useLocation();
  const hydrateTheme = useThemeStore((state) => state.hydrateTheme);
  const [showSplash, setShowSplash] = useState(() => !sessionStorage.getItem('pwa-splash-seen'));

  useEffect(() => { hydrateTheme(); }, [hydrateTheme]);
  useEffect(() => {
    if (!showSplash) return;
    const t = setTimeout(() => {
      sessionStorage.setItem('pwa-splash-seen', 'true');
      setShowSplash(false);
    }, 800);
    return () => clearTimeout(t);
  }, [showSplash]);

  return (
    <ErrorBoundary>
      {showSplash && (
        <div className="splash-screen">
          <div className="splash-card">
            <div className="splash-mark" />
          </div>
        </div>
      )}

      <Suspense fallback={<Loading />}>
        <AnimatePresence mode="wait">
          <Routes location={location} key={location.pathname}>
            <Route element={<LandingLayout />}>
              <Route path="/" element={<LandingPage />} />
              <Route path="/features" element={<FeaturesPage />} />
              <Route path="/features/:feature" element={<FeatureDetailPage />} />
              <Route path="/pricing" element={<PricingPage />} />
              <Route path="/about" element={<AboutPage />} />
            </Route>

            <Route path="/login" element={<GuestRoute><LoginPage /></GuestRoute>} />
            <Route path="/register" element={<GuestRoute><RegisterPage /></GuestRoute>} />

            <Route element={<ProtectedAuth><AppLayout /></ProtectedAuth>}>
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/habits" element={<HabitsPage />} />
              <Route path="/habits/new" element={<CreateHabitPage />} />
              <Route path="/habits/:id" element={<HabitDetailPage />} />
              <Route path="/schedule" element={<SchedulePage />} />
              <Route path="/templates" element={<TemplatesPage />} />
              <Route path="/insights/personal" element={<PersonalInsightsPage />} />
              <Route path="/insights/city" element={<CityInsightsPage />} />
              <Route path="/social/feed" element={<SocialFeedPage />} />
              <Route path="/social/friends" element={<FriendsPage />} />
              <Route path="/social/challenges" element={<ChallengesPage />} />
              <Route path="/gamification" element={<GamificationPage />} />
              <Route path="/journal" element={<JournalPage />} />
              <Route path="/economics" element={<EconomicsPage />} />
              <Route path="/webhooks" element={<WebhooksPage />} />
              <Route path="/profile" element={<ProfilePage />} />
            </Route>

            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </AnimatePresence>
      </Suspense>
    </ErrorBoundary>
  );
}

export default App;
