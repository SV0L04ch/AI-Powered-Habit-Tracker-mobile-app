import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Typography from '../../components/Typography/Typography';
import Substrate from '../../components/Substrate/Substrate';
import AddButton from './components/AddButton/AddButton';
import PageLayout from '../../components/PageLayout/PageLayout';
import Checkbox from './components/Checkbox/Checkbox';
import ContextMenu from './components/ContextMenu/ContextMenu';
import EditHabitModal from '../../components/EditHabitModal/EditHabitModal';
import Modal from '../../components/Modal/Modal';
import Button from '../../components/Button/Button';
import useAuthUser from '../../store/useAuthStore';
import useHabits from '../../store/useHabitsStore';
import useInsight from '../../store/useInsightStore';
import styles from './HabitsPage.module.scss';

function HabitsPage() {
  const [editingHabit, setEditingHabit] = useState(null);
  const [menuData, setMenuData] = useState(null);
  const [showInsightModal, setShowInsightModal] = useState(false);
  const [localCheckedState, setLocalCheckedState] = useState({});

  const { message: insightMessage, isLoading: isInsightLoading, error: insightError, fetchSupport, clearInsight } = useInsight();
  const navigate = useNavigate();
  const habits = useHabits((state) => state.habits);
  const isHabitsLoading = useHabits((state) => state.isLoading);
  const habitsError = useHabits((state) => state.error);
  const getHabits = useHabits((state) => state.getHabits);
  const deleteHabit = useHabits((state) => state.deleteHabit);
  const updateHabit = useHabits((state) => state.updateHabit);
  const clearError = useHabits((state) => state.clearError);
  const isAuthenticated = useAuthUser((state) => state.isAuthenticated);
  
  // Загрузка привычек при входе
  useEffect(() => {
    clearError();
    if (isAuthenticated) {
      getHabits();
    }
  }, [isAuthenticated]);

  // Открытие модалки AI-совета
  useEffect(() => {
    if (insightMessage) {
      setShowInsightModal(true);
    }
  }, [insightMessage]);

  const handleCloseInsight = () => {
    setShowInsightModal(false);
    clearInsight();
  };

  const handleMenuClick = (e, habitId) => {
    e.stopPropagation();
    if (menuData?.habitId === habitId) {
      setMenuData(null);
    } else {
      const rect = e.currentTarget.getBoundingClientRect();
      setMenuData({
        habitId,
        x: rect.right - 150,  
        y: rect.top + 20,
      });
    }
  };

  const closeMenu = () => setMenuData(null);

  const handleDelete = async (id) => {
    if (window.confirm('Удалить привычку?')) {
      await deleteHabit(id);
      setMenuData(null);
    }
  };

  const handleToggleActive = async (habit) => {
  try {
    // Отправляем запрос на сервер
    await updateHabit(habit.id, { isActive: false });
    
    // Перезагружаем свежий список из БД (этого достаточно)
    await getHabits();
    
  } catch (err) {
    console.error('❌ Ошибка:', err);
  }
  setMenuData(null);
};

  const handleClick = () => {
    navigate('/habits/new');
  };

  const activeHabits = habits.filter((h) => h.is_active !== false);
  const completedHabits = habits.filter((h) => h.is_active === false);

  const renderHabitCard = (habit, isCompleted = false) => {
    // Берём локальное состояние или определяем по умолчанию
    const isChecked = localCheckedState[habit.id] !== undefined 
      ? localCheckedState[habit.id] 
      : !isCompleted;  // активные = true, завершённые = false

    const handleCheckboxChange = () => {
      if (isCompleted) return;
      
      // Меняем локальное состояние (убираем галочку)
      setLocalCheckedState(prev => ({
        ...prev,
        [habit.id]: false
      }));
      
      // Отправляем запрос на сервер
      handleToggleActive(habit);
    };

    return (
      <Substrate key={habit.id} variant="secondary" data-testid={`${isCompleted ? 'inactive' : 'active'}-habit-${habit.id}`}>
        <div className={styles.habitWrap}>
          <div className={styles.checkDesc}>
            <Checkbox 
              checked={isChecked}
              onChange={handleCheckboxChange}
              disabled={isCompleted}
              data-testid={`${isCompleted ? 'inactive' : 'active'}-habit${habit.id}-checkbox`}
            />
            <div className={styles.desc}>
              <Typography variant="headline3" data-testid={`${isCompleted ? 'inactive' : 'active'}-habit-${habit.id}-name`}>
                {habit.name}
              </Typography>
              <div className={styles.captions}>
                <Typography variant="caption" data-testid={`${isCompleted ? 'inactive' : 'active'}-habit-${habit.id}-trigger`}>
                  {habit.triggerType === 1 ? habit.triggerValue : `${habit.triggerValue} раз`}
                </Typography>
                <Typography variant="caption" data-testid={`${isCompleted ? 'inactive' : 'active'}-habit-${habit.id}-counter`}>
                  • {habit.daysCount ?? 0} дн.
                </Typography>
              </div>
            </div>
          </div>
          <div className={styles.actions}>
            <button
              className={styles.menuButton}
              onClick={(e) => handleMenuClick(e, habit.id)}
            >
              ⋮
            </button>
            {menuData?.habitId === habit.id && (
              <ContextMenu
                items={[
                  { label: 'Редактировать', onClick: () => { setEditingHabit(habit); closeMenu(); } },
                  { label: 'Удалить', onClick: () => { handleDelete(habit.id); closeMenu(); } },
                  { label: 'Совет дня', onClick: () => { fetchSupport(habit.id, habit.name); closeMenu(); } },
                ]}
                onClose={closeMenu}
                position={{ x: menuData.x, y: menuData.y }}
              />
            )}
          </div>
        </div>
      </Substrate>
    );
  };

  return (
    <PageLayout>
      <Typography variant="headline1" className={styles.pageTitle}>Главная</Typography>

      {isHabitsLoading && <Typography variant="body1" data-testid="data-loading">Загрузка...</Typography>}
      {habitsError && <Typography variant="body1" style={{ color: 'red' }} data-testid="server-error">Ошибка: {habitsError}</Typography>}

      <Typography variant="body1" className={styles.progressText}>
        Твой прогресс сегодня: {habits.length > 0 ? `${habits.length}/5 привычек` : 'Нет данных'}
      </Typography>

      <div className={styles.blockHabits}>
        <Typography variant="headline2" className={styles.sectionTitle}>Активные привычки</Typography>
        {activeHabits.length === 0 && !isHabitsLoading && isAuthenticated && (
          <Typography variant="body2">Пока нет активных привычек</Typography>
        )}
        {activeHabits.map(habit => renderHabitCard(habit, false))}
      </div>

      <div className={styles.blockHabits}>
        <Typography variant="headline2" className={styles.sectionCompleted}>Завершены</Typography>
        {completedHabits.length === 0 && !isHabitsLoading && isAuthenticated && (
          <Typography variant="body2">Нет завершённых привычек</Typography>
        )}
        {completedHabits.map(habit => renderHabitCard(habit, true))}
      </div>

      <AddButton click={handleClick} data-testid="add-button"/>

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
            Хорошо
          </Button>
        </div>
      </Modal>
    </PageLayout>
  );
}

export default HabitsPage;