import { Link } from 'react-router-dom';
import styles from './PricingPage.module.scss';

const plans = [
  { name: 'Free', price: '$0', period: 'forever', features: ['Unlimited habits', 'Basic stats', '3 AI insights/day', 'City feed'], cta: 'Get Started', popular: false },
  { name: 'Pro', price: '$9', period: '/month', features: ['Everything in Free', 'Unlimited AI insights', 'Advanced analytics', 'Priority support', 'Custom themes'], cta: 'Start Pro Trial', popular: true },
  { name: 'Team', price: '$19', period: '/month', features: ['Everything in Pro', 'Team challenges', 'Admin dashboard', 'API access', 'Dedicated support'], cta: 'Contact Sales', popular: false },
];

export default function PricingPage() {
  return (
    <div className={styles.page}>
      <section className={styles.hero}>
        <h1>Simple, transparent <span className={styles.gradient}>pricing</span></h1>
        <p>Start free, upgrade when you need more power.</p>
      </section>
      <section className={styles.plans}>
        {plans.map((p) => (
          <div key={p.name} className={`${styles.card} ${p.popular ? styles.popular : ''}`}>
            {p.popular && <div className={styles.popularBadge}>Most Popular</div>}
            <h3>{p.name}</h3>
            <div className={styles.price}>{p.price}<span>{p.period}</span></div>
            <ul className={styles.features}>
              {p.features.map((f, i) => (
                <li key={i}>✓ {f}</li>
              ))}
            </ul>
            <Link to="/register" className={styles.ctaBtn}>{p.cta}</Link>
          </div>
        ))}
      </section>
    </div>
  );
}
