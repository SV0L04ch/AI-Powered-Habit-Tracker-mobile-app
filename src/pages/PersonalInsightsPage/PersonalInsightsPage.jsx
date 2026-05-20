import React from 'react'
import Typography from '../../components/Typography/Typography'
import Substrate from '../../components/Substrate/Substrate'
import PageLayout from '../../components/PageLayout/PageLayout'
import icons from '../../lib/icons'
import images from '../../lib/images'
import styles from './PersonalInsights.module.scss'
import HistoryCard from '../../components/HistoryCard/HistoryCard'

function HabitsPage() {

  const transparency = `${styles.basicText} ${styles.transparancy}`
  return (
    <PageLayout>
      <Typography variant='headline1' className={styles.mainTitle}>Аналитика</Typography>

        <div className={styles.cards}>

          <Substrate>
            <div className={styles.card}>
              <div className={styles.cardLeft}>
                <Typography variant='body1' className={transparency}>Сегодня</Typography>
                <Typography variant='headline3' className={styles.mainText}>90%</Typography>
                <Typography variant='body1' className={transparency}>Продуктивность</Typography>
              </div>

              <div className={styles.cardRight}>
                <Typography variant='body1' className={transparency}>Солнечно <br /> +23°C</Typography>
                <img src={images.Sun} alt="" width={29} height={29}/>
              </div>
            </div>

          </Substrate>

          <Substrate>
            <Typography variant='body1' className={styles.mainText}>Отличный день! Солнечная погода сопоставляется с вашим пиком активности в 9:00</Typography>
          </Substrate>

        </div>
        <div className={styles.cards}>
          <div className={styles.blockHistory}>
            <icons.History/>
            <Typography variant='headline1' className={styles.mainText}>История</Typography>
          </div>
          <HistoryCard image="Rain"date="Вчера, 11 апр." precentage="95%" comment="Идеальные условия для активности. Вы закрыли все утренние привычки до 10:00"></HistoryCard>
        </div>
    </PageLayout>
  )
}

export default HabitsPage