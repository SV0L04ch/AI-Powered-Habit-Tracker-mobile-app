import { useState, useRef, useCallback } from 'react';
import { motion } from 'framer-motion';
import styles from './StreakShareVideo.module.scss';

export default function StreakShareVideo({ habitName, streakCount, completions }) {
  const canvasRef = useRef(null);
  const [generating, setGenerating] = useState(false);
  const [videoUrl, setVideoUrl] = useState(null);

  const generateVideo = useCallback(async () => {
    setGenerating(true);
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    canvas.width = 400;
    canvas.height = 600;

    const frames = [];
    for (let i = 0; i < 60; i++) {
      ctx.clearRect(0, 0, 400, 600);

      const gradient = ctx.createLinearGradient(0, 0, 400, 600);
      gradient.addColorStop(0, '#faf8f5');
      gradient.addColorStop(1, '#f3ede6');
      ctx.fillStyle = gradient;
      ctx.fillRect(0, 0, 400, 600);

      ctx.fillStyle = '#1a1614';
      ctx.font = 'bold 24px Inter, sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(habitName || 'My Habit', 200, 80);

      const progress = Math.min(1, i / 30);
      const displayStreak = Math.floor(streakCount * progress);
      ctx.font = 'bold 64px Inter, sans-serif';
      ctx.fillStyle = '#d97706';
      ctx.fillText(`${displayStreak}`, 200, 200);
      ctx.font = '18px Inter, sans-serif';
      ctx.fillStyle = '#8a8279';
      ctx.fillText('Day Streak', 200, 230);

      if (i > 30) {
        const barProgress = Math.min(1, (i - 30) / 20);
        ctx.fillStyle = '#f3ede6';
        ctx.beginPath();
        ctx.roundRect(60, 280, 280, 12, 6);
        ctx.fill();
        ctx.fillStyle = '#059669';
        ctx.beginPath();
        ctx.roundRect(60, 280, 280 * barProgress, 12, 6);
        ctx.fill();
      }

      if (i > 45) {
        ctx.font = '14px Inter, sans-serif';
        ctx.fillStyle = '#8a8279';
        ctx.fillText('AI-Powered Habit Tracker', 200, 560);
      }

      frames.push(canvas.toDataURL('image/png'));
    }

    const blob = await new Promise(resolve => canvas.toBlob(resolve, 'image/png'));
    if (blob) {
      const url = URL.createObjectURL(blob);
      setVideoUrl(url);
    }
    setGenerating(false);
  }, [habitName, streakCount]);

  return (
    <div className={styles.container}>
      <canvas ref={canvasRef} className={styles.canvas} />
      <div className={styles.actions}>
        <button className={styles.generateBtn} onClick={generateVideo} disabled={generating}>
          {generating ? 'Generating...' : 'Generate Share Video'}
        </button>
        {videoUrl && (
          <a className={styles.downloadBtn} href={videoUrl} download="streak.png">
            Download
          </a>
        )}
      </div>
    </div>
  );
}
