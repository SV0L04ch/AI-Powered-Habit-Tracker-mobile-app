import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Outlet } from 'react-router-dom';
import styles from './LandingLayout.module.scss';

const navLinks = [
  { to: '/features', label: 'Features' },
  { to: '/pricing', label: 'Pricing' },
  { to: '/about', label: 'About' },
];

export default function LandingLayout() {
  const [menuOpen, setMenuOpen] = useState(false);
  const location = useLocation();

  return (
    <div className={styles.layout}>
      <header className={styles.header}>
        <div className={styles.headerInner}>
          <Link to="/" className={styles.logo}>
            <div className={styles.logoIcon}>✓</div>
            <span className={styles.logoText}>Flowstate</span>
          </Link>

          <nav className={styles.nav}>
            {navLinks.map(link => (
              <Link
                key={link.to}
                to={link.to}
                className={`${styles.navLink} ${location.pathname.startsWith(link.to) ? styles.active : ''}`}
              >
                {link.label}
              </Link>
            ))}
          </nav>

          <div className={styles.actions}>
            <Link to="/login" className={styles.signInBtn}>Sign in</Link>
            <Link to="/register" className={styles.getStartedBtn}>Get Started</Link>
          </div>

          <button className={styles.hamburger} onClick={() => setMenuOpen(!menuOpen)}>
            <span className={`${styles.hamburgerLine} ${menuOpen ? styles.open : ''}`} />
            <span className={`${styles.hamburgerLine} ${menuOpen ? styles.open : ''}`} />
            <span className={`${styles.hamburgerLine} ${menuOpen ? styles.open : ''}`} />
          </button>
        </div>

        {menuOpen && (
          <div className={styles.mobileMenu}>
            {navLinks.map(link => (
              <Link key={link.to} to={link.to} className={styles.mobileLink} onClick={() => setMenuOpen(false)}>
                {link.label}
              </Link>
            ))}
            <div className={styles.mobileActions}>
              <Link to="/login" className={styles.signInBtn} onClick={() => setMenuOpen(false)}>Sign in</Link>
              <Link to="/register" className={styles.getStartedBtn} onClick={() => setMenuOpen(false)}>Get Started</Link>
            </div>
          </div>
        )}
      </header>

      <main className={styles.main}>
        <Outlet />
      </main>

      <footer className={styles.footer}>
        <div className={styles.footerInner}>
          <div className={styles.footerBrand}>
            <div className={styles.logo}>
              <div className={styles.logoIcon}>✓</div>
              <span className={styles.logoText}>Flowstate</span>
            </div>
            <p className={styles.footerDesc}>Build better habits with AI-powered insights.</p>
          </div>
          <div className={styles.footerLinks}>
            <div className={styles.footerCol}>
              <h4>Product</h4>
              <Link to="/features">Features</Link>
              <Link to="/pricing">Pricing</Link>
              <Link to="/templates">Templates</Link>
            </div>
            <div className={styles.footerCol}>
              <h4>Company</h4>
              <Link to="/about">About</Link>
              <a href="#">Blog</a>
              <a href="#">Careers</a>
            </div>
            <div className={styles.footerCol}>
              <h4>Support</h4>
              <a href="#">Help Center</a>
              <a href="#">Contact</a>
              <a href="#">Privacy</a>
            </div>
          </div>
        </div>
        <div className={styles.footerBottom}>
          <span>&copy; 2026 Flowstate. All rights reserved.</span>
        </div>
      </footer>
    </div>
  );
}
