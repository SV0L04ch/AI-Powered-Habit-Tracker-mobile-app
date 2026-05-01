import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import styles from './ProfilePage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import icons from '../../lib/icons';
import illustrations from '../../assets/images/illustrations/avatar.png';




const ProfilePage = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [city, setCity] = useState('');
  const [darkTheme, setDarkTheme] = useState(false);
  const [reportTime, setReportTime] = useState('08:00');
  const [citiesList] = useState(['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань']);

  useEffect(() => {
    const currentUser = JSON.parse(localStorage.getItem('currentUser'));
    if (!currentUser || !currentUser.email) {
      navigate('/login');
      return;
    }
    setUser(currentUser);
    setCity(currentUser.city || 'Москва');
    
    // Загружаем настройки из localStorage
    const savedDark = localStorage.getItem('darkTheme') === 'true';
    const savedTime = localStorage.getItem('reportTime') || '08:00';
    setDarkTheme(savedDark);
    setReportTime(savedTime);
    // применяем тему (опционально)
    if (savedDark) document.body.classList.add('dark-theme');
    else document.body.classList.remove('dark-theme');
  }, [navigate]);

  const handleCityChange = (e) => {
    const newCity = e.target.value;
    setCity(newCity);
    // обновляем город в currentUser (если нужно)
    const updatedUser = { ...user, city: newCity };
    localStorage.setItem('currentUser', JSON.stringify(updatedUser));
    // также обновляем в массиве users
    const users = JSON.parse(localStorage.getItem('users') || '[]');
    const userIndex = users.findIndex(u => u.email === user.email);
    if (userIndex !== -1) users[userIndex].city = newCity;
    localStorage.setItem('users', JSON.stringify(users));
  };

  const toggleDarkTheme = () => {
    const newValue = !darkTheme;
    setDarkTheme(newValue);
    localStorage.setItem('darkTheme', newValue);
    if (newValue) document.body.classList.add('dark-theme');
    else document.body.classList.remove('dark-theme');
  };

  const handleReportTimeChange = (e) => {
    const newTime = e.target.value;
    setReportTime(newTime);
    localStorage.setItem('reportTime', newTime);
    
  };

  const handleLogout = () => {
    localStorage.removeItem('currentUser');
    navigate('/login');
  };

  if (!user) return <div className={styles.page}>Загрузка...</div>;

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>Профиль</Typography>
      <img src={illustrations} alt="Аватар" className={styles.avatarImage} />
      <Typography variant="headline2" className={styles.name}>Алексей Волков</Typography>
      <Typography variant="headline3" className={styles.sectionTitle}>Персонализация</Typography>
     
     <div className={styles.settingsContainer}>
      
      <Substrate className={styles.settingItem}>
        <div className={styles.reportRow}>
            <icons.Notification className={styles.icon} />
            <div className={styles.reportInfo}>
                <Typography variant="body1" className={styles.settingLabel}>Ежедневный отчёт</Typography>
                <Typography variant="caption" className={styles.settingHint}>Напоминание о прогрессе</Typography>
            </div>
            <input
                type="time"
                value={reportTime}
                onChange={handleReportTimeChange}
                className={styles.timePicker}
            />
        </div>
      </Substrate>

      
      <Substrate className={styles.settingItem}>
        <div className={styles.settingRow}>
        <icons.MapPoint className={styles.icon} />
          <Typography variant="body1" className={styles.settingLabel}>Ваш город</Typography>
          <select value={city} onChange={handleCityChange} className={styles.citySelect}>
            {citiesList.map(c => (
              <option key={c} value={c}>{c}</option>
            ))}
          </select>
        </div>
      </Substrate>

      
      <Substrate className={styles.settingItem}>
        <div className={styles.settingRow}>
        <icons.Moon className={styles.icon} />
          <Typography variant="body1" className={styles.settingLabel}>Тёмная тема</Typography>
          <label className={styles.switch}>
            <input type="checkbox" checked={darkTheme} onChange={toggleDarkTheme} />
            <span className={styles.slider}></span>
          </label>
        </div>
      </Substrate>

        <Button variant="form" className={styles.logoutButton} onClick={handleLogout}>
          ВЫХОД
        </Button>
           
     </div>
    </div>
  );
};

export default ProfilePage;