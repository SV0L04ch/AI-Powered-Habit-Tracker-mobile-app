import { useState } from 'react';
import apiClient from '../../services/apiClient';
import styles from './SocialFeedPage.module.scss';
export default function SocialFeedPage() {
  const [feed, setFeed] = useState([]);
  const [city, setCity] = useState('');
  const [postText, setPostText] = useState('');
  const load = async (c) => { if (!c) return; const r = await apiClient.get(`/social/feed?city=${c}`); setFeed(r.data); };
  const post = async () => { if (!postText || !city) return; await apiClient.post('/social/feed', { city, habitName: postText }); setPostText(''); load(city); };
  return (
    <div className={styles.page}>
      <h1>Social Feed</h1>
      <div className={styles.search}>
        <input value={city} onChange={e => { setCity(e.target.value); load(e.target.value); }} placeholder="City name..." />
      </div>
      <div className={styles.postBar}>
        <input value={postText} onChange={e => setPostText(e.target.value)} placeholder="What habit did you complete?" />
        <button onClick={post}>Post</button>
      </div>
      <div className={styles.feed}>
        {feed.map(f => (
          <div key={f.id} className={styles.post}>
            <strong>{f.habitName}</strong>
            <span className={styles.time}>{new Date(f.completedAt).toLocaleString()}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
