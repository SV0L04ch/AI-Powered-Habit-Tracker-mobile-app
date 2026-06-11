import { useState } from 'react';
import { motion } from 'framer-motion';
import styles from './MoodPicker.module.scss';

const moods = [
  { value: 5, emoji: '😊', label: 'Great' },
  { value: 4, emoji: '🙂', label: 'Good' },
  { value: 3, emoji: '😐', label: 'Okay' },
  { value: 2, emoji: '😔', label: 'Low' },
  { value: 1, emoji: '😤', label: 'Bad' },
];

export default function MoodPicker({ selected, onSelect }) {
  return (
    <div className={styles.container}>
      <span className={styles.label}>How are you feeling?</span>
      <div className={styles.grid}>
        {moods.map((mood) => (
          <motion.button
            key={mood.value}
            className={`${styles.moodBtn} ${selected === mood.value ? styles.selected : ''}`}
            onClick={() => onSelect(mood.value)}
            whileTap={{ scale: 0.85 }}
            whileHover={{ scale: 1.1 }}
          >
            <span className={styles.emoji}>{mood.emoji}</span>
            <span className={styles.moodLabel}>{mood.label}</span>
          </motion.button>
        ))}
      </div>
    </div>
  );
}
