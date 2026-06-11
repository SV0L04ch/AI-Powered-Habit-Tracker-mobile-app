import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import useHabitsStore from '../../store/useHabitsStore';
import styles from './CreateHabitPage.module.scss';

export default function CreateHabitPage() {
  const [form, setForm] = useState({ name: '', triggerType: 1, triggerValue: '1', isPositive: true, hasPenalty: false, targetDays: 30 });
  const { addHabit } = useHabitsStore();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    await addHabit(form);
    navigate('/habits');
  };

  const update = (k, v) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div className={styles.page}>
      <h1>Create New Habit</h1>
      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.field}>
          <label>Habit Name</label>
          <input value={form.name} onChange={e => update('name', e.target.value)} placeholder="e.g. Morning Meditation" required />
        </div>
        <div className={styles.field}>
          <label>Type</label>
          <div className={styles.toggleGroup}>
            <button type="button" className={`${styles.toggle} ${form.isPositive ? styles.active : ''}`} onClick={() => update('isPositive', true)}>Positive ✓</button>
            <button type="button" className={`${styles.toggle} ${!form.isPositive ? styles.active : ''}`} onClick={() => update('isPositive', false)}>Negative ✗</button>
          </div>
        </div>
        <div className={styles.row}>
          <div className={styles.field}>
            <label>Trigger Type</label>
            <select value={form.triggerType} onChange={e => update('triggerType', Number(e.target.value))}>
              <option value={1}>Time of Day</option>
              <option value={2}>Count Per Day</option>
            </select>
          </div>
          <div className={styles.field}>
            <label>Trigger Value</label>
            <input value={form.triggerValue} onChange={e => update('triggerValue', e.target.value)} placeholder={form.triggerType === 1 ? '09:00' : '8'} />
          </div>
        </div>
        <div className={styles.field}>
          <label>Target Days</label>
          <input type="number" value={form.targetDays} onChange={e => update('targetDays', Number(e.target.value))} min="1" max="365" />
        </div>
        <button type="submit" className={styles.submitBtn}>Create Habit</button>
      </form>
    </div>
  );
}
