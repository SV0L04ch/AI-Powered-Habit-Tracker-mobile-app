import { useState } from 'react';
import styles from './JournalPage.module.scss';
const tabs = ['Notes', 'Mood', 'Sleep', 'Meals', 'Goals'];
export default function JournalPage() {
  const [activeTab, setActiveTab] = useState('Notes');
  return (
    <div className={styles.page}>
      <h1>Journal</h1>
      <div className={styles.tabs}>
        {tabs.map(t => <button key={t} className={`${styles.tab} ${activeTab === t ? styles.active : ''}`} onClick={() => setActiveTab(t)}>{t}</button>)}
      </div>
      <div className={styles.content}>
        <p className={styles.placeholder}>{activeTab} section — data will appear here.</p>
      </div>
    </div>
  );
}
