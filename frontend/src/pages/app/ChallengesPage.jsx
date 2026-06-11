import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './ChallengesPage.module.scss';
export default function ChallengesPage() {
  const [challenges, setChallenges] = useState([]);
  useEffect(() => { apiClient.get('/social/challenges').then(r => setChallenges(r.data)); }, []);
  const join = async (id) => { await apiClient.post(`/social/challenges/${id}/join`); };
  return (
    <div className={styles.page}>
      <h1>Challenges</h1>
      <div className={styles.grid}>
        {challenges.map(c => (
          <div key={c.id} className={styles.card}>
            <h3>{c.name}</h3>
            <p>{c.description}</p>
            <div className={styles.dates}>{new Date(c.startDate).toLocaleDateString()} — {new Date(c.endDate).toLocaleDateString()}</div>
            <button onClick={() => join(c.id)} className={styles.joinBtn}>Join Challenge</button>
          </div>
        ))}
      </div>
    </div>
  );
}
