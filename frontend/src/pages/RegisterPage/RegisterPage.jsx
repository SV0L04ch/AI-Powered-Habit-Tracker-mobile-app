import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Button from '../../components/Button/Button';
import Input from '../../components/Input/Input';
import Typography from '../../components/Typography/Typography';
import useAuthUser from '../../store/useAuthStore';
import PageLayout from '../../components/PageLayout/PageLayout';
import styles from './RegisterPage.module.scss';

const RegisterPage = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', city: '', password: '', confirm: '' });
  const [errors, setErrors] = useState({});
  const error = useAuthUser((state) => state.error);
  const isLoading = useAuthUser((state) => state.isLoading);
  const registrationMessage = useAuthUser((state) => state.registrationMessage);
  const register = useAuthUser((state) => state.register);
  const clearError = useAuthUser((state) => state.clearError);

  useEffect(() => {
    clearError();
  }, [clearError]);

  const handleChange = (event) => {
    setForm({ ...form, [event.target.name]: event.target.value });
    if (errors[event.target.name]) {
      setErrors({ ...errors, [event.target.name]: '' });
    }
  };

  const validate = () => {
    const nextErrors = {};
    if (!form.email) nextErrors.email = 'Укажите email.';
    else if (!/\S+@\S+\.\S+/.test(form.email)) nextErrors.email = 'Email выглядит некорректно.';
    else if (!form.email.toLowerCase().endsWith('@gmail.com')) nextErrors.email = 'Используйте Gmail адрес для подтверждения.';
    if (!form.city.trim()) nextErrors.city = 'Укажите город.';
    if (!form.password) nextErrors.password = 'Укажите пароль.';
    else if (form.password.length < 6) nextErrors.password = 'Минимум 6 символов.';
    if (form.password !== form.confirm) nextErrors.confirm = 'Пароли не совпадают.';
    return nextErrors;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const nextErrors = validate();
    if (Object.keys(nextErrors).length) {
      setErrors(nextErrors);
      return;
    }

    const data = await register(form.email, form.city, form.password);
    if (data) {
      window.setTimeout(() => navigate('/login', { replace: true }), 1200);
    }
  };

  return (
    <PageLayout className={styles.centeredPage} data-testid="register-page">
      <section className={styles.hero} data-testid="register-hero">
        <span className={styles.kicker} data-testid="register-kicker">
          Начните с малого
        </span>
        <Typography variant="headline1" className={styles.title} data-testid="register-title">
          Новый аккаунт
        </Typography>
        <Typography variant="body1" className={styles.subtitle} data-testid="register-subtitle">
          Город понадобится для погоды и городской сводки привычек.
        </Typography>
      </section>

      <section className={styles.card} data-testid="register-form-card">
        <form onSubmit={handleSubmit} className={styles.form} data-testid="register-form">
          <Input
            name="email"
            type="email"
            label="Email (Gmail)"
            value={form.email}
            onChange={handleChange}
            autoComplete="email"
            placeholder="yourname@gmail.com"
            data-testid="email-input"
          />
          {errors.email && (
            <p className={styles.error} data-testid="error-email">
              {errors.email}
            </p>
          )}

          <Input
            name="city"
            label="Город"
            value={form.city}
            onChange={handleChange}
            autoComplete="address-level2"
            data-testid="city-input"
          />
          {errors.city && (
            <p className={styles.error} data-testid="city-error">
              {errors.city}
            </p>
          )}

          <Input
            name="password"
            type="password"
            label="Пароль"
            value={form.password}
            onChange={handleChange}
            autoComplete="new-password"
            data-testid="input-password"
          />
          {errors.password && (
            <p className={styles.error} data-testid="password-error">
              {errors.password}
            </p>
          )}

          <Input
            name="confirm"
            type="password"
            label="Подтвердите пароль"
            value={form.confirm}
            onChange={handleChange}
            autoComplete="new-password"
            data-testid="confPass-input"
          />
          {errors.confirm && (
            <p className={styles.error} data-testid="confPass-error">
              {errors.confirm}
            </p>
          )}

          {registrationMessage && (
            <p className={styles.success} data-testid="registration-message">
              Проверьте почту и подтвердите аккаунт.
            </p>
          )}
          {error && (
            <p className={styles.error} data-testid="server-error">
              {error}
            </p>
          )}

          <Button variant="form" loading={isLoading} type="submit" data-testid="reg-button">
            Зарегистрироваться
          </Button>
          <Link to="/login" className={styles.link} data-testid="auth-link">
            Уже есть аккаунт
          </Link>
        </form>
      </section>
    </PageLayout>
  );
};

export default RegisterPage;
