import { useMemo } from 'react';
import styles from './QRCode.module.scss';

export default function QRCode({ value, size = 200 }) {
  const qrSvg = useMemo(() => {
    if (!value) return null;
    const modules = generateQRMatrix(value);
    const cellSize = size / modules.length;

    return (
      <svg viewBox={`0 0 ${size} ${size}`} width={size} height={size} className={styles.qr}>
        <rect width={size} height={size} fill="white" rx="12" />
        {modules.map((row, y) =>
          row.map((cell, x) =>
            cell ? (
              <rect
                key={`${x}-${y}`}
                x={x * cellSize}
                y={y * cellSize}
                width={cellSize}
                height={cellSize}
                fill="#1a1614"
                rx={cellSize * 0.15}
              />
            ) : null
          )
        )}
      </svg>
    );
  }, [value, size]);

  return <div className={styles.container}>{qrSvg}</div>;
}

function generateQRMatrix(text) {
  const size = 21;
  const matrix = Array.from({ length: size }, () => Array(size).fill(0));

  for (let i = 0; i < 7; i++) {
    for (let j = 0; j < 7; j++) {
      if (i === 0 || i === 6 || j === 0 || j === 6 || (i >= 2 && i <= 4 && j >= 2 && j <= 4)) {
        matrix[i][j] = 1;
      }
    }
  }

  let hash = 0;
  for (let i = 0; i < text.length; i++) {
    hash = ((hash << 5) - hash + text.charCodeAt(i)) | 0;
  }

  for (let i = 8; i < size - 8; i++) {
    for (let j = 8; j < size; j++) {
      matrix[i][j] = ((hash >> ((i + j) % 31)) & 1);
    }
  }

  for (let i = size - 7; i < size; i++) {
    for (let j = 0; j < 7; j++) {
      if (i === size - 7 || i === size - 1 || j === 0 || j === 6 || (i >= size - 5 && i <= size - 3 && j >= 2 && j <= 4)) {
        matrix[i][j] = 1;
      }
    }
  }

  return matrix;
}
