import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './ProfilePage.module.scss';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import icons from '../../lib/icons';
import useAuthUser from '../../store/useAuthStore';
import useThemeStore from '../../store/useThemeStore';

const ProfilePage = () => {
  const navigate = useNavigate();
  const { profile, email, profileLoading, profileError, loadProfile, saveProfile, logout, isLoading } =
    useAuthUser();
  const theme = useThemeStore((state) => state.theme);
  const setTheme = useThemeStore((state) => state.setTheme);
  const [form, setForm] = useState({
    city: '',
    habitReminderEnabled: true,
    habitReminderTime: '08:00',
    themePreference: 'light',
  });
  const [savedMessage, setSavedMessage] = useState('');

  useEffect(() => {
    loadProfile();
  }, [loadProfile]);

  useEffect(() => {
    if (!profile) return;

    setForm({
      city: profile.city || '',
      habitReminderEnabled: Boolean(profile.habitReminderEnabled),
      habitReminderTime: profile.habitReminderTime || '08:00',
      themePreference: profile.themePreference === 'dark' ? 'dark' : 'light',
    });
  }, [profile]);

  const patchForm = (updates) => {
    setForm((current) => ({ ...current, ...updates }));
    setSavedMessage('');
  };

  const handleThemeChange = (nextTheme) => {
    setTheme(nextTheme);
    patchForm({ themePreference: nextTheme });
  };

  const requestGeoCity = () => {
    if (!navigator.geolocation) {
      setSavedMessage('Геолокация недоступна в этом браузере.');
      return;
    }

    navigator.geolocation.getCurrentPosition(
      () => setSavedMessage('Геолокация получена. Введите название города и сохраните профиль.'),
      () => setSavedMessage('Не удалось получить геолокацию. Город можно ввести вручную.'),
      { enableHighAccuracy: false, timeout: 6000 },
    );
  };

  const handleSave = async () => {
    const saved = await saveProfile(form);
    if (saved) setSavedMessage('Профиль сохранен.');
  };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <div className={styles.page} data-testid="profile-page">
      <header className={styles.header} data-testid="profile-header">
        <Typography variant="headline1" className={styles.title} data-testid="profile-title">
          Профиль
        </Typography>
        <Typography variant="body1" className={styles.muted} data-testid="profile-email">
          {email || profile?.email}
        </Typography>
      </header>

      <section className={styles.settingsContainer} data-testid="profile-settings">
        <article className={styles.settingItem} data-testid="profile-city-card">
          <div className={styles.settingTitle}>
            <icons.MapPoint className={styles.icon} />
            <div>
              <Typography variant="headline3">Город</Typography>
              <Typography variant="body2" className={styles.muted}>
                Используется для погоды и городской сводки.
              </Typography>
            </div>
          </div>
          <Input
            label="Город"
            value={form.city}
            onChange={(event) => patchForm({ city: event.target.value })}
            data-testid="profile-city-input"
          />
          <Button variant="ghost" onClick={requestGeoCity} data-testid="profile-geolocation-button">
            Использовать геолокацию
          </Button>
        </article>

        <article className={styles.settingItem} data-testid="profile-reminder-card">
          <div className={styles.settingTitle}>
            <icons.Notification className={styles.icon} />
            <div>
              <Typography variant="headline3">Уведомления</Typography>
              <Typography variant="body2" className={styles.muted}>
                Время будет применяться для ежедневных напоминаний.
              </Typography>
            </div>
          </div>
          <label className={styles.switchRow} data-testid="reminder-enabled-row">
            <span>Ежедневное напоминание</span>
            <input
              type="checkbox"
              checked={form.habitReminderEnabled}
              onChange={(event) => patchForm({ habitReminderEnabled: event.target.checked })}
              data-testid="reminder-enabled-checkbox"
            />
          </label>
          <Input
            type="time"
            label="Время уведомлений"
            value={form.habitReminderTime}
            onChange={(event) => patchForm({ habitReminderTime: event.target.value })}
            disabled={!form.habitReminderEnabled}
            data-testid="profile-reminder-time-input"
          />
        </article>

        <article className={styles.settingItem} data-testid="profile-theme-card">
          <div className={styles.settingTitle}>
            <icons.Moon className={styles.icon} />
            <div>
              <Typography variant="headline3">Тема</Typography>
              <Typography variant="body2" className={styles.muted}>
                Светлая тема включена по умолчанию.
              </Typography>
            </div>
          </div>
          <div className={styles.themeToggle} data-testid="theme-toggle">
            <button
              type="button"
              className={form.themePreference === 'light' ? styles.activeTheme : ''}
              onClick={() => handleThemeChange('light')}
              data-testid="theme-light-button"
            >
              Светлая
            </button>
            <button
              type="button"
              className={form.themePreference === 'dark' ? styles.activeTheme : ''}
              onClick={() => handleThemeChange('dark')}
              data-testid="theme-dark-button"
            >
              Темная
            </button>
          </div>
        </article>
      </section>

      {profileLoading && <div className={styles.loader} data-testid="profile-loader" />}
      {profileError && (
        <p className={styles.error} data-testid="profile-error">
          {profileError}
        </p>
      )}
      {savedMessage && (
        <p className={styles.success} data-testid="profile-save-message">
          {savedMessage}
        </p>
      )}

      <div className={styles.actions} data-testid="profile-actions">
        <Button variant="primary" onClick={handleSave} loading={profileLoading} data-testid="profile-save-button">
          Сохранить
        </Button>
        <Button variant="danger" onClick={handleLogout} loading={isLoading} data-testid="logout-button">
          Выйти
        </Button>
      </div>
    </div>
  );
};

export default ProfilePage;
