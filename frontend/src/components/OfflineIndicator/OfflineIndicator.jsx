import { useState, useEffect } from 'react';
import styles from './OfflineIndicator.module.scss';

export default function OfflineIndicator() {
  const [isOnline, setIsOnline] = useState(navigator.onLine);

  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);
    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);
    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  if (isOnline) return null;

  return (
    <div className={styles.banner}>
      <span className={styles.icon}>📡</span>
      <span className={styles.text}>You are offline. Changes will sync when connected.</span>
    </div>
  );
}
