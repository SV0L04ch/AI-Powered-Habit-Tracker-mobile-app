import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './GamificationPage.module.scss';
export default function GamificationPage() {
  const [data, setData] = useState(null);
  useEffect(() => { apiClient.get('/gamification').then(r => setData(r.data)); }, []);
  if (!data) return <div className="page-loader"><div className="loader-spinner" /></div>;
  return (
    <div className={styles.page}>
      <h1>Gamification</h1>
      <div className={styles.stats}>
        <div className={styles.card}><div className={styles.level}>{data.level}</div><div className={styles.label}>Level</div></div>
        <div className={styles.card}><div className={styles.xp}>{data.totalXP} XP</div><div className={styles.label}>Experience</div></div>
        <div className={styles.card}>
          <div className={styles.progressBar}><div className={styles.progressFill} style={{ width: `${data.progressPercent}%` }} /></div>
          <div className={styles.label}>{data.progressPercent}% to next level</div>
        </div>
      </div>
      <h2>Recent Achievements</h2>
      <div className={styles.achievements}>
        {(data.recentAchievements || []).map(a => (
          <div key={a.id} className={styles.achievement}><span className={styles.achievementIcon}>{a.icon}</span><span>{a.name}</span></div>
        ))}
      </div>
    </div>
  );
}
