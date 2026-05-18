import React from 'react'
import Substrate from '../Substrate/Substrate'
import images from '../../lib/images'
import styles from './HistoryCard.module.scss'
import icons from '../../lib/icons'
import Typography from '../Typography/Typography'


const HistoryCard = ({image, date, precentage, comment}) => {
    const srcImg = images[image]
  return (
    <Substrate variant='secondary'>
         <img src={srcImg} width={32} height={32}/>
            <div className={styles.historyCard}>
              <div className={styles.cardTop}>
                <div className={styles.icon}>
                  <icons.Calendar />
                  <Typography variant='body2'>{date}</Typography>
                </div>
                <Typography variant='body2'>{precentage}</Typography>
              </div>
              <Typography variant='caption' className={styles.basicText}>{comment}</Typography>
            </div>
    </Substrate>
  )
}

export default HistoryCard