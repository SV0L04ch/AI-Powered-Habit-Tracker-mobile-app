import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import styles from './RegisterPage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Input from '../../components/Input/Input';
import Typography from '../../components/Typography/Typography';
import useAuthUser from '../../store/useAuthStore';
import PageLayout from '../../components/PageLayout/PageLayout'
import { useEffect } from 'react';

const RegisterPage = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', city: '', password: '', confirm: '' });
  const [errors, setErrors] = useState({});
  const error = useAuthUser((state) => state.error)
  const isLoading = useAuthUser((state) => state.isLoading)
  const register = useAuthUser((state) => state.register)

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    if (errors[e.target.name]) setErrors({ ...errors, [e.target.name]: '' });
  };

  const clearError = useAuthUser((state) => state.clearError)
  useEffect(() => {
    clearError()
  }, [])

  const validate = () => {
    const newErrors = {};
    if (!form.email) newErrors.email = 'Эл. почта обязательна';
    else if (!/\S+@\S+\.\S+/.test(form.email)) newErrors.email = 'Неверный email';
    if (!form.city) newErrors.city = 'Выберите город';
    if (!form.password) newErrors.password = 'Пароль обязателен';
    else if (form.password.length < 6) newErrors.password = 'Минимум 6 символов';
    if (form.password !== form.confirm) newErrors.confirm = 'Пароли не совпадают';
    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const newErrors = validate();

    if (Object.keys(newErrors).length) {
      setErrors(newErrors);
      return;
    }

    await register(form.email, form.city, form.password)
    console.log(useAuthUser.getState())
  
    const email = useAuthUser.getState().email
    const error = useAuthUser.getState().error;
    
    if (!error){
      navigate('/login', {replace: true})
    }

    
  };

  const cities = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань', 'Нижний Новгород', 'Красноярск'];

  return (
    <PageLayout className={styles.centeredPage}>
      <div className={styles.circle1}></div>
      <div className={styles.circle2}></div>
      <div className={styles.circle3}></div>
      <div className={styles.circle4}></div>
      <div className={styles.page}>
        <Typography variant="headline1" className={styles.auth}>Регистрация</Typography>
        <Substrate variant="form" >
          <form onSubmit={handleSubmit} data-testid="register-form">
            <div className={styles.form}>
              <Input
                name="email"
                type="email"
                placeholder="Эл. почта"
                value={form.email}
                onChange={handleChange}
                data-testid="email-input"
              />
              {errors.email && <p className={styles.error} data-testid="error-email">{errors.email}</p>}

              <select
                name="city"
                value={form.city}
                onChange={handleChange}
                className={styles.selectSpacing}
                data-testid="city-select"
              >
                <option value="" disabled>Город</option>
                {cities.map(c => <option key={c} value={c}>{c}</option>)}
              </select>
              {errors.city && <p className={styles.error} data-testid="city-error">{errors.city}</p>}

              <Input
                name="password"
                type="password"
                placeholder="Пароль"
                value={form.password}
                onChange={handleChange}
                data-testid="input-password"
              />
              {errors.password && <p className={styles.error} data-testid="password-error">{errors.password}</p>}

              <Input
                name="confirm"
                type="password"
                placeholder="Подтвердить пароль"
                value={form.confirm}
                onChange={handleChange}
                data-testid="confPass-input"
              />
              {errors.confirm && <p className={styles.error} data-testid="confPass-error">{errors.confirm}</p>}
              {error && <p className={styles.error} data-testid="server-error">{error}</p>}
              <div className={styles.Buttons}>
                <Button variant="form" disabled={isLoading} type="submit" data-testid="reg-button">Регистрация</Button>
                <Link to="/login" className={styles.link} data-testid="auth-link">Авторизация</Link>
              </div>
            </div>
            
          </form>
          
        </Substrate>
      </div>
    </PageLayout>
  );
};

export default RegisterPage;