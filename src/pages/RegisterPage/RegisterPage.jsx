import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import styles from './RegisterPage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Input from '../../components/Input/Input';
import Typography from '../../components/Typography/Typography';

const RegisterPage = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', city: '', password: '', confirm: '' });
  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    if (errors[e.target.name]) setErrors({ ...errors, [e.target.name]: '' });
  };

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

  const handleSubmit = (e) => {
    e.preventDefault();
    const newErrors = validate();
    if (Object.keys(newErrors).length) {
      setErrors(newErrors);
      return;
    }
    
    const users = JSON.parse(localStorage.getItem('users') || '[]');
    if (users.some(u => u.email === form.email)) {
      setErrors({ email: 'Пользователь уже существует' });
      return;
    }
    const newUser = { id: Date.now(), email: form.email, city: form.city, password: form.password };
    users.push(newUser);
    localStorage.setItem('users', JSON.stringify(users));
    localStorage.setItem('currentUser', JSON.stringify({ email: newUser.email, city: newUser.city }));
    navigate('/habits');
  };

  const cities = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань', 'Нижний Новгород', 'Красноярск'];

  return (
    <div className={styles.page}>
      <div className={styles.circle1}></div>
      <div className={styles.circle2}></div>
      <div className={styles.circle3}></div>
      <div className={styles.circle4}></div>
      <Typography variant="headline1" className={styles.auth}>Регистрация</Typography>
      <Substrate variant="form" >
        <form onSubmit={handleSubmit}>
          <Input
            name="email"
            type="email"
            placeholder="Эл. почта"
            value={form.email}
            onChange={handleChange}
            className={styles.inputSpacing}
          />
          {errors.email && <div className={styles.error}>{errors.email}</div>}

          <select
            name="city"
            value={form.city}
            onChange={handleChange}
            className={styles.selectSpacing}
          >
            <option value="" disabled>Город</option>
            {cities.map(c => <option key={c} value={c}>{c}</option>)}
          </select>
          {errors.city && <div className={styles.error}>{errors.city}</div>}

          <Input
            name="password"
            type="password"
            placeholder="Пароль"
            value={form.password}
            onChange={handleChange}
            className={styles.inputSpacing}
          />
          {errors.password && <div className={styles.error}>{errors.password}</div>}

          <Input
            name="confirm"
            type="password"
            placeholder="Подтвердить пароль"
            value={form.confirm}
            onChange={handleChange}
            className={styles.inputSpacing}
          />
          {errors.confirm && <div className={styles.error}>{errors.confirm}</div>}

          <Button type="submit" variant="form" className={styles.submitButton}>Регистрация</Button>
        </form>
        <Link to="/login" className={styles.link}>Авторизация</Link>
      </Substrate>
    </div>
  );
};

export default RegisterPage;