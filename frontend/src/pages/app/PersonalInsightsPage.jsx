import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './PersonalInsightsPage.module.scss';

export default function PersonalInsightsPage() {
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiClient.get('/stats/daily-summary').then(r => setSummary(r.data)).finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="page-loader"><div className="loader-spinner" /></div>;

  return (
    <div className={styles.page}>
      <h1>Personal Insights</h1>
      {summary ? (
        <div className={styles.content}>
          <div className={styles.metricRow}>
            <div className={styles.metric}><span className={styles.value}>{summary.completionRate || 0}%</span><span className={styles.label}>Completion Rate</span></div>
            <div className={styles.metric}><span className={styles.value}>{summary.completedCount || 0}</span><span className={styles.label}>Completed</span></div>
            <div className={styles.metric}><span className={styles.value}>{summary.partialCount || 0}</span><span className={styles.label}>Partial</span></div>
            <div className={styles.metric}><span className={styles.value}>{summary.skippedCount || 0}</span><span className={styles.label}>Skipped</span></div>
          </div>
          {summary.aiComment && <div className={styles.aiCard}><h3>AI Insight</h3><p>{summary.aiComment}</p></div>}
        </div>
      ) : <p>No data available yet.</p>}
    </div>
  );
}
