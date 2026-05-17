<<<<<<< HEAD
import { useNavigate } from "react-router-dom";
import Typography from "../../components/Typography/Typography";
import Substrate from "../../components/Substrate/Substrate";
import AddButton from "./components/AddButton/AddButton";
import PageLayout from "../../components/PageLayout/PageLayout";
import Checkbox from "./components/Checkbox/Checkbox";
import ContextMenu from "./components/ContextMenu/ContextMenu";
import styles from "./HabitsPage.module.scss"
import EditHabitModal from "../../components/EditHabitModal/EditHabitModal";
import { useState, useEffect } from "react";
import useAuthUser from '../../store/useAuthStore';
import useHabits from "../../store/useHabitsStore";
import useInsight from "../../store/useInsightStore";
import Modal from "../../components/Modal/Modal";
import Button from "../../components/Button/Button";

function HabitsPage() {
  const [editingHabit, setEditingHabit] = useState(null)
  const {message: insightMessage, isLoading: isInsightLoading, error: insightError, fetchSupport, clearInsight} = useInsight()
  const navigate = useNavigate();
  const habits = useHabits((state) => state.habits)
  const isHabitsLoading = useHabits((state) => state.isLoading)
  const habitsError = useHabits((state) => state.error)
  const getHabits = useHabits((state) => state.getHabits)
  const deleteHabit = useHabits((state) => state.deleteHabit);
  const clearError = useHabits((state) => state.clearError);
  const isAuthenticated = useAuthUser((state) => state.isAuthenticated);
  const isLoaded = useHabits((state) => state.isLoaded)
  
  const [showInsightModal, setShowInsightModal] = useState(false)



    useEffect(() => {
      clearError()
      getHabits()
      if (insightMessage) {
        setShowInsightModal(true)
      }
      
    }, [insightMessage]);

    const handleCloseInsight = () => {
      setShowInsightModal(false);
      clearInsight();
    };

  const handleDelete = async (id) => {
    if (window.confirm('Удалить привычку?')) {
      await deleteHabit(id);
    }
  };

  const handleClick = () => {
    navigate("/habits/new");
  };
  const activeHabits = habits.filter((h) => h.is_active !== false);
  const completedHabits = habits.filter((h) => h.is_active === false);
  return (
    <PageLayout>
      <Typography variant="headline1">Главная</Typography>

      {isHabitsLoading && <Typography variant="body1" data-testid="data-loading">Загрузка...</Typography>}
      {habitsError && <Typography variant="body1" style={{ color: 'red' }} data-testid="server-error">Ошибка: {habitsError}</Typography>} {/*Здесь выводится ошибка*/ }

      <Typography variant="body1">
        Твой прогресс сегодня: 2/5 привычек
      </Typography>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Активные привычки</Typography>
                {activeHabits.length === 0 && !isHabitsLoading && isAuthenticated && (
          <Typography variant="body2">Пока нет активных привычек</Typography>
        )}
        {activeHabits.map((habit) => (
          <Substrate key={habit.id} variant="secondary" data-testid={`active-habit-${habit.id}`}>
            <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
              <div className={styles.checkDesc}>
                <Checkbox checked={!!habit.is_active} data-testid={`active-habit${habit.id}-checkbox`}/>
                <div className={styles.desc}>
                  <Typography variant="headline3" data-testid={`active-habit-${habit.id}-name`}>{habit.name}</Typography>
                  <div className={styles.captions}>
                    {/* Формат значения в зависимости от типа */}
                    <Typography variant="caption" data-testid={`active-habit-${habit.id}-trigger`}>
                      {habit.triggerType === 1 ? habit.triggerValue : `${habit.triggerValue} раз`}                
                    </Typography>
                    {/* Разделитель и количество дней (пока статика, потом можно брать из API) */}
                    <Typography variant="caption" data-testid={`active-habit-${habit.id}-counter`}>
                      • {habit.daysCount ?? 0} дн.
                    </Typography>
                  </div>
                </div>
              </div>
              <div className={styles.actions}>
                <button onClick={() => setEditingHabit(habit)} data-testid={`active-habit-${habit.id}-edit-button`}>✎</button>
                <button onClick={() => handleDelete(habit.id)} data-testid={`active-habit-${habit.id}-delete-button`}>✕</button>
                <button onClick={() => fetchSupport(habit.id, habit.name)} disabled={isInsightLoading} data-testid={`ai-insight-${habit.id}`}>💡</button>
              </div>
            </div>
          </Substrate>
        ))}
      </div>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Завершены</Typography>
                {completedHabits.length === 0 && !isHabitsLoading && isAuthenticated && (
          <Typography variant="body2">Нет завершённых привычек</Typography>
        )}
        {completedHabits.map((habit) => (
          <Substrate key={habit.id} variant="secondary" data-testid={`inactive-habit-${habit.id}`}>
            <div className={styles.checkDesc}>
              <Checkbox checked={false} data-testid={`inactive-habit-${habit.id}-checkbox`}/>
              <div className={styles.desc}>  clearError();
                <Typography variant="headline3" data-testid={`inactive-habit-${habit.id}-name`}>
                  {habit.name}
                </Typography>
                <div className={styles.captions}>
                  <Typography variant="caption" data-testid={`inactive-habit-${habit.id}-trigger`}>
                    {habit.triggerType === 1 ? habit.triggerValue : `${habit.triggerValue} раз`}                
                  </Typography>
                  <Typography variant="caption" data-testid={`inactive-habit-${habit.id}-counter`}>
                    • {habit.daysCount ?? 0} дн.
                  </Typography>
                </div>
              </div>
            </div>
          </Substrate>
        ))}
      </div>
      <AddButton click={handleClick}></AddButton>
      <EditHabitModal
        isOpen={!!editingHabit}
        onClose={() => setEditingHabit(null)}
        habit={editingHabit}
      />

    <Modal isOpen={showInsightModal} onClose={handleCloseInsight} data-testid="insight-modal">
      <div className={styles.insightContainer}>
        <Typography variant="headline3">Совет дня</Typography>
        {isInsightLoading && <Typography>Генерация совета...</Typography>}
        {insightError && <Typography className={styles.errorText} data-testid="insight-error">{insightError}</Typography>}
        {insightMessage && <Typography variant="body1" data-testid="insight-message">{insightMessage}</Typography>}
      </div>
      <div className={styles.modalActions}>
          <Button variant="primary" onClick={handleCloseInsight} data-testid="insight-close-button">
            Отлично!
          </Button>
        </div>
    </Modal>

=======
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
>>>>>>> feature/frontend-city-insights-page
    </PageLayout>
  );
}

export default HabitsPage;