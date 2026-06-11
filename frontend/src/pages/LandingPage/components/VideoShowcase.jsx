import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './VideoShowcase.module.scss';

const videos = [
  {
    title: 'Track Habits',
    description: 'Create and complete habits with a single tap',
    icon: '✅',
    color: 'var(--app-accent)',
  },
  {
    title: 'Build Streaks',
    description: 'Watch your streaks grow with animated celebrations',
    icon: '🔥',
    color: 'var(--app-primary)',
  },
  {
    title: 'Get Insights',
    description: 'AI-powered analytics and daily summaries',
    icon: '🧠',
    color: 'var(--app-accent-2)',
  },
];

const container = {
  hidden: {},
  show: { transition: { staggerChildren: 0.15 } },
};

const card = {
  hidden: { opacity: 0, y: 40 },
  show: { opacity: 1, y: 0, transition: { duration: 0.6, ease: [0.25, 1.2, 0.5, 1] } },
};

export default function VideoShowcase() {
  const [ref, inView] = useInView({ threshold: 0.2, triggerOnce: true });

  return (
    <section className={styles.showcase} ref={ref}>
      <motion.div
        className={styles.header}
        initial={{ opacity: 0, y: 30 }}
        animate={inView ? { opacity: 1, y: 0 } : {}}
        transition={{ duration: 0.6 }}
      >
        <span className={styles.label}>See It In Action</span>
        <h2 className={styles.title}>App Showcase</h2>
        <p className={styles.subtitle}>Experience the key features through our interactive demos</p>
      </motion.div>

      <motion.div
        className={styles.grid}
        variants={container}
        initial="hidden"
        animate={inView ? 'show' : 'hidden'}
      >
        {videos.map((v, i) => (
          <motion.div key={i} className={styles.card} variants={card}>
            <div className={styles.cardIcon} style={{ background: `${v.color}15`, color: v.color }}>
              {v.icon}
            </div>
            <div className={styles.playButton}>
              <svg viewBox="0 0 24 24" fill="currentColor" width="24" height="24">
                <path d="M8 5v14l11-7z" />
              </svg>
            </div>
            <h3 className={styles.cardTitle}>{v.title}</h3>
            <p className={styles.cardDescription}>{v.description}</p>
          </motion.div>
        ))}
      </motion.div>
    </section>
  );
}
