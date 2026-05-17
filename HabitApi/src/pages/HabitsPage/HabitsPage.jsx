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

function HabitsPage() {
  const [editingHabit, setEditingHabit] = useState(null)

  const navigate = useNavigate();
  const habits = useHabits((state) => state.habits)
  const isLoading = useHabits((state) => state.isLoading)
  const error = useHabits((state) => state.error)
  const getHabits = useHabits((state) => state.getHabits)
  const deleteHabit = useHabits((state) => state.deleteHabit);
  const clearError = useHabits((state) => state.clearError);
  const isAuthenticated = useAuthUser((state) => state.isAuthenticated);



  //   useEffect(() => {
  //   clearError();
  //   if (isAuthenticated) {
  //     getHabits();
  //   }
  // }, [isAuthenticated])

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

      {isLoading && <Typography variant="body1" data-testid="data-loading">Загрузка...</Typography>}
      {error && <Typography variant="body1" style={{ color: 'red' }} data-testid="server-error">Ошибка: {error}</Typography>}

      <Typography variant="body1">
        Твой прогресс сегодня: 2/5 привычек
      </Typography>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Активные привычки</Typography>
                {activeHabits.length === 0 && !isLoading && isAuthenticated && (
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
    </div>
  </div>
</Substrate>
))}
      </div>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Завершены</Typography>
                {completedHabits.length === 0 && !isLoading && isAuthenticated && (
          <Typography variant="body2">Нет завершённых привычек</Typography>
        )}
        {completedHabits.map((habit) => (
          <Substrate key={habit.id} variant="secondary" data-testid={`inactive-habit-${habit.id}`}>
            <div className={styles.checkDesc}>
              <Checkbox checked={false} data-testid={`inactive-habit-${habit.id}-checkbox`}/>
              <div className={styles.desc}>
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

    </PageLayout>
  );
}

export default HabitsPage;