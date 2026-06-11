import { useMemo } from 'react';
import { motion } from 'framer-motion';
import styles from './WeeklyRecap.module.scss';

export default function WeeklyRecap({ habits, entries }) {
  const stats = useMemo(() => {
    const completed = entries?.filter(e => e.status === 'Completed').length || 0;
    const total = entries?.length || 0;
    const streaks = habits?.map(h => h.streak || 0) || [];
    const maxStreak = Math.max(...streaks, 0);
    const avgCompletion = total > 0 ? Math.round((completed / total) * 100) : 0;
    return { completed, total, maxStreak, avgCompletion, habitCount: habits?.length || 0 };
  }, [habits, entries]);

  return (
    <motion.div
      className={styles.recap}
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.6, ease: [0.25, 1.2, 0.5, 1] }}
    >
      <div className={styles.header}>
        <span className={styles.badge}>Weekly Recap</span>
        <h3 className={styles.title}>This Week</h3>
      </div>

      <div className={styles.statsGrid}>
        <div className={styles.statCard}>
          <span className={styles.statValue}>{stats.completed}</span>
          <span className={styles.statLabel}>Completions</span>
        </div>
        <div className={styles.statCard}>
          <span className={styles.statValue}>{stats.avgCompletion}%</span>
          <span className={styles.statLabel}>Success Rate</span>
        </div>
        <div className={styles.statCard}>
          <span className={styles.statValue}>{stats.maxStreak}</span>
          <span className={styles.statLabel}>Best Streak</span>
        </div>
        <div className={styles.statCard}>
          <span className={styles.statValue}>{stats.habitCount}</span>
          <span className={styles.statLabel}>Active Habits</span>
        </div>
      </div>

      <div className={styles.progressSection}>
        <div className={styles.progressBar}>
          <motion.div
            className={styles.progressFill}
            initial={{ width: 0 }}
            animate={{ width: `${stats.avgCompletion}%` }}
            transition={{ duration: 1, delay: 0.3 }}
          />
        </div>
        <span className={styles.progressText}>{stats.avgCompletion}% completion rate</span>
      </div>

      <div className={styles.footer}>
        Keep going! Every day is a new opportunity.
      </div>
    </motion.div>
  );
}
