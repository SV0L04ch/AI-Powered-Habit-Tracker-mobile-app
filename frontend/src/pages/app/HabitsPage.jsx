import { useEffect } from 'react';
import { Link } from 'react-router-dom';
import useHabitsStore from '../../store/useHabitsStore';
import styles from './HabitsPage.module.scss';

export default function HabitsPage() {
  const { habits, isLoading, getHabits, markHabitCompleted, deleteHabit } = useHabitsStore();

  useEffect(() => { getHabits(); }, [getHabits]);

  if (isLoading) return <div className="page-loader"><div className="loader-spinner" /></div>;

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <h1>My Habits</h1>
        <Link to="/habits/new" className={styles.addBtn}>+ New Habit</Link>
      </div>
      <div className={styles.list}>
        {habits.length === 0 ? (
          <div className={styles.empty}>
            <p>No habits yet. Start by creating your first habit!</p>
            <Link to="/habits/new" className={styles.addBtn}>Create Habit</Link>
          </div>
        ) : habits.map((h) => (
          <div key={h.id} className={styles.card}>
            <div className={styles.cardInfo}>
              <h3>{h.name}</h3>
              <span className={styles.badge}>{h.isPositive ? 'Positive' : 'Negative'}</span>
            </div>
            <div className={styles.cardActions}>
              <button onClick={() => markHabitCompleted(h)} className={styles.completeBtn}>✓</button>
              <Link to={`/habits/${h.id}`} className={styles.detailBtn}>→</Link>
              <button onClick={() => deleteHabit(h.id)} className={styles.deleteBtn}>×</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
