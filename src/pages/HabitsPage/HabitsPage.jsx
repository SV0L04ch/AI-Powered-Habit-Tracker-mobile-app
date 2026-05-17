import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Typography from "../../components/Typography/Typography";
import Substrate from "../../components/Substrate/Substrate";
import PageLayout from "../../components/PageLayout/PageLayout";
import AddButton from "./components/AddButton/AddButton";
import Checkbox from "./components/Checkbox/Checkbox";
import ContextMenu from "./components/ContextMenu/ContextMenu";
import styles from "./HabitsPage.module.scss";

function HabitsPage() {
  const navigate = useNavigate();
  const [menu, setMenu] = useState(null);
  const [activeHabits, setActiveHabits] = useState([
    { id: 1, name: 'Утренняя медитация', streak: 12, time: '08:00' },
    { id: 2, name: 'Чтение книги', streak: 5, time: '21:00' },
    { id: 3, name: 'Тренировка в зале', streak: 3, time: '18:30' },
  ]);
  const [completedHabits, setCompletedHabits] = useState([
    { id: 4, name: 'Выпить 2л воды', streak: 7, time: '12:00' },
  ]);

  const handleAdd = () => navigate("/habits/new");

  const handleCheckboxToggle = (habit, isCompleted) => {
    if (isCompleted) {
      setCompletedHabits(prev => prev.filter(h => h.id !== habit.id));
      setActiveHabits(prev => [...prev, habit]);
    } else {
      setActiveHabits(prev => prev.filter(h => h.id !== habit.id));
      setCompletedHabits(prev => [...prev, habit]);
    }
  };

  const handleEdit = (habitId) => {
    console.log('Редактировать', habitId);
    setMenu(null);
  };

  const handleDelete = (habitId) => {
    setActiveHabits(prev => prev.filter(h => h.id !== habitId));
    setCompletedHabits(prev => prev.filter(h => h.id !== habitId));
    setMenu(null);
  };

  const handleMenuClick = (e, habitId) => {
    e.stopPropagation();
    const rect = e.currentTarget.getBoundingClientRect();
    setMenu({
      habitId,
      x: rect.right - 150,
      y: rect.top + 20,
    });
  };

  const closeMenu = () => setMenu(null);

  const renderHabitCard = (habit, isCompleted = false) => (
    <Substrate key={habit.id} variant="secondary" className={styles.habitCard}>
      <div className={styles.checkDesc}>
        {!isCompleted && (
        <Checkbox
          checked={isCompleted}
          onChange={() => handleCheckboxToggle(habit, isCompleted)}
        />
        )}
        <div className={styles.desc}>
          <Typography variant="Head3">{habit.name}</Typography>
          <div className={styles.captions}>
            <Typography variant="caption">{habit.streak} дней</Typography>
            <Typography variant="caption">{habit.time}</Typography>
          </div>
        </div>
        <button
          className={styles.menuButton}
          onClick={(e) => handleMenuClick(e, habit.id)}
        >
          ⋮
        </button>
      </div>
      {menu?.habitId === habit.id && (
        <ContextMenu
          items={[
            { label: 'Редактировать', onClick: () => handleEdit(habit.id) },
            { label: 'Удалить', onClick: () => handleDelete(habit.id) },
          ]}
          onClose={closeMenu}
          position={{ x: menu.x, y: menu.y }}
        />
      )}
    </Substrate>
  );

  return (
    <PageLayout>
      <Typography variant="headline1">Главная</Typography>
      <Typography variant="body1">
        Твой прогресс сегодня: {activeHabits.length}/{activeHabits.length + completedHabits.length} привычек
      </Typography>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Активные привычки</Typography>
        {activeHabits.map(habit => renderHabitCard(habit, false))}
      </div>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Завершены</Typography>
        {completedHabits.map(habit => renderHabitCard(habit, true))}
      </div>
      <AddButton click={handleAdd} />
    </PageLayout>
  );
}

export default HabitsPage;