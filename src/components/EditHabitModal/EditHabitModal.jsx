import React, { useState, useEffect } from 'react';
import Modal from '../Modal/Modal';
import Input from '../Input/Input';
import Button from '../Button/Button';
import Typography from '../Typography/Typography';
import icons from '../../lib/icons';
import useHabitsStore from '../../store/useHabitsStore';
import styles from './EditHabitModal.module.scss';

const EditHabitModal = ({ isOpen, onClose, habit }) => {
  const [name, setName] = useState('');
  const [triggerValue, setTriggerValue] = useState('');
  const [type, setType] = useState(false);       // false = Время, true = Повторы
  const [category, setCategory] = useState(false); // false = Легко, true = Тяжело

  const updateHabit = useHabitsStore((state) => state.updateHabit);

  useEffect(() => {
    if (habit) {
      setName(habit.name || '');
      setTriggerValue(habit.triggerValue || '');
      setType(habit.type ?? false);
      setCategory(habit.category ?? false);
    }
  }, [habit, isOpen]);

  const handleSave = async () => {
    const updates = {
      name,
      triggerValue,
      type,
      category,
      triggerType: type ? 2 : 1,
    };
    await updateHabit(habit.id, updates);
    onClose();
  };

  if (!habit) return null;

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <div className={styles.container}>
        <Typography variant="headline2">Редактирование привычки</Typography>

        <Input
          placeholder="Название"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />

        {/* Тип контроля */}
        <div className={styles.section}>
          <Typography variant="body2" className={styles.label}>Тип контроля</Typography>
          <div className={styles.buttons}>
            <Button
              variant={!type ? 'primary' : 'secondary'}
              onClick={() => setType(false)}
            >
              <div className={styles.optionContent}>
                <icons.Wristwatch />
                <Typography variant="body2">Время</Typography>
              </div>
            </Button>
            <Button
              variant={type ? 'primary' : 'secondary'}
              onClick={() => setType(true)}
            >
              <div className={styles.optionContent}>
                <icons.Count />
                <Typography variant="body2">Повторы</Typography>
              </div>
            </Button>
          </div>
        </div>

        {/* Сложность */}
        <div className={styles.section}>
          <Typography variant="body2" className={styles.label}>Сложность</Typography>
          <div className={styles.buttons}>
            <Button
              variant={category ? 'primary' : 'secondary'}
              onClick={() => setCategory(true)}
            >
              <div className={styles.optionContent}>
                <div className={styles.stars}>
                  <icons.FillStar />
                  <icons.FillStar />
                  <icons.FillStar />
                </div>
                <Typography variant="body2">Тяжело</Typography>
              </div>
            </Button>
            <Button
              variant={!category ? 'primary' : 'secondary'}
              onClick={() => setCategory(false)}
            >
              <div className={styles.optionContent}>
                <div className={styles.stars}>
                  <icons.EmptyStar />
                  <icons.EmptyStar />
                  <icons.EmptyStar />
                </div>
                <Typography variant="body2">Легко</Typography>
              </div>
            </Button>
          </div>
        </div>

        <Input
          placeholder="Значение (время или количество)"
          value={triggerValue}
          onChange={(e) => setTriggerValue(e.target.value)}
        />

        <div className={styles.actions}>
          <Button variant="secondary" onClick={onClose}>Отмена</Button>
          <Button variant="primary" onClick={handleSave}>Сохранить</Button>
        </div>
      </div>
    </Modal>
  );
};

export default EditHabitModal;