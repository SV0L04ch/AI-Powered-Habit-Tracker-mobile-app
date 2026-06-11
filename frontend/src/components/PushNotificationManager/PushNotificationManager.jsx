import { useState, useEffect } from 'react';
import styles from './PushNotificationManager.module.scss';

export default function PushNotificationManager() {
  const [permission, setPermission] = useState('default');
  const [subscribed, setSubscribed] = useState(false);

  useEffect(() => {
    if ('Notification' in window) {
      setPermission(Notification.permission);
    }
  }, []);

  const requestPermission = async () => {
    if (!('Notification' in window)) return;
    const result = await Notification.requestPermission();
    setPermission(result);
    if (result === 'granted') {
      setSubscribed(true);
      new Notification('Habit Tracker', {
        body: 'Notifications enabled! You will receive daily reminders.',
        icon: '/icons-192.png',
      });
    }
  };

  if (permission === 'denied') {
    return (
      <div className={styles.container}>
        <span className={styles.icon}>🔕</span>
        <span className={styles.text}>Notifications blocked. Enable in browser settings.</span>
      </div>
    );
  }

  if (subscribed) {
    return (
      <div className={styles.containerSuccess}>
        <span className={styles.icon}>✅</span>
        <span className={styles.text}>Notifications enabled!</span>
      </div>
    );
  }

  return (
    <button className={styles.enableBtn} onClick={requestPermission}>
      Enable Notifications
    </button>
  );
}
