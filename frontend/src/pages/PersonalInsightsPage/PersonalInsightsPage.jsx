import React, { useEffect } from 'react';
import Typography from '../../components/Typography/Typography';
import Substrate from '../../components/Substrate/Substrate';
import PageLayout from '../../components/PageLayout/PageLayout';
import icons from '../../lib/icons';
import images from '../../lib/images';
import styles from './PersonalInsights.module.scss';
import HistoryCard from '../../components/HistoryCard/HistoryCard';
import useDailySummaryStore from '../../store/useDailySummaryStore';

function PersonalInsightsPage() {
  const {summary, isLoading, error, fetchStats} = useDailySummaryStore()

  useEffect(() => {
    const today = new Date().toISOString().slice(0, 10)
    fetchStats(today)
  }, [fetchStats])

  const productivityPercent = summary && summary.habitsCompleted + summary.habitsPartiallyCompleted + summary.habitsSkipped > 0 ? 
  Math.round((summary.habitsCompleted / (summary.habitsCompleted + summary.habitsPartiallyCompleted + summary.habitsSkipped)) * 100) : null

  const transparency = `${styles.basicText} ${styles.transparancy}`
  return (
    <PageLayout>
      <Typography variant='headline1' className={styles.mainText}>Аналитика</Typography>

      {isLoading && <Typography variant='body1'>Загрузка...</Typography>}
      {error && (<Typography variant='body1'>{error}</Typography>)}

      {summary &&
        (
          <div className={styles.cards}>

            <Substrate>
              <div className={styles.card}>
                <div className={styles.cardLeft}>
                  <Typography variant='body1' className={transparency}>Сегодня</Typography>
                  <Typography variant='headline3' className={styles.mainText}> 
                    {productivityPercent !== null ? `${productivityPercent}%` : '—'}
                  </Typography>
                  <Typography variant='body1' className={transparency}>Продуктивность</Typography>
                </div>

                <div className={styles.cardRight}>
                  <Typography variant='body1' className={transparency}>
                    {summary.weather?.condition || 'Нет данных'} <br />{' '}
                    {summary.weather?.temperatureCelsius != null
                      ? `${summary.weather.temperatureCelsius}°C`
                      : ''}
                  </Typography>
                  <img src={images.Sun} alt="" width={29} height={29}/>
                </div>
              </div>
            </Substrate>

            <Substrate>
              <Typography variant='body1' className={styles.mainText}>{summary.aiInsight || 'Отличный день! Сегодня всё получится.'}</Typography>
            </Substrate>

          </div>
        )
      }
        <div className={styles.cards}>
          <div className={styles.blockHistory}>
            <icons.History />
            <Typography variant='headline1' className={styles.mainText}>История</Typography>
          </div>
          <HistoryCard image="Rain"date="Вчера, 11 апр." precentage="95%" comment="Идеальные условия для активности. Вы закрыли все утренние привычки до 10:00"></HistoryCard>
        </div>
    </PageLayout>
  )
}

export default PersonalInsightsPage