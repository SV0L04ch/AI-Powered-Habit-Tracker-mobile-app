import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './CtaSection.module.scss';

export default function CtaSection() {
  const navigate = useNavigate();
  const [ref, inView] = useInView({ threshold: 0.3, triggerOnce: true });

  return (
    <section className={styles.cta} ref={ref}>
      <div className={styles.bgGlow} />
      <motion.div
        className={styles.content}
        initial={{ opacity: 0, y: 40 }}
        animate={inView ? { opacity: 1, y: 0 } : {}}
        transition={{ duration: 0.7, ease: [0.25, 1.2, 0.5, 1] }}
      >
        <h2 className={styles.title}>Start Your Journey Today</h2>
        <p className={styles.subtitle}>
          Free forever. No credit card required. Join thousands building better habits.
        </p>
        <div className={styles.actions}>
          <button className={styles.ctaPrimary} onClick={() => navigate('/register')}>
            Create Free Account
          </button>
          <button className={styles.ctaGhost} onClick={() => navigate('/login')}>
            Sign In
          </button>
        </div>
      </motion.div>
    </section>
  );
}
