import { useState } from 'react';
import apiClient from '../../services/apiClient';
import styles from './CityInsightsPage.module.scss';
export default function CityInsightsPage() {
  const [city, setCity] = useState('');
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const search = async () => { if (!city) return; setLoading(true); try { const r = await apiClient.get(`/stats/city-summary?city=${city}`); setData(r.data); } finally { setLoading(false); } };
  return (
    <div className={styles.page}>
      <h1>City Insights</h1>
      <div className={styles.search}>
        <input value={city} onChange={e => setCity(e.target.value)} placeholder="Enter city name..." onKeyDown={e => e.key === 'Enter' && search()} />
        <button onClick={search} disabled={loading}>Search</button>
      </div>
      {data && <div className={styles.result}><pre>{JSON.stringify(data, null, 2)}</pre></div>}
    </div>
  );
}
