import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { fetchHabitEntries } from '../../services/habitService';
import styles from './HabitDetailPage.module.scss';

export default function HabitDetailPage() {
  const { id } = useParams();
  const [entries, setEntries] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchHabitEntries(id).then(r => setEntries(r || [])).finally(() => setLoading(false));
  }, [id]);

  if (loading) return <div className="page-loader"><div className="loader-spinner" /></div>;

  return (
    <div className={styles.page}>
      <Link to="/habits" className={styles.back}>← Back to Habits</Link>
      <h1>Habit Details</h1>
      <div className={styles.entries}>
        {entries.length === 0 ? <p className={styles.empty}>No entries yet.</p> : entries.map(e => (
          <div key={e.id} className={styles.entry}>
            <span className={styles.date}>{new Date(e.date).toLocaleDateString()}</span>
            <span className={styles.status}>{e.status || 'N/A'}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
