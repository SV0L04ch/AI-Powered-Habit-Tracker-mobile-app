import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './CreateHabitPage.module.scss';
import PageLayout from '../../components/PageLayout/PageLayout';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import Button from '../../components/Button/Button';
import icons from '../../lib/icons';
import useHabits from '../../store/useHabitsStore';

function CreateHabitPage() {
  const navigate = useNavigate();
  const addHabit = useHabits((state) => state.addHabit);
  const isLoading = useHabits((state) => state.isLoading);
  const error = useHabits((state) => state.error);
  const [habit, setHabit] = useState({
    name: '',
    isPositive: true,
    hasPenalty: false,
    triggerType: 1,
    triggerValue: '',
    targetDays: 30,
    reminderTime: '08:00',
  });
  const [validationError, setValidationError] = useState('');

  const patchHabit = (updates) => {
    setHabit((current) => ({ ...current, ...updates }));
    setValidationError('');
  };

  const handleChange = (event) => {
    patchHabit({ [event.target.name]: event.target.value });
  };

  const validate = () => {
    if (!habit.name.trim()) return 'Введите название привычки.';
    if (!habit.triggerValue.trim()) return 'Введите время или количество.';
    if (Number(habit.triggerType) === 1 && !/^\d{2}:\d{2}$/.test(habit.triggerValue)) {
      return 'Для контроля по времени используйте формат 08:00.';
    }
    if (Number(habit.triggerType) === 2 && (!Number.isInteger(Number(habit.triggerValue)) || Number(habit.triggerValue) < 1)) {
      return 'Для счетчика укажите целое число больше нуля.';
    }
    return '';
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const nextError = validate();
    if (nextError) {
      setValidationError(nextError);
      return;
    }

    const habitData = {
      name: habit.name.trim(),
      isPositive: habit.isPositive,
      hasPenalty: habit.hasPenalty,
      triggerType: Number(habit.triggerType),
      triggerValue: habit.triggerValue.trim(),
      targetDays: Number(habit.targetDays),
      penaltyDaysPerMiss: habit.hasPenalty ? 1 : 0,
      reminders: habit.reminderTime ? [habit.reminderTime] : [],
    };

    const created = await addHabit(habitData);
    if (created) navigate('/habits');
  };

  const triggerLabel = Number(habit.triggerType) === 1 ? 'Время выполнения' : 'Количество в день';

  return (
    <PageLayout data-testid="create-habit-page">
      <header className={styles.header} data-testid="create-header">
        <Typography variant="headline1" className={styles.mainText} data-testid="create-title">
          Новая привычка
        </Typography>
        <Typography variant="body1" className={styles.muted} data-testid="create-subtitle">
          Настройте контроль, напоминание и сложность.
        </Typography>
      </header>

      <form onSubmit={handleSubmit} className={styles.form} data-testid="create-habit-form">
        <section className={styles.card} data-testid="habit-name-section">
          <Typography variant="headline3">Название</Typography>
          <Input
            name="name"
            label="Название привычки"
            value={habit.name}
            onChange={handleChange}
            disabled={isLoading}
            data-testid="name-input"
          />
        </section>

        <section className={styles.card} data-testid="control-type-section">
          <Typography variant="headline3">Тип контроля</Typography>
          <div className={styles.segmented}>
            <Button
              variant={Number(habit.triggerType) === 1 ? 'primary' : 'secondary'}
              onClick={() => patchHabit({ triggerType: 1, triggerValue: habit.reminderTime })}
              data-testid="controlType-time-button"
            >
              <span className={styles.optionContent}>
                <icons.Wristwatch />
                Время
              </span>
            </Button>
            <Button
              variant={Number(habit.triggerType) === 2 ? 'primary' : 'secondary'}
              onClick={() => patchHabit({ triggerType: 2, triggerValue: '1' })}
              data-testid="controlType-counter-button"
            >
              <span className={styles.optionContent}>
                <icons.Count />
                Счетчик
              </span>
            </Button>
          </div>
        </section>

        <section className={styles.card} data-testid="habit-type-section">
          <Typography variant="headline3">Направление</Typography>
          <div className={styles.segmented}>
            <Button
              variant={habit.isPositive ? 'primary' : 'secondary'}
              onClick={() => patchHabit({ isPositive: true })}
              data-testid="positive-habit-button"
            >
              Полезная
            </Button>
            <Button
              variant={!habit.isPositive ? 'primary' : 'secondary'}
              onClick={() => patchHabit({ isPositive: false })}
              data-testid="negative-habit-button"
            >
              Контроль срывов
            </Button>
          </div>
        </section>

        <section className={styles.card} data-testid="category-section">
          <Typography variant="headline3">Сложность</Typography>
          <div className={styles.segmented}>
            <Button
              variant={habit.hasPenalty ? 'primary' : 'secondary'}
              onClick={() => patchHabit({ hasPenalty: true })}
              data-testid="categoryType-hard-button"
            >
              <span className={styles.optionContent}>
                <icons.FillStar />
                Со штрафом
              </span>
            </Button>
            <Button
              variant={!habit.hasPenalty ? 'primary' : 'secondary'}
              onClick={() => patchHabit({ hasPenalty: false })}
              data-testid="categoryType-easy-button"
            >
              <span className={styles.optionContent}>
                <icons.EmptyStar />
                Без штрафа
              </span>
            </Button>
          </div>
        </section>

        <section className={styles.card} data-testid="trigger-section">
          <Typography variant="headline3">{triggerLabel}</Typography>
          <Input
            name="triggerValue"
            type={Number(habit.triggerType) === 1 ? 'time' : 'number'}
            label={triggerLabel}
            icon={<icons.Watch />}
            value={habit.triggerValue}
            onChange={handleChange}
            disabled={isLoading}
            min={Number(habit.triggerType) === 2 ? '1' : undefined}
            data-testid="trigger-type"
          />
          <Input
            name="reminderTime"
            type="time"
            label="Время уведомления"
            value={habit.reminderTime}
            onChange={handleChange}
            disabled={isLoading}
            data-testid="reminder-time-input"
          />
          <Input
            name="targetDays"
            type="number"
            label="Цель в днях"
            value={habit.targetDays}
            onChange={handleChange}
            min="1"
            disabled={isLoading}
            data-testid="target-days-input"
          />
        </section>

        {validationError && (
          <p className={styles.errorText} data-testid="validation-error">
            {validationError}
          </p>
        )}
        {error && (
          <p className={styles.errorText} data-testid="server-error">
            {error}
          </p>
        )}

        <Button type="submit" variant="primary" loading={isLoading} data-testid="submit-button">
          Создать привычку
        </Button>
      </form>
    </PageLayout>
  );
}

export default CreateHabitPage;
