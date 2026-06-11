import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './AppPreview3D.module.scss';

const screens = [
  { label: 'Habits', color: '#d97706', icon: '✅' },
  { label: 'Insights', color: '#7c3aed', icon: '📊' },
  { label: 'Profile', color: '#059669', icon: '👤' },
];

export default function AppPreview3D() {
  const [ref, inView] = useInView({ threshold: 0.2, triggerOnce: true });

  return (
    <section className={styles.preview} ref={ref}>
      <motion.div
        className={styles.header}
        initial={{ opacity: 0, y: 30 }}
        animate={inView ? { opacity: 1, y: 0 } : {}}
        transition={{ duration: 0.6 }}
      >
        <span className={styles.label}>Beautiful Design</span>
        <h2 className={styles.title}>See Every Screen</h2>
        <p className={styles.subtitle}>Crafted with attention to detail across every interaction</p>
      </motion.div>

      <div className={styles.showcase}>
        {screens.map((screen, i) => (
          <motion.div
            key={i}
            className={styles.phoneCard}
            initial={{ opacity: 0, y: 60, rotateY: -15 + i * 15 }}
            animate={inView ? { opacity: 1, y: 0, rotateY: -10 + i * 10 } : {}}
            transition={{ duration: 0.8, delay: i * 0.15, ease: [0.25, 1.2, 0.5, 1] }}
            style={{ '--card-color': screen.color }}
          >
            <div className={styles.phoneFrame}>
              <div className={styles.notch} />
              <div className={styles.screen}>
                <div className={styles.screenHeader} style={{ background: `${screen.color}15` }}>
                  <span className={styles.screenIcon}>{screen.icon}</span>
                  <span className={styles.screenLabel}>{screen.label}</span>
                </div>
                <div className={styles.mockContent}>
                  <div className={styles.mockBar} style={{ width: '80%' }} />
                  <div className={styles.mockBar} style={{ width: '60%' }} />
                  <div className={styles.mockBar} style={{ width: '90%' }} />
                  <div className={styles.mockCircle} style={{ background: screen.color }} />
                </div>
              </div>
            </div>
            <div className={styles.glow} style={{ background: `radial-gradient(circle, ${screen.color}20 0%, transparent 70%)` }} />
          </motion.div>
        ))}
      </div>
    </section>
  );
}
