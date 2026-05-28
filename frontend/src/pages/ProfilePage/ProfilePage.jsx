import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './ProfilePage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import icons from '../../lib/icons';
import useAuthUser from '../../store/useAuthStore';
import useProfileStore from '../../store/useProfileStore';

const ProfilePage = () => {
  const navigate = useNavigate();

  // Аутентификация и выход
  const logOut = useAuthUser((state) => state.logout);
  const isAuthenticated = useAuthUser((state) => state.isAuthenticated);

  // Профиль: состояния и действия
  const isProfileLoading = useProfileStore((state) => state.isLoading);
  const profileError = useProfileStore((state) => state.error);
  const fetchProfile = useProfileStore((state) => state.fetchProfile);
  const updProfile = useProfileStore((state) => state.updProfile);
  const toggleTheme = useProfileStore((state) => state.toggleTheme);

  // Данные профиля
  const city = useProfileStore((state) => state.city);
  const remindTime = useProfileStore((state) => state.remindTime);
  const theme = useProfileStore((state) => state.theme);
  const profileEmail = useProfileStore((state) => state.email);

  // Локальное состояние формы
  const [formCity, setFormCity] = useState(city || '');
  const [formRemindTime, setFormRemindTime] = useState(remindTime || '08:00');

  // Заглушка городов
  const citiesList = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань'];

  // Загружаем профиль при входе
  useEffect(() => {
    if (isAuthenticated) {
      fetchProfile();
    }
  }, [isAuthenticated]);

  // Синхронизируем локальные поля с данными из стора
  useEffect(() => {
    if (city) setFormCity(city);
    if (remindTime) setFormRemindTime(remindTime);
  }, [city, remindTime]);

  // Обработчики изменений (сразу сохраняют на сервер)
  const handleCityChange = (e) => {
    const newCity = e.target.value;
    setFormCity(newCity);
    updProfile({ city: newCity });
  };

  const handleRemindTimeChange = (e) => {
    const newTime = e.target.value;
    setFormRemindTime(newTime);
    updProfile({ remindTime: newTime });
  };

  const handleThemeToggle = () => {
    toggleTheme();
  };

  const handleLogout = async () => {
    await logOut();
    navigate('/login', { replace: true });
  };

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>Профиль</Typography>
      <Typography variant="headline2" className={styles.name} data-testid="profile-name">
        {profileEmail || 'Пользователь'}
      </Typography>
      <Typography variant="headline3" className={styles.sectionTitle}>Персонализация</Typography>

      <div className={styles.settingsContainer}>
        {/* Время ежедневного отчёта */}
        <Substrate className={styles.settingItem}>
          <div className={styles.reportRow}>
            <icons.Notification className={styles.icon} />
            <div className={styles.reportInfo}>
              <Typography variant="body1" className={styles.settingLabel}>Ежедневный отчёт</Typography>
              <Typography variant="caption" className={styles.settingHint}>Напоминание о прогрессе</Typography>
            </div>
            <input
              type="time"
              value={formRemindTime}
              onChange={handleRemindTimeChange}
              className={styles.timePicker}
              data-testid="remind-time-input"
            />
          </div>
        </Substrate>

        {/* Город */}
        <Substrate className={styles.settingItem}>
          <div className={styles.settingRow}>
            <icons.MapPoint className={styles.icon} />
            <Typography variant="body1" className={styles.settingLabel}>Ваш город</Typography>
            <select
              value={formCity}
              onChange={handleCityChange}
              className={styles.citySelect}
              data-testid="city-select"
            >
              {citiesList.map(c => (
                <option key={c} value={c}>{c}</option>
              ))}
            </select>
          </div>
        </Substrate>

        {/* Тёмная тема */}
        <Substrate className={styles.settingItem}>
          <div className={styles.settingRow}>
            <icons.Moon className={styles.icon} />
            <Typography variant="body1" className={styles.settingLabel}>Тёмная тема</Typography>
            <label className={styles.switch}>
              <input
                type="checkbox"
                checked={theme === 'dark'}
                onChange={handleThemeToggle}
                data-testid="theme-toggle"
              />
              <span className={styles.slider}></span>
            </label>
          </div>
        </Substrate>

        {/* Ошибки и загрузка */}
        {profileError && (
          <Typography variant="caption" className={styles.error} data-testid="profile-error">
            {profileError}
          </Typography>
        )}
        {isProfileLoading && <Typography variant="body1">Загрузка профиля...</Typography>}

        {/* Кнопка выхода */}
        <Button
          variant="form"
          className={styles.logoutButton}
          onClick={handleLogout}
          disabled={isProfileLoading}
          data-testid="logout-button"
        >
          {isProfileLoading ? 'Выход...' : 'Выход'}
        </Button>
      </div>
    </div>
  );
};

export default ProfilePage;