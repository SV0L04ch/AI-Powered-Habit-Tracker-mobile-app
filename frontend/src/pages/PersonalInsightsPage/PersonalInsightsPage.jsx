import { useEffect, useMemo } from 'react';
import Typography from '../../components/Typography/Typography';
import PageLayout from '../../components/PageLayout/PageLayout';
import images from '../../lib/images';
import useDailySummaryStore from '../../store/useDailySummaryStore';
import styles from './PersonalInsights.module.scss';

const getWeatherImage = (condition) => {
  const normalized = String(condition || '').toLowerCase();
  if (normalized.includes('rain')) return images.Rain;
  if (normalized.includes('cloud')) return images.Clouds;
  return images.Sun;
};

function PersonalInsightsPage() {
  const { summary, isLoading, error, fetchStats } = useDailySummaryStore();

  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  const totals = useMemo(() => {
    const completed = summary?.habitsCompleted || 0;
    const partial = summary?.habitsPartiallyCompleted || 0;
    const skipped = summary?.habitsSkipped || 0;
    const total = completed + partial + skipped;
    return {
      completed,
      partial,
      skipped,
      total,
      percent: total ? Math.round((completed / total) * 100) : 0,
    };
  }, [summary]);

  return (
    <PageLayout data-testid="personal-insights-page">
      <header className={styles.header} data-testid="insights-header">
        <Typography variant="headline1" data-testid="insights-title">
          Аналитика
        </Typography>
        <Typography variant="body1" className={styles.muted} data-testid="insights-subtitle">
          Сводка строится по реальным отметкам, погоде и комментарию дня.
        </Typography>
      </header>

      {isLoading && (
        <div className={styles.skeletonGrid} data-testid="insights-loader">
          <div />
          <div />
          <div />
        </div>
      )}

      {error && (
        <p className={styles.error} data-testid="server-error">
          {error}
        </p>
      )}

      {summary && (
        <section className={styles.cards} data-testid="daily-summary-section">
          <article className={styles.heroCard} data-testid="daily-summary-card">
            <div>
              <span className={styles.label}>Сегодня</span>
              <Typography variant="headline1" data-testid="productivity-percent">
                {totals.percent}%
              </Typography>
              <Typography variant="body2" className={styles.muted}>
                продуктивность
              </Typography>
            </div>
            <img src={getWeatherImage(summary.weather?.condition)} alt="" data-testid="weather-image" />
          </article>

          <div className={styles.metrics} data-testid="summary-metrics">
            <article data-testid="completed-metric">
              <span>Выполнено</span>
              <strong>{totals.completed}</strong>
            </article>
            <article data-testid="partial-metric">
              <span>Частично</span>
              <strong>{totals.partial}</strong>
            </article>
            <article data-testid="skipped-metric">
              <span>Пропущено</span>
              <strong>{totals.skipped}</strong>
            </article>
          </div>

          <article className={styles.aiCard} data-testid="ai-summary-card">
            <span className={styles.label}>Комментарий дня</span>
            <Typography variant="body1" data-testid="ai-summary-text">
              {summary.aiInsight}
            </Typography>
          </article>

          <article className={styles.weatherCard} data-testid="weather-details-card">
            <div>
              <span className={styles.label}>Погода</span>
              <Typography variant="headline3" data-testid="weather-city">
                {summary.weather?.city || 'Город не указан'}
              </Typography>
              <Typography variant="body2" className={styles.muted} data-testid="weather-condition">
                {summary.weather?.condition || 'Нет данных'}
              </Typography>
            </div>
            <div className={styles.temperature} data-testid="weather-temperature">
              {summary.weather?.temperatureCelsius != null ? `${summary.weather.temperatureCelsius}°C` : '-'}
            </div>
          </article>
        </section>
      )}
    </PageLayout>
  );
}

export default PersonalInsightsPage;
