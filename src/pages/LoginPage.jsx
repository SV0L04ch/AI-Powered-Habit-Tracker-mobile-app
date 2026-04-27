import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';

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

  return (
    <div style={styles.container}>
      <div style={styles.circle1}></div>
      <div style={styles.circle2}></div>
      <div style={styles.circle3}></div>
      <div style={styles.circle4}></div>
      <h1 style={styles.title}>Авторизация</h1>
      <div style={styles.card}>
        <form onSubmit={handleSubmit}>
          <input
            name="email"
            placeholder="Эл. почта"
            value={form.email}
            onChange={handleChange}
            style={styles.input}
          />
          <input
            name="password"
            type="password"
            placeholder="Пароль"
            value={form.password}
            onChange={handleChange}
            style={styles.input}
          />
          {error && <div style={styles.error}>{error}</div>}
          <button type="submit" style={styles.button}>Войти</button>
        </form>
        <Link to="/register" style={styles.link}>Регистрация</Link>
      </div>
    </div>
  );
};

const styles = {
  container: {
    width: '402px',
    height: '874px',
    background: '#133348',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '20px',
    fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, sans-serif",
    position: 'relative',
    overflow: 'hidden',
  },

  circle1: {
  position: 'absolute',
  width: '108px',
  height: '108px',
  left: '94px',
  top: '-36px',
  borderRadius: '50%',          
  background: 'linear-gradient(180deg, #D69A85 0%, #3A5A6E 100%)',        
  pointerEvents: 'none',        
  zIndex: 0,                    
  boxShadow: '0px 0px 16px 1px rgba(0, 0, 0, 0.2)',
  },
  
  circle2: {
  position: 'absolute',
  width: '217px',
  height: '217px',
  left: '281px',
  top: '-68px',
  borderRadius: '50%',
  background: 'linear-gradient(180deg, #D69A85 0%, #3A5A6E 100%)',
  boxShadow: '0px 0px 16px 1px rgba(0, 0, 0, 0.2)',
  pointerEvents: 'none',
  zIndex: 0,
  },

  circle3: {
  position: 'absolute',
  width: '217px',
  height: '217px',
  left: '-85px',
  top: '600px',
  borderRadius: '50%',
  background: 'linear-gradient(180deg, #D69A85 0%, #3A5A6E 100%)',
  boxShadow: '0px 0px 16px 1px rgba(0, 0, 0, 0.2)',
  pointerEvents: 'none',
  zIndex: 0,
  },
  
  circle4: {
  position: 'absolute',
  width: '108px',
  height: '108px',
  left: '360px',
  top: '840px',
  borderRadius: '50%',
  background: 'linear-gradient(180deg, #D69A85 0%, #3A5A6E 100%)',
  boxShadow: '0px 0px 16px 1px rgba(0, 0, 0, 0.2)',
  pointerEvents: 'none',     
  zIndex: 0,     
  },
  
  title: {
  marginTop: '-182px',
  fontSize: '32px',
  fontWeight: '700',
  color: '#ffffff',
  marginBottom: '10px',
  textAlign: 'center',
  letterSpacing: '-0.5px',
  textShadow: '0 2px 4px rgba(0,0,0,0.1)',
  zIndex: 1,
  },
  
  card: {
  width: '100%',
  maxWidth: '354px',
  background: 'linear-gradient(302.46deg, #D69A85 0.26%, #3A5A6E 50%)',
  borderRadius: '32px',
  padding: '40px 28px',
  backdropFilter: 'blur(4px)',
  boxShadow: '0 20px 40px rgba(0,0,0,0.1)',
  textAlign: 'center',
  zIndex: 1,
  },
  
  input: {  
  placeholder: '#769DB7',
  width: '100%',
  height: '39px',                
  padding: '0 20px',           
  marginBottom: '30px',         
  fontSize: '16px',
  border: '1.5px solid #A0522D',
  borderRadius: '16px',
  outline: 'none',
  background: '#2A3E4C',
  fontWeight: '600',
  transition: '0.2s',
  color: '#ffffff',
  boxSizing: 'border-box',      
  lineHeight: '39px',           
  },

  error: {
  fontSize: '13px',
  color: '#ff3b30',
  marginTop: '-12px',
  marginBottom: '12px',
  textAlign: 'left',
  },
  button: {
  marginBottom: '15px',
  marginTop: '1px',
  boxSizing: 'border-box',
  padding: '10px 20px',
  width: '195px',
  height: '44px',
  background: '#A0522D',
  border: '1.5px solid #A0522D',
  boxShadow: '0px 0px 16px 1px rgba(0, 0, 0, 0.2)',
  borderRadius: '16px',
  marginBottom: '0px',
  cursor: 'pointer',
  fontSize: '20px',
  fontWeight: '600',
  color: '#ffffff',
  transition: '0.2s',
  },
  
  link: {
    display: 'block',
    marginTop: '15px',
    fontSize: '20px',
    color: '#ffffff',
    textDecoration: 'none',
    fontWeight: '600',
  },

  
};

export default LoginPage;