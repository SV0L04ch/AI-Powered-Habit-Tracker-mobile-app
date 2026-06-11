import { useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import styles from './HeroSection.module.scss';

function seededRandom(seed) {
  let x = Math.sin(seed) * 10000;
  return x - Math.floor(x);
}

const container = {
  hidden: { opacity: 0 },
  show: {
    opacity: 1,
    transition: { staggerChildren: 0.15, delayChildren: 0.3 },
  },
};

const item = {
  hidden: { opacity: 0, y: 40, filter: 'blur(10px)' },
  show: { opacity: 1, y: 0, filter: 'blur(0px)', transition: { duration: 0.7, ease: [0.25, 1.2, 0.5, 1] } },
};

const phoneVariants = {
  hidden: { opacity: 0, y: 80, rotateY: -15 },
  show: {
    opacity: 1,
    y: 0,
    rotateY: 0,
    transition: { duration: 1, delay: 0.6, ease: [0.25, 1.2, 0.5, 1] },
  },
};

export default function HeroSection() {
  const navigate = useNavigate();

  const particles = useMemo(() =>
    Array.from({ length: 20 }).map((_, i) => ({
      left: `${seededRandom(i * 4 + 1) * 100}%`,
      top: `${seededRandom(i * 4 + 2) * 100}%`,
      animationDelay: `${seededRandom(i * 4 + 3) * 5}s`,
      animationDuration: `${3 + seededRandom(i * 4 + 4) * 4}s`,
    })), []
  );

  return (
    <section className={styles.hero}>
      <div className={styles.particles}>
        {particles.map((p, i) => (
          <div
            key={i}
            className={styles.particle}
            style={p}
          />
        ))}
      </div>

      <motion.div className={styles.content} variants={container} initial="hidden" animate="show">
        <motion.div className={styles.badge} variants={item}>
          <span className={styles.badgeDot} />
          AI-Powered Habit Tracking
        </motion.div>

        <motion.h1 className={styles.headline} variants={item}>
          Build Better Habits,
          <br />
          <span className={styles.gradient}>One Day at a Time</span>
        </motion.h1>

        <motion.p className={styles.subheadline} variants={item}>
          Track positive and negative habits with AI-powered insights, streaks,
          gamification, and beautiful analytics. Your personal wellness companion.
        </motion.p>

        <motion.div className={styles.actions} variants={item}>
          <button className={styles.ctaPrimary} onClick={() => navigate('/register')}>
            Get Started Free
          </button>
          <button className={styles.ctaGhost} onClick={() => navigate('/login')}>
            Sign In
          </button>
        </motion.div>

        <motion.div className={styles.stats} variants={item}>
          <div className={styles.stat}>
            <span className={styles.statNumber}>10K+</span>
            <span className={styles.statLabel}>Habits Tracked</span>
          </div>
          <div className={styles.statDivider} />
          <div className={styles.stat}>
            <span className={styles.statNumber}>50K+</span>
            <span className={styles.statLabel}>Completions</span>
          </div>
          <div className={styles.statDivider} />
          <div className={styles.stat}>
            <span className={styles.statNumber}>4.8</span>
            <span className={styles.statLabel}>User Rating</span>
          </div>
        </motion.div>
      </motion.div>

      <motion.div className={styles.phoneFrame} variants={phoneVariants} initial="hidden" animate="show">
        <div className={styles.phoneNotch} />
        <div className={styles.phoneScreen}>
          <div className={styles.mockHeader}>
            <div className={styles.mockGreeting}>Good morning!</div>
            <div className={styles.mockDate}>Today, June 11</div>
          </div>
          <div className={styles.mockStreak}>
            <span className={styles.mockFire}>🔥</span>
            <span className={styles.mockStreakCount}>7 Day Streak</span>
          </div>
          <div className={styles.mockHabits}>
            <div className={styles.mockHabit}>
              <div className={`${styles.mockCheck} ${styles.checked}`}>✓</div>
              <span>Morning Meditation</span>
            </div>
            <div className={styles.mockHabit}>
              <div className={`${styles.mockCheck} ${styles.checked}`}>✓</div>
              <span>Read 30 minutes</span>
            </div>
            <div className={styles.mockHabit}>
              <div className={styles.mockCheck} />
              <span>Workout</span>
            </div>
            <div className={styles.mockHabit}>
              <div className={`${styles.mockCheck} ${styles.checked}`}>✓</div>
              <span>Drink water</span>
            </div>
          </div>
          <div className={styles.mockProgress}>
            <div className={styles.mockProgressBar}>
              <div className={styles.mockProgressFill} style={{ width: '75%' }} />
            </div>
            <span className={styles.mockProgressText}>75% Complete</span>
          </div>
        </div>
      </motion.div>
    </section>
  );
}
