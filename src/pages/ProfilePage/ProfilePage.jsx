import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import styles from './ProfilePage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import icons from '../../lib/icons';
import useAuthUser from '../../store/useAuthStore';
// import illustrations from '../../assets/images/illustrations/avatar.png';




const ProfilePage = () => {
  const navigate = useNavigate();
  const isLoading = useAuthUser((state) => state.isLoading)
  const [user, setUser] = useState(null);
  const [city, setCity] = useState('');
  const [darkTheme, setDarkTheme] = useState(false);
  const [reportTime, setReportTime] = useState('08:00');
  const [citiesList] = useState(['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань']);
  const logOut = useAuthUser((state) => state.logout)



  const handleCityChange = (e) => {
    const newCity = e.target.value;
    setCity(newCity);
    // обновляем город в currentUser (если нужно)
    const updatedUser = { ...user, city: newCity };
    localStorage.setItem('currentUser', JSON.stringify(updatedUser));
    // также обновляем в массиве users
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

  const handleLogout = async () => {
    await logOut()
    navigate('/login');
  };

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>Профиль</Typography>
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

        <Button variant="form" className={styles.logoutButton} onClick={handleLogout} disabled={isLoading}>
          {isLoading ? "Выход..." : "Выход"}
        </Button>
           
     </div>
    </div>
  );
};

export default ProfilePage;