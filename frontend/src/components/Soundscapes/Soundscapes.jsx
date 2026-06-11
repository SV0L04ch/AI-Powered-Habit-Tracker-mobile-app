import { useState, useRef, useEffect } from 'react';
import { motion } from 'framer-motion';
import styles from './Soundscapes.module.scss';

const sounds = [
  { id: 'rain', name: 'Rain', icon: '🌧️', frequency: 200 },
  { id: 'forest', name: 'Forest', icon: '🌲', frequency: 300 },
  { id: 'ocean', name: 'Ocean', icon: '🌊', frequency: 150 },
  { id: 'fire', name: 'Fireplace', icon: '🔥', frequency: 400 },
  { id: 'cafe', name: 'Coffee Shop', icon: '☕', frequency: 350 },
];

export default function Soundscapes({ onClose }) {
  const [activeSound, setActiveSound] = useState(null);
  const [volume, setVolume] = useState(50);
  const audioCtxRef = useRef(null);
  const oscillatorRef = useRef(null);
  const gainRef = useRef(null);

  const startSound = (sound) => {
    stopSound();
    const ctx = new (window.AudioContext || window.webkitAudioContext)();
    const osc = ctx.createOscillator();
    const gain = ctx.createGain();
    osc.type = 'sine';
    osc.frequency.value = sound.frequency;
    gain.gain.value = volume / 200;
    osc.connect(gain);
    gain.connect(ctx.destination);
    osc.start();
    audioCtxRef.current = ctx;
    oscillatorRef.current = osc;
    gainRef.current = gain;
    setActiveSound(sound.id);
  };

  const stopSound = () => {
    oscillatorRef.current?.stop();
    audioCtxRef.current?.close();
    oscillatorRef.current = null;
    audioCtxRef.current = null;
    gainRef.current = null;
    setActiveSound(null);
  };

  useEffect(() => {
    return () => stopSound();
  }, []);

  useEffect(() => {
    if (gainRef.current) {
      gainRef.current.gain.value = volume / 200;
    }
  }, [volume]);

  return (
    <motion.div
      className={styles.overlay}
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      exit={{ opacity: 0 }}
    >
      <motion.div
        className={styles.panel}
        initial={{ y: '100%' }}
        animate={{ y: 0 }}
        exit={{ y: '100%' }}
        transition={{ type: 'spring', damping: 25 }}
      >
        <div className={styles.header}>
          <h3 className={styles.title}>Soundscapes</h3>
          <button className={styles.close} onClick={onClose}>✕</button>
        </div>

        <div className={styles.grid}>
          {sounds.map((sound) => (
            <button
              key={sound.id}
              className={`${styles.soundCard} ${activeSound === sound.id ? styles.active : ''}`}
              onClick={() => activeSound === sound.id ? stopSound() : startSound(sound)}
            >
              <span className={styles.soundIcon}>{sound.icon}</span>
              <span className={styles.soundName}>{sound.name}</span>
              {activeSound === sound.id && <div className={styles.playing}>♫</div>}
            </button>
          ))}
        </div>

        <div className={styles.volumeControl}>
          <span className={styles.volumeLabel}>Volume</span>
          <input
            type="range"
            min="0"
            max="100"
            value={volume}
            onChange={(e) => setVolume(Number(e.target.value))}
            className={styles.slider}
          />
          <span className={styles.volumeValue}>{volume}%</span>
        </div>
      </motion.div>
    </motion.div>
  );
}
