import styles from './LandingFooter.module.scss';

export default function LandingFooter() {
  return (
    <footer className={styles.footer}>
      <div className={styles.container}>
        <div className={styles.top}>
          <div className={styles.brand}>
            <div className={styles.logo}>✦</div>
            <span className={styles.brandName}>Habit Tracker</span>
            <p className={styles.brandTagline}>Build better habits, one day at a time.</p>
          </div>

          <div className={styles.links}>
            <div className={styles.linkGroup}>
              <h4 className={styles.linkTitle}>Product</h4>
              <a href="#features" className={styles.link}>Features</a>
              <a href="#pricing" className={styles.link}>Pricing</a>
              <a href="#faq" className={styles.link}>FAQ</a>
            </div>
            <div className={styles.linkGroup}>
              <h4 className={styles.linkTitle}>Company</h4>
              <a href="#" className={styles.link}>About</a>
              <a href="#" className={styles.link}>Blog</a>
              <a href="#" className={styles.link}>Careers</a>
            </div>
            <div className={styles.linkGroup}>
              <h4 className={styles.linkTitle}>Support</h4>
              <a href="#" className={styles.link}>Help Center</a>
              <a href="#" className={styles.link}>Contact</a>
              <a href="#" className={styles.link}>Status</a>
            </div>
          </div>
        </div>

        <div className={styles.divider} />

        <div className={styles.bottom}>
          <p className={styles.copyright}>© 2026 AI-Powered Habit Tracker. All rights reserved.</p>
          <div className={styles.legal}>
            <a href="#" className={styles.legalLink}>Privacy Policy</a>
            <a href="#" className={styles.legalLink}>Terms of Service</a>
          </div>
        </div>
      </div>
    </footer>
  );
}
