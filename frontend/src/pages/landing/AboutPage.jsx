import styles from './AboutPage.module.scss';

export default function AboutPage() {
  return (
    <div className={styles.page}>
      <section className={styles.hero}>
        <h1>About <span className={styles.gradient}>Flowstate</span></h1>
        <p>We believe small daily actions lead to extraordinary results.</p>
      </section>
      <section className={styles.content}>
        <div className={styles.card}>
          <h2>Our Mission</h2>
          <p>To help millions of people build lasting habits through technology, community, and AI-powered insights.</p>
        </div>
        <div className={styles.card}>
          <h2>Why Flowstate?</h2>
          <p>Because consistency beats intensity. We help you find your flow state — where habits become effortless and progress is automatic.</p>
        </div>
        <div className={styles.card}>
          <h2>Built with Love</h2>
          <p>Created by developers who struggled with habits themselves. Every feature is designed to make habit-building enjoyable, not a chore.</p>
        </div>
      </section>
    </div>
  );
}
