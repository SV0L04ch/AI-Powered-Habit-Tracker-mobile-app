import { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './SocialProof.module.scss';

const stats = [
  { value: 10000, suffix: '+', label: 'Habits Tracked' },
  { value: 500, suffix: '+', label: 'Active Users' },
  { value: 500000, suffix: '+', label: 'Completions' },
  { value: 1000, suffix: '+', label: 'Streaks Achieved' },
];

function AnimatedCounter({ target, suffix, inView }) {
  const [count, setCount] = useState(0);

  useEffect(() => {
    if (!inView) return;
    const duration = 2000;
    const steps = 60;
    const increment = target / steps;
    let current = 0;
    const timer = setInterval(() => {
      current += increment;
      if (current >= target) {
        setCount(target);
        clearInterval(timer);
      } else {
        setCount(Math.floor(current));
      }
    }, duration / steps);
    return () => clearInterval(timer);
  }, [inView, target]);

  const formatted = count >= 1000 ? `${(count / 1000).toFixed(count >= 10000 ? 0 : 1)}K` : count;

  return (
    <span className={styles.statNumber}>
      {formatted}{suffix}
    </span>
  );
}

const container = {
  hidden: {},
  show: { transition: { staggerChildren: 0.1 } },
};

const item = {
  hidden: { opacity: 0, y: 30 },
  show: { opacity: 1, y: 0, transition: { duration: 0.5 } },
};

const testimonials = [
  { name: 'Alex K.', text: 'This app helped me build a 30-day meditation streak. The AI insights are incredible!', role: 'Developer' },
  { name: 'Maria S.', text: 'Love the gamification! The streaks and achievements keep me motivated every day.', role: 'Designer' },
  { name: 'James L.', text: 'Finally an app that understands habit building. The weather integration is genius.', role: 'Student' },
];

export default function SocialProof() {
  const [ref, inView] = useInView({ threshold: 0.3, triggerOnce: true });

  return (
    <section className={styles.socialProof} ref={ref}>
      <div className={styles.container}>
        <motion.div
          className={styles.header}
          initial={{ opacity: 0, y: 30 }}
          animate={inView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
        >
          <span className={styles.label}>Trusted by Thousands</span>
          <h2 className={styles.title}>Real Results, Real People</h2>
        </motion.div>

        <motion.div
          className={styles.statsGrid}
          variants={container}
          initial="hidden"
          animate={inView ? 'show' : 'hidden'}
        >
          {stats.map((stat, i) => (
            <motion.div key={i} className={styles.statCard} variants={item}>
              <AnimatedCounter target={stat.value} suffix={stat.suffix} inView={inView} />
              <span className={styles.statLabel}>{stat.label}</span>
            </motion.div>
          ))}
        </motion.div>

        <motion.div
          className={styles.testimonials}
          variants={container}
          initial="hidden"
          animate={inView ? 'show' : 'hidden'}
        >
          {testimonials.map((t, i) => (
            <motion.div key={i} className={styles.testimonialCard} variants={item}>
              <div className={styles.stars}>{'\u2605\u2605\u2605\u2605\u2605'}</div>
              <p className={styles.testimonialText}>{'\u201C'}{t.text}{'\u201D'}</p>
              <div className={styles.author}>
                <div className={styles.avatar}>{t.name[0]}</div>
                <div>
                  <div className={styles.authorName}>{t.name}</div>
                  <div className={styles.authorRole}>{t.role}</div>
                </div>
              </div>
            </motion.div>
          ))}
        </motion.div>
      </div>
    </section>
  );
}
