import { useState, useEffect, useRef, useCallback } from 'react';
import { motion } from 'framer-motion';
import styles from './MeditationTimer.module.scss';

export default function MeditationTimer({ onClose }) {
  const [duration, setDuration] = useState(300);
  const [timeLeft, setTimeLeft] = useState(300);
  const [isRunning, setIsRunning] = useState(false);
  const [phase, setPhase] = useState('inhale');
  const intervalRef = useRef(null);

  useEffect(() => {
    if (isRunning && timeLeft > 0) {
      intervalRef.current = setInterval(() => {
        setTimeLeft(prev => {
          if (prev <= 1) {
            setIsRunning(false);
            clearInterval(intervalRef.current);
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }
    return () => clearInterval(intervalRef.current);
  }, [isRunning, timeLeft]);

  useEffect(() => {
    if (!isRunning) return;
    const breatheInterval = setInterval(() => {
      setPhase(prev => prev === 'inhale' ? 'exhale' : 'inhale');
    }, 4000);
    return () => clearInterval(breatheInterval);
  }, [isRunning]);

  const toggleTimer = useCallback(() => {
    if (timeLeft === 0) setTimeLeft(duration);
    setIsRunning(prev => !prev);
  }, [timeLeft, duration]);

  const minutes = Math.floor(timeLeft / 60);
  const seconds = timeLeft % 60;
  const progress = ((duration - timeLeft) / duration) * 100;

  return (
    <motion.div
      className={styles.overlay}
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      exit={{ opacity: 0 }}
    >
      <div className={styles.container}>
        <button className={styles.close} onClick={onClose}>✕</button>

        <motion.div
          className={`${styles.breatheCircle} ${isRunning ? styles.animating : ''}`}
          animate={isRunning ? {
            scale: phase === 'inhale' ? 1.2 : 0.8,
          } : {}}
          transition={{ duration: 4, ease: 'easeInOut' }}
        >
          <div className={styles.timer}>
            <span className={styles.time}>{minutes}:{seconds.toString().padStart(2, '0')}</span>
            <span className={styles.phase}>{isRunning ? (phase === 'inhale' ? 'Breathe In' : 'Breathe Out') : 'Ready'}</span>
          </div>
        </motion.div>

        <div className={styles.progressRing}>
          <svg viewBox="0 0 100 100">
            <circle cx="50" cy="50" r="45" fill="none" stroke="var(--app-bg-soft)" strokeWidth="4" />
            <circle
              cx="50" cy="50" r="45" fill="none"
              stroke="var(--app-primary)" strokeWidth="4"
              strokeDasharray={`${2 * Math.PI * 45}`}
              strokeDashoffset={`${2 * Math.PI * 45 * (1 - progress / 100)}`}
              strokeLinecap="round"
              transform="rotate(-90 50 50)"
            />
          </svg>
        </div>

        <div className={styles.durations}>
          {[1, 3, 5, 10].map(min => (
            <button
              key={min}
              className={`${styles.durBtn} ${duration === min * 60 ? styles.active : ''}`}
              onClick={() => { setDuration(min * 60); setTimeLeft(min * 60); setIsRunning(false); }}
            >
              {min}m
            </button>
          ))}
        </div>

        <button className={styles.startBtn} onClick={toggleTimer}>
          {isRunning ? 'Pause' : timeLeft === 0 ? 'Restart' : 'Start'}
        </button>
      </div>
    </motion.div>
  );
}
