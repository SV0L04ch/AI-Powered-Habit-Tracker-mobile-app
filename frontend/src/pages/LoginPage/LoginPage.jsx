import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import Button from '../../components/Button/Button';
import Input from '../../components/Input/Input';
import Typography from '../../components/Typography/Typography';
import PageLayout from '../../components/PageLayout/PageLayout';
import useAuthUser from '../../store/useAuthStore';
import styles from './LoginPage.module.scss';

const LoginPage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const savedEmail = localStorage.getItem('remembered-email') || '';
  const [form, setForm] = useState({ email: savedEmail, password: '' });
  const [remember, setRemember] = useState(Boolean(savedEmail));
  const [error, setError] = useState('');
  const login = useAuthUser((state) => state.login);
  const isLoading = useAuthUser((state) => state.isLoading);
  const serverError = useAuthUser((state) => state.error);
  const clearError = useAuthUser((state) => state.clearError);

  useEffect(() => {
    clearError();
  }, [clearError]);

  const handleChange = (event) => {
    setForm({ ...form, [event.target.name]: event.target.value });
    setError('');
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (!form.email || !form.password) {
      setError(t('auth.fillEmailPassword'));
      return;
    }

    const data = await login(form.email, form.password);
    if (data) {
      if (remember) localStorage.setItem('remembered-email', form.email);
      else localStorage.removeItem('remembered-email');
      navigate('/habits');
    }
  };

  return (
    <PageLayout className={styles.centeredPage} data-testid="login-page">
      <section className={styles.hero} data-testid="login-hero">
        <span className={styles.kicker} data-testid="login-kicker">
          AI-Powered Habit Tracker
        </span>
        <Typography variant="headline1" className={styles.title} data-testid="login-title">
          {t('auth.loginTitle')}
        </Typography>
        <Typography variant="body1" className={styles.subtitle} data-testid="login-subtitle">
          {t('auth.loginSubtitle')}
        </Typography>
      </section>

      <section className={styles.card} data-testid="login-form-card">
        <form onSubmit={handleSubmit} className={styles.form} data-testid="login-form">
          <Input
            name="email"
            type="email"
            label="Email"
            value={form.email}
            onChange={handleChange}
            autoComplete="email"
            data-testid="email-input"
          />
          <Input
            name="password"
            type="password"
            label={t('auth.password')}
            value={form.password}
            onChange={handleChange}
            autoComplete="current-password"
            data-testid="password-input"
          />

          <label className={styles.remember} data-testid="remember-me-row">
            <input
              type="checkbox"
              checked={remember}
              onChange={(event) => setRemember(event.target.checked)}
              data-testid="remember-me-checkbox"
            />
            <span data-testid="remember-me-label">{t('auth.rememberMe')}</span>
          </label>

          {error && (
            <p className={styles.error} data-testid="validate-error">
              {error}
            </p>
          )}
          {serverError && (
            <p className={styles.error} data-testid="server-error">
              {serverError}
            </p>
          )}

          <Button type="submit" variant="form" loading={isLoading} data-testid="login-button">
            Войти
          </Button>
          <Link to="/register" className={styles.link} data-testid="register-link">
            Создать аккаунт
          </Link>
        </form>
      </section>
    </PageLayout>
  );
};

export default LoginPage;
