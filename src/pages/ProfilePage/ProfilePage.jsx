import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './ProfilePage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import icons from '../../lib/icons';
import useAuthUser from '../../store/useAuthStore';
import useThemeStore from '../../store/useThemeStore';
import illustrations from '../../assets/images/illustrations/avatar.png';

const ProfilePage = () => {
  const navigate = useNavigate();
  const isLoading = useAuthUser((state) => state.isLoading);
  const logOut = useAuthUser((state) => state.logout);
  const { theme, toggleTheme } = useThemeStore(); // получаем глобальную тему

  const [user, setUser] = useState(null);
  const [city, setCity] = useState('');
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
    const savedTime = localStorage.getItem('reportTime') || '08:00';
    setReportTime(savedTime);
  }, [navigate]);

  const handleCityChange = (e) => {
    const newCity = e.target.value;
    setCity(newCity);
    const updatedUser = { ...user, city: newCity };
    localStorage.setItem('currentUser', JSON.stringify(updatedUser));
    const users = JSON.parse(localStorage.getItem('users') || '[]');
    const userIndex = users.findIndex(u => u.email === user.email);
    if (userIndex !== -1) users[userIndex].city = newCity;
    localStorage.setItem('users', JSON.stringify(users));
  };

  const handleReportTimeChange = (e) => {
    setReportTime(e.target.value);
    localStorage.setItem('reportTime', e.target.value);
  };

  const handleLogout = async () => {
    await logOut();
    navigate('/login');
  };

  if (!user) return <div className={styles.page}>Загрузка...</div>;

  return (
    <div className={styles.page}>
      <div className={styles.circle1}></div>
      <div className={styles.circle2}></div>
      <div className={styles.circle3}></div>
      <div className={styles.circle4}></div>

      <Typography variant="headline1" className={styles.title}>Профиль</Typography>
      <div className={styles.avatarContainer}>
        <img src={illustrations} alt="Аватар" className={styles.avatar} />
      </div>
      <Typography variant="headline2" className={styles.name}>Алексей Волков</Typography>
      <Typography variant="headline3" className={styles.sectionTitle}>Персонализация</Typography>

      <div className={styles.settingsContainer}>
        {/* Ежедневный отчёт */}
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

        {/* Город */}
        <Substrate className={styles.settingItem}>
          <div className={styles.settingRow}>
            <icons.MapPoint className={styles.icon} />
            <Typography variant="body1" className={styles.settingLabel}>Ваш город</Typography>
            <select value={city} onChange={handleCityChange} className={styles.citySelect}>
              {citiesList.map(c => <option key={c} value={c}>{c}</option>)}
            </select>
          </div>
        </Substrate>

        {/* Тёмная тема (глобальная через Zustand) */}
        <Substrate className={styles.settingItem}>
          <div className={styles.settingRow}>
            <icons.Moon className={styles.icon} />
            <Typography variant="body1" className={styles.settingLabel}>Тёмная тема</Typography>
            <label className={styles.switch}>
              <input
                type="checkbox"
                checked={theme === 'dark'}
                onChange={toggleTheme}
              />
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