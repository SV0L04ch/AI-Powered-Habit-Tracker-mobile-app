import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './ProfilePage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import icons from '../../lib/icons';
import useAuthUser from '../../store/useAuthStore';
import useThemeStore from '../../store/useThemeStore';

const ProfilePage = () => {
  const navigate = useNavigate();
  
  // Хранилище темы
  const { theme, toggleTheme } = useThemeStore();
  
  // Хранилище авторизации
  const updateProfile = useAuthUser((state) => state.updateProfile);
  const getProfile = useAuthUser((state) => state.getProfile);
  const userCity = useAuthUser((state) => state.city);
  const userReportTime = useAuthUser((state) => state.reportTime);
  const userEmail = useAuthUser((state) => state.email);
  const logOut = useAuthUser((state) => state.logout);
  const isLoading = useAuthUser((state) => state.isLoading);
  
  const [city, setCity] = useState(userCity || '');
  const [reportTime, setReportTime] = useState(userReportTime || '08:00');
  const [citiesList] = useState(['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань']);

  // Загрузка профиля при открытии страницы
  useEffect(() => {
    const loadProfile = async () => {
      try {
        const profile = await getProfile();
        if (profile.city) setCity(profile.city);
        if (profile.reportTime) setReportTime(profile.reportTime);
      } catch (error) {
        console.error('Ошибка загрузки профиля:', error);
      }
    };
    loadProfile();
  }, []);

  const handleCityChange = async (e) => {
    const newCity = e.target.value;
    setCity(newCity);
    try {
      await updateProfile({ city: newCity });
      console.log('Город обновлён');
    } catch (error) {
      console.error('Ошибка обновления города:', error);
    }
  };

  const handleReportTimeChange = async (e) => {
    const newTime = e.target.value;
    setReportTime(newTime);
    try {
      await updateProfile({ reportTime: newTime });
      console.log('Время отчёта обновлено');
    } catch (error) {
      console.error('Ошибка обновления времени:', error);
    }
  };

  const handleLogout = async () => {
    await logOut();
    navigate('/login');
  };

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>Профиль</Typography>
      <Typography variant="headline2" className={styles.name} data-testid="profile-name">
        {userEmail || 'Пользователь'}
      </Typography>
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
              disabled={isLoading}
            />
          </div>
        </Substrate>

        <Substrate className={styles.settingItem}>
          <div className={styles.settingRow}>
            <icons.MapPoint className={styles.icon} />
            <Typography variant="body1" className={styles.settingLabel}>Ваш город</Typography>
            <select 
              value={city} 
              onChange={handleCityChange} 
              className={styles.citySelect}
              disabled={isLoading}
            >
              <option value="">Выберите город</option>
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