import { motion } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import styles from './FeaturesSection.module.scss';

const features = [
  {
    icon: '✅',
    title: 'Smart Tracking',
    description: 'Track positive and negative habits with AI-powered suggestions and smart scheduling.',
  },
  {
    icon: '🔥',
    title: 'Streaks & Gamification',
    description: 'Build momentum with streaks, XP, levels, and achievement badges.',
  },
  {
    icon: '🧠',
    title: 'AI Insights',
    description: 'Get personalized daily summaries, trend analysis, and coaching tips.',
  },
  {
    icon: '🌤️',
    title: 'Weather Integration',
    description: 'See how weather affects your habit completion with smart correlations.',
  },
  {
    icon: '📊',
    title: 'City Statistics',
    description: 'Compare your habits with your city\'s popular habits anonymously.',
  },
  {
    icon: '📴',
    title: 'Offline First',
    description: 'Works offline. Syncs when you\'re back online. Never lose progress.',
  },
];

const container = {
  hidden: {},
  show: { transition: { staggerChildren: 0.1 } },
};

const card = {
  hidden: { opacity: 0, y: 50, scale: 0.95 },
  show: {
    opacity: 1,
    y: 0,
    scale: 1,
    transition: { duration: 0.6, ease: [0.25, 1.2, 0.5, 1] },
  },
};

export default function FeaturesSection() {
  const [ref, inView] = useInView({ threshold: 0.1, triggerOnce: true });

  return (
    <section className={styles.features} ref={ref}>
      <div className={styles.container}>
        <motion.div
          className={styles.header}
          initial={{ opacity: 0, y: 30 }}
          animate={inView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
        >
          <span className={styles.label}>Features</span>
          <h2 className={styles.title}>Everything You Need</h2>
          <p className={styles.subtitle}>
            Powerful tools to help you build lasting habits and achieve your goals.
          </p>
        </motion.div>

        <motion.div
          className={styles.grid}
          variants={container}
          initial="hidden"
          animate={inView ? 'show' : 'hidden'}
        >
          {features.map((feature, i) => (
            <motion.div key={i} className={styles.card} variants={card}>
              <div className={styles.iconWrapper}>
                <span className={styles.icon}>{feature.icon}</span>
              </div>
              <h3 className={styles.cardTitle}>{feature.title}</h3>
              <p className={styles.cardDescription}>{feature.description}</p>
            </motion.div>
          ))}
        </motion.div>
      </div>
    </section>
  );
}
