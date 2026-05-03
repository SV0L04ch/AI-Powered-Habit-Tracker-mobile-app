import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import styles from './LoginPage.module.scss'
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Input from '../../components/Input/Input';
import Typography from '../../components/Typography/Typography';
import useAuthUser from '../../store/useAuthStore';
import PageLayout from '../../components/PageLayout/PageLayout';
import { useEffect } from 'react';


const LoginPage = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const login = useAuthUser((state) => state.login)
  const isLoading = useAuthUser((state) => state.isLoading)
  const aErorr = useAuthUser((state) => state.error)

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setError('');
  };

  const clearError = useAuthUser((state) => state.clearError)
  useEffect(() => {
    clearError()
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault();
    const { email, password } = form;

    

    if (!email || !password) {
      setError('Заполните все поля');
      return;
    }
    
    await login(email, password)
    
    const isAuth = useAuthUser.getState().isAuthenticated
    if (isAuth){
      navigate('/habits');
    }
    
    
  };

  const errorButton = `${styles.button} ${styles.error}`

  return (
    <PageLayout className={styles.centeredPage}>
      <div className={styles.circle1} />
      <div className={styles.circle2} />
      <div className={styles.circle3} />
      <div className={styles.circle4} />
      <div className={styles.page}>
      <Typography variant='headline1' className={styles.auth}>Авторизация</Typography>
      <Substrate variant='form' >
        <form onSubmit={handleSubmit}>
          <div className={styles.form}>
          
      <Input
      name="email"
      type="email"
      placeholder="Эл. почта"
      value={form.email}
      onChange={handleChange}
      />
      <Input
      name="password"
      type="password"
      placeholder="Пароль"
      value={form.password}
      onChange={handleChange}
      />

          {error && <p className={styles.error}>{error}</p>}
          {aErorr && <p className={styles.error}>{aErorr}</p>}
          <div className={styles.Buttons}>
            <Button type="submit" variant='form' className={styles.submitButton} disabled={isLoading}>Войти</Button>
            <Link to="/register" className={styles.link}>Регистрация</Link>
          </div>
          </div>
        </form>
        </Substrate>
        </div>
          
        
      </PageLayout>
  );
};


export default LoginPage;