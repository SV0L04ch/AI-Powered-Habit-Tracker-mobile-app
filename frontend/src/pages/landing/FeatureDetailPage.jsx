import { useParams, Link } from 'react-router-dom';
import styles from './FeatureDetailPage.module.scss';

const featureData = {
  habits: { icon: '🎯', title: 'Smart Habit Tracking', desc: 'Create positive and negative habits with custom triggers, schedules, and penalty days.', steps: ['Create a habit with name, type, and trigger', 'Set your schedule (daily, weekdays, custom)', 'Track completions with one tap', 'View detailed history and streaks'] },
  insights: { icon: '📊', title: 'AI-Powered Insights', desc: 'Get personalized daily summaries and AI coaching.', steps: ['View your daily productivity score', 'Get AI-generated habit recommendations', 'See city-wide habit statistics', 'Receive weather-aware suggestions'] },
  social: { icon: '👥', title: 'Social Features', desc: 'Connect with friends and your city community.', steps: ['Share your progress to the city feed', 'Send and accept friend requests', 'Join or create habit challenges', 'Compete on the leaderboard'] },
  gamification: { icon: '🏆', title: 'Gamification & Rewards', desc: 'Earn rewards and level up as you build habits.', steps: ['Earn XP for every completed habit', 'Level up through 15 levels', 'Unlock 8 unique achievements', 'Collect HabitCoins for the store'] },
  journal: { icon: '📓', title: 'Wellness Journal', desc: 'Track your complete wellness alongside habits.', steps: ['Log your mood throughout the day', 'Track sleep quality and duration', 'Record meals and nutrition', 'Set and monitor personal goals'] },
  weather: { icon: '🌤', title: 'Weather Integration', desc: 'Habit suggestions that adapt to the weather.', steps: ['Auto-detect your city weather', 'Get weather-aware habit tips', 'See weather in your daily summary', 'Plan outdoor habits around forecasts'] },
};

export default function FeatureDetailPage() {
  const { feature } = useParams();
  const data = featureData[feature] || featureData.habits;

  return (
    <div className={styles.page}>
      <Link to="/features" className={styles.back}>← Back to Features</Link>
      <section className={styles.hero}>
        <div className={styles.icon}>{data.icon}</div>
        <h1>{data.title}</h1>
        <p>{data.desc}</p>
      </section>
      <section className={styles.steps}>
        <h2>How it works</h2>
        <div className={styles.stepsGrid}>
          {data.steps.map((step, i) => (
            <div key={i} className={styles.stepCard}>
              <div className={styles.stepNum}>{String(i + 1).padStart(2, '0')}</div>
              <p>{step}</p>
            </div>
          ))}
        </div>
      </section>
      <section className={styles.cta}>
        <h2>Ready to try {data.title}?</h2>
        <Link to="/register" className={styles.ctaBtn}>Get Started Free</Link>
      </section>
    </div>
  );
}
