import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import useAuthStore from '../../store/useAuthStore';
import useThemeStore from '../../store/useThemeStore';
import styles from './ProfilePage.module.scss';
export default function ProfilePage() {
  const { profile, saveProfile, logout } = useAuthStore();
  const { theme, toggleTheme } = useThemeStore();
  const navigate = useNavigate();
  const [form, setForm] = useState({ name: '', city: '' });
  useEffect(() => { if (profile) setForm({ name: profile.name || '', city: profile.city || '' }); }, [profile]);
  const handleSave = () => saveProfile(form);
  const handleLogout = () => { logout(); navigate('/login'); };
  return (
    <div className={styles.page}>
      <h1>Profile</h1>
      <div className={styles.form}>
        <div className={styles.field}><label>Name</label><input value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} /></div>
        <div className={styles.field}><label>City</label><input value={form.city} onChange={e => setForm(f => ({ ...f, city: e.target.value }))} /></div>
        <div className={styles.field}><label>Theme</label><button onClick={toggleTheme} className={styles.themeBtn}>{theme === 'dark' ? '☀️ Light' : '🌙 Dark'}</button></div>
        <button onClick={handleSave} className={styles.saveBtn}>Save Changes</button>
        <button onClick={handleLogout} className={styles.logoutBtn}>Logout</button>
      </div>
    </div>
  );
}
