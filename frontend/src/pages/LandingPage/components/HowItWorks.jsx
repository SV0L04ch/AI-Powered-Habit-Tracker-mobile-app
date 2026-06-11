import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './HowItWorks.module.scss';

const steps = [
  {
    number: '01',
    title: 'Create',
    description: 'Set up your habits in seconds with smart templates and AI suggestions.',
    icon: '✨',
  },
  {
    number: '02',
    title: 'Track',
    description: 'Mark completion with a single tap. AI handles the rest.',
    icon: '📋',
  },
  {
    number: '03',
    title: 'Grow',
    description: 'Watch your streaks grow and discover insights about yourself.',
    icon: '🌱',
  },
];

const container = {
  hidden: {},
  show: { transition: { staggerChildren: 0.2 } },
};

const step = {
  hidden: { opacity: 0, x: -50 },
  show: {
    opacity: 1,
    x: 0,
    transition: { duration: 0.7, ease: [0.25, 1.2, 0.5, 1] },
  },
};

export default function HowItWorks() {
  const [ref, inView] = useInView({ threshold: 0.2, triggerOnce: true });

  return (
    <section className={styles.howItWorks} ref={ref}>
      <div className={styles.container}>
        <motion.div
          className={styles.header}
          initial={{ opacity: 0, y: 30 }}
          animate={inView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
        >
          <span className={styles.label}>How It Works</span>
          <h2 className={styles.title}>Three Simple Steps</h2>
          <p className={styles.subtitle}>
            Start building better habits in minutes, not hours.
          </p>
        </motion.div>

        <motion.div
          className={styles.steps}
          variants={container}
          initial="hidden"
          animate={inView ? 'show' : 'hidden'}
        >
          {steps.map((s, i) => (
            <motion.div key={i} className={styles.stepCard} variants={step}>
              <div className={styles.stepNumber}>{s.number}</div>
              <div className={styles.stepIcon}>{s.icon}</div>
              <h3 className={styles.stepTitle}>{s.title}</h3>
              <p className={styles.stepDescription}>{s.description}</p>
              {i < steps.length - 1 && <div className={styles.connector} />}
            </motion.div>
          ))}
        </motion.div>
      </div>
    </section>
  );
}
