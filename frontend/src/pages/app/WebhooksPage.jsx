import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './WebhooksPage.module.scss';
export default function WebhooksPage() {
  const [webhooks, setWebhooks] = useState([]);
  const [form, setForm] = useState({ url: '', events: '' });
  useEffect(() => { apiClient.get('/webhooks').then(r => setWebhooks(r.data)); }, []);
  const create = async () => { if (!form.url) return; await apiClient.post('/webhooks', { url: form.url, events: form.events.split(',').map(e => e.trim()), secret: null }); setForm({ url: '', events: '' }); apiClient.get('/webhooks').then(r => setWebhooks(r.data)); };
  const remove = async (id) => { await apiClient.delete(`/webhooks/${id}`); setWebhooks(w => w.filter(wh => wh.id !== id)); };
  return (
    <div className={styles.page}>
      <h1>Webhooks</h1>
      <div className={styles.form}>
        <input value={form.url} onChange={e => setForm(f => ({ ...f, url: e.target.value }))} placeholder="Webhook URL" />
        <input value={form.events} onChange={e => setForm(f => ({ ...f, events: e.target.value }))} placeholder="Events (comma separated)" />
        <button onClick={create}>Create</button>
      </div>
      <div className={styles.list}>
        {webhooks.map(w => (
          <div key={w.id} className={styles.webhook}>
            <span>{w.url}</span>
            <button onClick={() => remove(w.id)} className={styles.deleteBtn}>Delete</button>
          </div>
        ))}
      </div>
    </div>
  );
}
