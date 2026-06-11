import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './TemplatesPage.module.scss';

export default function TemplatesPage() {
  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiClient.get('/templates').then(r => setTemplates(r.data)).finally(() => setLoading(false));
  }, []);

  const install = async (templateId) => {
    await apiClient.post(`/templates/${templateId}/install`);
    setTemplates(t => t.map(tp => tp.id === templateId ? { ...tp, installCount: tp.installCount + 1 } : tp));
  };

  if (loading) return <div className="page-loader"><div className="loader-spinner" /></div>;

  return (
    <div className={styles.page}>
      <h1>Templates</h1>
      <div className={styles.grid}>
        {templates.map(t => (
          <div key={t.id} className={styles.card}>
            <div className={styles.icon}>{t.icon}</div>
            <h3>{t.name}</h3>
            <p>{t.description}</p>
            <span className={styles.category}>{t.category}</span>
            <button onClick={() => install(t.id)} className={styles.installBtn}>Install ({t.installCount})</button>
          </div>
        ))}
      </div>
    </div>
  );
}
