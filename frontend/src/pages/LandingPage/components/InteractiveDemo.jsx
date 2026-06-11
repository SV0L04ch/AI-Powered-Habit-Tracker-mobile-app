import { useState } from 'react';
import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './InteractiveDemo.module.scss';

export default function InteractiveDemo() {
  const [ref, inView] = useInView({ threshold: 0.2, triggerOnce: true });
  const [habits, setHabits] = useState([
    { id: 1, name: 'Morning Meditation', done: false },
    { id: 2, name: 'Read 30 minutes', done: false },
    { id: 3, name: 'Workout', done: false },
  ]);

  const toggleHabit = (id) => {
    setHabits(habits.map(h => h.id === id ? { ...h, done: !h.done } : h));
  };

  const completed = habits.filter(h => h.done).length;
  const progress = (completed / habits.length) * 100;

  return (
    <section className={styles.demo} ref={ref}>
      <motion.div
        className={styles.header}
        initial={{ opacity: 0, y: 30 }}
        animate={inView ? { opacity: 1, y: 0 } : {}}
        transition={{ duration: 0.6 }}
      >
        <span className={styles.label}>Try It Now</span>
        <h2 className={styles.title}>Interactive Demo</h2>
        <p className={styles.subtitle}>Try the app without signing up. Click to complete habits!</p>
      </motion.div>

      <motion.div
        className={styles.phone}
        initial={{ opacity: 0, y: 50, rotateY: -10 }}
        animate={inView ? { opacity: 1, y: 0, rotateY: 0 } : {}}
        transition={{ duration: 0.8, delay: 0.2, ease: [0.25, 1.2, 0.5, 1] }}
      >
        <div className={styles.phoneNotch} />
        <div className={styles.phoneScreen}>
          <div className={styles.greeting}>Good morning!</div>
          <div className={styles.date}>Today, June 11</div>

          <div className={styles.progressSection}>
            <div className={styles.progressBar}>
              <div className={styles.progressFill} style={{ width: `${progress}%` }} />
            </div>
            <span className={styles.progressText}>{completed}/{habits.length} Complete</span>
          </div>

          <div className={styles.habitList}>
            {habits.map((habit, i) => (
              <motion.button
                key={habit.id}
                className={`${styles.habitItem} ${habit.done ? styles.done : ''}`}
                onClick={() => toggleHabit(habit.id)}
                whileTap={{ scale: 0.95 }}
                initial={{ opacity: 0, x: -20 }}
                animate={inView ? { opacity: 1, x: 0 } : {}}
                transition={{ delay: 0.3 + i * 0.1 }}
              >
                <div className={`${styles.check} ${habit.done ? styles.checked : ''}`}>
                  {habit.done && '✓'}
                </div>
                <span>{habit.name}</span>
              </motion.button>
            ))}
          </div>

          {completed === habits.length && (
            <motion.div
              className={styles.complete}
              initial={{ opacity: 0, scale: 0.8 }}
              animate={{ opacity: 1, scale: 1 }}
              transition={{ type: 'spring', stiffness: 300, damping: 20 }}
            >
              All done! 🎉
            </motion.div>
          )}
        </div>
      </motion.div>
    </section>
  );
}
