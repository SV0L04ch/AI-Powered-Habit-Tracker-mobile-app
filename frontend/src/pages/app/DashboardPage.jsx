import { useState, useEffect } from 'react';
import useAuthStore from '../../store/useAuthStore';
import { getDailySummary } from '../../services/dailySummaryService';
import { getWeather } from '../../services/weatherService';
import styles from './DashboardPage.module.scss';

export default function DashboardPage() {
  const profile = useAuthStore((s) => s.profile);
  const [summary, setSummary] = useState(null);
  const [weather, setWeather] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const [s, w] = await Promise.all([
          getDailySummary().catch(() => null),
          getWeather(profile?.city || 'Moscow').catch(() => null),
        ]);
        setSummary(s?.data);
        setWeather(w?.data);
      } finally { setLoading(false); }
    };
    load();
  }, [profile?.city]);

  if (loading) return <div className="page-loader"><div className="loader-spinner" /></div>;

  const greeting = () => {
    const h = new Date().getHours();
    if (h < 12) return 'Good morning';
    if (h < 18) return 'Good afternoon';
    return 'Good evening';
  };

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <h1>{greeting()}, {profile?.name || 'User'}!</h1>
        <p className={styles.date}>{new Date().toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}</p>
      </div>

      <div className={styles.metrics}>
        <div className={styles.metricCard}>
          <div className={styles.metricIcon}>🔥</div>
          <div className={styles.metricValue}>{summary?.currentStreak || 0}</div>
          <div className={styles.metricLabel}>Day Streak</div>
        </div>
        <div className={styles.metricCard}>
          <div className={styles.metricIcon}>⭐</div>
          <div className={styles.metricValue}>{summary?.totalXP || 0}</div>
          <div className={styles.metricLabel}>Total XP</div>
        </div>
        <div className={styles.metricCard}>
          <div className={styles.metricIcon}>💰</div>
          <div className={styles.metricValue}>{summary?.habitCoins || 0}</div>
          <div className={styles.metricLabel}>HabitCoins</div>
        </div>
        <div className={styles.metricCard}>
          <div className={styles.metricIcon}>📊</div>
          <div className={styles.metricValue}>{summary?.completionRate || 0}%</div>
          <div className={styles.metricLabel}>Today</div>
        </div>
      </div>

      {summary?.quote && (
        <div className={styles.quoteCard}>
          <p className={styles.quoteText}>&ldquo;{summary.quote.text}&rdquo;</p>
          <p className={styles.quoteAuthor}>— {summary.quote.author}</p>
        </div>
      )}

      {weather && (
        <div className={styles.weatherCard}>
          <div className={styles.weatherInfo}>
            <span className={styles.weatherIcon}>🌤</span>
            <div>
              <div className={styles.weatherTemp}>{Math.round(weather.temp)}°C</div>
              <div className={styles.weatherCity}>{profile?.city || 'Your city'}</div>
            </div>
          </div>
          <div className={styles.weatherDesc}>{weather.description}</div>
        </div>
      )}
    </div>
  );
}
