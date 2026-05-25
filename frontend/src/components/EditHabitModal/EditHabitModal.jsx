import { useEffect, useState } from 'react';
import Modal from '../Modal/Modal';
import Input from '../Input/Input';
import Button from '../Button/Button';
import Typography from '../Typography/Typography';
import icons from '../../lib/icons';
import useHabitsStore from '../../store/useHabitsStore';
import styles from './EditHabitModal.module.scss';

const EditHabitModal = ({ isOpen, onClose, habit }) => {
  const [form, setForm] = useState({
    name: '',
    triggerValue: '',
    triggerType: 1,
    isPositive: true,
    hasPenalty: false,
    targetDays: 30,
    reminderTime: '',
  });
  const [validationError, setValidationError] = useState('');
  const updateHabit = useHabitsStore((state) => state.updateHabit);
  const actionLoadingId = useHabitsStore((state) => state.actionLoadingId);

  useEffect(() => {
    if (habit) {
      setForm({
        name: habit.name || '',
        triggerValue: habit.triggerValue || '',
        triggerType: Number(habit.triggerType) || 1,
        isPositive: habit.isPositive !== false,
        hasPenalty: Boolean(habit.hasPenalty),
        targetDays: habit.targetDays || 30,
        reminderTime: habit.reminders?.[0] || '',
      });
      setValidationError('');
    }
  }, [habit]);

  const patchForm = (updates) => {
    setForm((current) => ({ ...current, ...updates }));
    setValidationError('');
  };

  const handleSave = async () => {
    if (!form.name.trim()) {
      setValidationError('Введите название привычки.');
      return;
    }
    if (!form.triggerValue.trim()) {
      setValidationError('Введите значение триггера.');
      return;
    }

    const updated = await updateHabit(habit.id, {
      name: form.name.trim(),
      triggerValue: form.triggerValue.trim(),
      triggerType: Number(form.triggerType),
      isPositive: form.isPositive,
      hasPenalty: form.hasPenalty,
      targetDays: Number(form.targetDays),
      penaltyDaysPerMiss: form.hasPenalty ? 1 : 0,
      reminders: form.reminderTime ? [form.reminderTime] : [],
    });

    if (updated) onClose();
  };

  if (!habit) return null;

  return (
    <Modal isOpen={isOpen} onClose={onClose} data-testid="edit-habit-modal">
      <div className={styles.container}>
        <Typography variant="headline2" data-testid="edit-habit-title">
          Редактирование
        </Typography>

        <Input
          label="Название"
          value={form.name}
          onChange={(event) => patchForm({ name: event.target.value })}
          data-testid="habit-name"
        />

        <div className={styles.section} data-testid="edit-control-section">
          <Typography variant="body2" className={styles.label}>
            Тип контроля
          </Typography>
          <div className={styles.buttons}>
            <Button
              variant={Number(form.triggerType) === 1 ? 'primary' : 'secondary'}
              onClick={() => patchForm({ triggerType: 1 })}
              data-testid="controlType-time-button"
            >
              <span className={styles.optionContent}>
                <icons.Wristwatch />
                Время
              </span>
            </Button>
            <Button
              variant={Number(form.triggerType) === 2 ? 'primary' : 'secondary'}
              onClick={() => patchForm({ triggerType: 2 })}
              data-testid="controlType-counter-button"
            >
              <span className={styles.optionContent}>
                <icons.Count />
                Счетчик
              </span>
            </Button>
          </div>
        </div>

        <div className={styles.section} data-testid="edit-category-section">
          <Typography variant="body2" className={styles.label}>
            Сложность
          </Typography>
          <div className={styles.buttons}>
            <Button
              variant={form.hasPenalty ? 'primary' : 'secondary'}
              onClick={() => patchForm({ hasPenalty: true })}
              data-testid="categoryType-hard-button"
            >
              Со штрафом
            </Button>
            <Button
              variant={!form.hasPenalty ? 'primary' : 'secondary'}
              onClick={() => patchForm({ hasPenalty: false })}
              data-testid="categoryType-easy-button"
            >
              Без штрафа
            </Button>
          </div>
        </div>

        <Input
          type={Number(form.triggerType) === 1 ? 'time' : 'number'}
          label={Number(form.triggerType) === 1 ? 'Время' : 'Количество'}
          value={form.triggerValue}
          onChange={(event) => patchForm({ triggerValue: event.target.value })}
          data-testid="trigger-type"
        />

        <Input
          type="time"
          label="Напоминание"
          value={form.reminderTime}
          onChange={(event) => patchForm({ reminderTime: event.target.value })}
          data-testid="edit-reminder-time"
        />

        {validationError && (
          <p className={styles.errorText} data-testid="edit-validation-error">
            {validationError}
          </p>
        )}

        <div className={styles.actions}>
          <Button variant="secondary" onClick={onClose} data-testid="close-button">
            Отмена
          </Button>
          <Button
            variant="primary"
            onClick={handleSave}
            loading={actionLoadingId === habit.id}
            data-testid="save-button"
          >
            Сохранить
          </Button>
        </div>
      </div>
    </Modal>
  );
};

export default EditHabitModal;
