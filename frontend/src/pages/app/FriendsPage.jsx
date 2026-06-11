import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './FriendsPage.module.scss';
export default function FriendsPage() {
  const [friends, setFriends] = useState([]);
  const [friendId, setFriendId] = useState('');
  const load = async () => { const r = await apiClient.get('/social/friends'); setFriends(r.data); };
  useEffect(() => { load(); }, []);
  const sendRequest = async () => { if (!friendId) return; await apiClient.post(`/social/friends/${friendId}`); setFriendId(''); };
  return (
    <div className={styles.page}>
      <h1>Friends</h1>
      <div className={styles.addRow}>
        <input value={friendId} onChange={e => setFriendId(e.target.value)} placeholder="Friend ID to add..." />
        <button onClick={sendRequest}>Send Request</button>
      </div>
      <div className={styles.list}>
        {friends.map(f => (
          <div key={f.id} className={styles.friend}>
            <span>{f.friendId}</span>
            <span className={styles.status}>{f.status}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
