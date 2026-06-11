import { Link } from 'react-router-dom';
import styles from './FeaturesPage.module.scss';

const features = [
  { slug: 'habits', icon: '🎯', title: 'Smart Habit Tracking', desc: 'Create positive and negative habits with custom triggers, schedules, and penalty days. Track your progress with detailed entry logs.' },
  { slug: 'insights', icon: '📊', title: 'AI-Powered Insights', desc: 'Get personalized daily summaries, city-wide statistics, and AI coaching tips to optimize your habit routine.' },
  { slug: 'social', icon: '👥', title: 'Social Features', desc: 'Connect with friends, share your city feed, and join challenges to stay motivated together.' },
  { slug: 'gamification', icon: '🏆', title: 'Gamification & Rewards', desc: 'Earn XP, level up, unlock achievements, collect HabitCoins, and climb league rankings.' },
  { slug: 'journal', icon: '📓', title: 'Wellness Journal', desc: 'Track mood, sleep, meals, and goals alongside your habits for a complete wellness picture.' },
  { slug: 'weather', icon: '🌤', title: 'Weather Integration', desc: 'Habit suggestions adapt to weather conditions. Get AI daily summaries that factor in your local weather.' },
];

export default function FeaturesPage() {
  return (
    <div className={styles.page}>
      <section className={styles.hero}>
        <h1 className={styles.title}>
          Features that <span className={styles.gradient}>actually work</span>
        </h1>
        <p className={styles.subtitle}>
          Everything you need to build, track, and maintain better habits.
        </p>
      </section>
      <section className={styles.grid}>
        {features.map((f) => (
          <Link to={`/features/${f.slug}`} key={f.slug} className={styles.card}>
            <div className={styles.icon}>{f.icon}</div>
            <h3>{f.title}</h3>
            <p>{f.desc}</p>
            <span className={styles.link}>Learn more →</span>
          </Link>
        ))}
      </section>
    </div>
  );
}
