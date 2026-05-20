import React from 'react';
import Substrate from '../Substrate/Substrate';
import images from '../../lib/images';
import styles from './HistoryCard.module.scss';
import icons from '../../lib/icons';
import Typography from '../Typography/Typography';

const HistoryCard = ({ image, date, precentage, comment }) => {
  const srcImg = images[image];
  return (
    <Substrate variant='secondary' className={styles.historyCard}>
      <img src={srcImg} width={32} height={32} alt="weather" />
      <div className={styles.content}>
        <div className={styles.cardTop}>
          <div className={styles.iconText}>
            <icons.Calendar className={styles.icon} />
            <Typography variant='body2' className={styles.dateText}>{date}</Typography>
          </div>
          <Typography variant='body2' className={styles.percentText}>{precentage}</Typography>
        </div>
        <Typography variant='caption' className={styles.commentText}>{comment}</Typography>
      </div>
    </Substrate>
  );
};

export default HistoryCard;