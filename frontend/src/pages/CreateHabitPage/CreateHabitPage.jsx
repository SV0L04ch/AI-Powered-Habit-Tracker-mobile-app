import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './CreateHabitPage.module.scss';
import PageLayout from '../../components/PageLayout/PageLayout';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import Button from '../../components/Button/Button';
import icons from '../../lib/icons';
import useHabits from '../../store/useHabitsStore';
import useAuthStore from '../../store/useAuthStore';

function CreateHabitPage() {
  const navigate = useNavigate();
  const addHabit = useHabits((state) => state.addHabit);
  const isLoading = useHabits((state) => state.isLoading);
  const error = useHabits((state) => state.error);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  // Локальное состояние формы
  const [habit, setHabit] = useState({
    name: '',
    type: false,        // false = время, true = повторы
    category: false,    // false = легко, true = тяжело
    trigger_value: '',
  });
  const [validationError, setValidationError] = useState('');

  const handleChange = (e) => {
    setHabit({ ...habit, [e.target.name]: e.target.value });
    setValidationError('');
  };

const handleSubmit = async (e) => {
  e.preventDefault();

  // Валидация
  if (!habit.name.trim()) {
    setValidationError('Введите название привычки');
    return;
  }
  if (!habit.trigger_value.trim()) {
    setValidationError('Введите значение (время или количество)');
    return;
  }

  const triggerType = habit.type ? 2 : 1;   // 1 = время, 2 = повторы

  // Собираем данные строго по контракту (camelCase, верхний уровень)
  const habitData = {
    name: habit.name,
    type: habit.type,                // true = положительная, false = отрицательная
    category: habit.category,        // true = сложная, false = легкая
    triggerType: triggerType,        // 1 или 2
    triggerValue: habit.trigger_value,  // всегда строка ("08:00" или "5")
    // Остальные поля пока не отправляем, т.к. они необязательны
    // targetDays: 30,
    // penaltyDaysPerMiss: 0,
    // reminders: [habit.trigger_value],
  };

  await addHabit(habitData);

  if (!useHabits.getState().error) {
    navigate('/habits');
  }
};

  const descClass = `${styles.basicText} ${styles.desc}`;

  return (
    <PageLayout>
      <Typography variant='headline1' className={styles.mainText}>Создать привычку</Typography>

      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.blocktext}>
          <Typography variant='headline2' className={styles.mainText}>Название привычки</Typography>
          <Input
            name="name"
            placeholder='Например: Медитация'
            value={habit.name}
            onChange={handleChange}
            disabled={isLoading}
            data-testid="name-input"
          />
        </div>

        <div className={styles.block}>
          <div className={styles.blocktext}>
            <Typography variant='headline2' className={styles.mainText}>Тип контроля</Typography>
            <div className={styles.buttons}>
              <Button
                variant={!habit.type ? 'primary' : 'secondary'}
                onClick={() => setHabit({ ...habit, type: false })}
                data-testid="controlType-time-button"
              >
                <div className={styles.optionContent}>
                  <icons.Wristwatch />
                  <Typography variant='body2' className={styles.basicText}>Время</Typography>
                  <Typography variant='caption' className={descClass}>Напоминание</Typography>
                </div>
              </Button>
              <Button
                variant={habit.type ? 'primary' : 'secondary'}
                onClick={() => setHabit({ ...habit, type: true })}
                data-testid="controlType-counter-button"
              >
                <div className={styles.optionContent}>
                  <icons.Count className={styles.basicText} />
                  <Typography variant='body2' className={styles.basicText}>Повторы</Typography>
                  <Typography variant='caption' className={descClass}>Счетчик</Typography>
                </div>
              </Button>
            </div>
          </div>

          <div className={styles.blocktext}>
            <Typography variant='headline2' className={styles.mainText}>Сложность</Typography>
            <div className={styles.buttons}>
              <Button
                variant={habit.category ? 'primary' : 'secondary'}
                onClick={() => setHabit({ ...habit, category: true })}
                data-testid="categoryType-hard-button"
              >
                <div className={styles.optionContent}>
                  <div className={styles.stars}>
                    <icons.FillStar />
                    <icons.FillStar />
                    <icons.FillStar />
                  </div>
                  <Typography variant='body2' className={styles.basicText}>Тяжело</Typography>
                  <Typography variant='caption' className={descClass}>Штрафы<br/> за пропуск</Typography>
                </div>
              </Button>
              <Button
                variant={!habit.category ? 'primary' : 'secondary'}
                onClick={() => setHabit({ ...habit, category: false })}
                data-testid="categoryType-easy-button"
              >
                <div className={styles.optionContent}>
                  <div className={styles.stars}>
                    <icons.EmptyStar />
                    <icons.EmptyStar />
                    <icons.EmptyStar />
                  </div>
                  <Typography variant='body2' className={styles.basicText}>Легко</Typography>
                  <Typography variant='caption' className={descClass}>Нету штрафов<br/> за пропуск</Typography>
                </div>
              </Button>
            </div>
          </div>
        </div>

        <div className={styles.block}>
          <div className={styles.blocktext}>
            <Typography variant='headline2' className={styles.mainText}>Время напоминания</Typography>
            <Input
              name="trigger_value"
              placeholder="08:00 или 5"
              icon={<icons.Watch />}
              value={habit.trigger_value}
              onChange={handleChange}
              disabled={isLoading}
              data-testid="trigger-type"
            />
          </div>
        </div>

        {validationError && (
          <Typography variant="caption" className={styles.errorText} data-testid="validation-error">
            {validationError}
          </Typography>
        )}
        {error && (
          <Typography variant="caption" className={styles.errorText} data-testid="server-error">
            {error}
          </Typography>
        )}

        <Button type="submit" variant='primary' disabled={isLoading} data-testid="submit-button">
          {isLoading ? 'Создание...' : 'Создать привычку'}
        </Button>
      </form>
    </PageLayout>
  );
}

export default CreateHabitPage;