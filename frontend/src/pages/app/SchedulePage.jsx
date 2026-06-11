import styles from './SchedulePage.module.scss';
export default function SchedulePage() {
  return (
    <div className={styles.page}>
      <h1>Schedule</h1>
      <div className={styles.placeholder}>
        <p>Calendar view coming soon.</p>
        <p className={styles.muted}>Connect your habits to a weekly schedule with custom days and times.</p>
      </div>
    </div>
  );
}
