import { Link } from 'react-router-dom';
import styles from './HomePage.module.scss';

const features = [
  { icon: '🎯', title: 'Smart Habits', desc: 'Create positive and negative habits with custom triggers and schedules.' },
  { icon: '📊', title: 'AI Insights', desc: 'Get AI-powered daily summaries and personalized coaching.' },
  { icon: '🌤', title: 'Weather Aware', desc: 'Habit suggestions based on weather conditions in your city.' },
  { icon: '🏆', title: 'Gamification', desc: 'Earn XP, level up, unlock achievements and compete in leagues.' },
  { icon: '👥', title: 'Social', desc: 'Share your progress, join challenges, and connect with friends.' },
  { icon: '📓', title: 'Journal', desc: 'Track mood, sleep, meals, and goals alongside your habits.' },
];

const stats = [
  { value: '10K+', label: 'Active Users' },
  { value: '500K+', label: 'Habits Tracked' },
  { value: '82%', label: 'Success Rate' },
  { value: '4.9', label: 'App Rating' },
];

export default function HomePage() {
  return (
    <div className={styles.page}>
      <section className={styles.hero}>
        <div className={styles.heroContent}>
          <div className={styles.badge}>AI-Powered Habit Tracking</div>
          <h1 className={styles.title}>
            Build better habits,<br />
            <span className={styles.gradient}>one day at a time.</span>
          </h1>
          <p className={styles.subtitle}>
            Flowstate combines AI insights, gamification, and social features
            to help you build lasting habits that stick.
          </p>
          <div className={styles.heroActions}>
            <Link to="/register" className={styles.ctaBtn}>Get Started Free</Link>
            <Link to="/features" className={styles.learnBtn}>Learn More →</Link>
          </div>
        </div>
        <div className={styles.heroVisual}>
          <div className={styles.mockupCard}>
            <div className={styles.mockupHeader}>
              <span className={styles.mockupDot} />
              <span className={styles.mockupDot} />
              <span className={styles.mockupDot} />
            </div>
            <div className={styles.mockupBody}>
              {['Morning Meditation', 'Read 30min', 'Drink Water', 'Workout'].map((h, i) => (
                <div key={i} className={styles.mockupItem}>
                  <span className={styles.mockupCheck}>{i < 2 ? '✓' : '○'}</span>
                  <span>{h}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>

      <section className={styles.stats}>
        {stats.map((s, i) => (
          <div key={i} className={styles.statItem}>
            <div className={styles.statValue}>{s.value}</div>
            <div className={styles.statLabel}>{s.label}</div>
          </div>
        ))}
      </section>

      <section className={styles.features}>
        <h2 className={styles.sectionTitle}>Everything you need to build better habits</h2>
        <div className={styles.featuresGrid}>
          {features.map((f, i) => (
            <Link to="/features" key={i} className={styles.featureCard}>
              <div className={styles.featureIcon}>{f.icon}</div>
              <h3 className={styles.featureTitle}>{f.title}</h3>
              <p className={styles.featureDesc}>{f.desc}</p>
            </Link>
          ))}
        </div>
      </section>

      <section className={styles.cta}>
        <h2 className={styles.ctaTitle}>Ready to transform your habits?</h2>
        <p className={styles.ctaDesc}>Start your journey today. Free for individuals.</p>
        <Link to="/register" className={styles.ctaBtn}>Get Started Free</Link>
      </section>
    </div>
  );
}
