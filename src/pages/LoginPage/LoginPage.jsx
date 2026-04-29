import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import styles from './LoginPage.module.scss'
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Input from '../../components/Input/Input';
import Typography from '../../components/Typography/Typography';



const LoginPage = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setError('');
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const { email, password } = form;

    if (!email || !password) {
      setError('Заполните все поля');
      return;
    }

    
    const users = JSON.parse(localStorage.getItem('users') || '[]');
    const user = users.find(u => u.email === email && u.password === password);

    if (!user) {
      setError('Неверный email или пароль');
      return;
    }

    
    localStorage.setItem('currentUser', JSON.stringify({ email: user.email, city: user.city }));
    console.log('Вошёл:', user);
    navigate('/habits');
  };

  const errorButton = `${styles.button} ${styles.error}`

  return (
    <div className={styles.page}>
      <div className={styles.circle1}></div>
      <div className={styles.circle2}></div>
      <div className={styles.circle3}></div>
      <div className={styles.circle4}></div>
      <Typography variant='headline1' className={styles.auth}>Авторизация</Typography>
      <Substrate variant='form' >
        <form onSubmit={handleSubmit}>
      <Input className={styles.inputSpacing}
      type="email"
      placeholder="Эл. почта"
      value={form.email}
      onChange={handleChange}
      />
      <Input className={styles.inputSpacing}
      name="password"
      type="password"
      placeholder="Пароль"
      value={form.password}
      onChange={handleChange}
      />

          {error && <div className={styles.error}>{error}</div>}
          <Button type="submit" variant='form' className={styles.submitButton}>Войти</Button>
        </form>
        <Link to="/register" className={styles.link}>Регистрация</Link>
        </Substrate>
      </div>
  );
};


export default LoginPage;