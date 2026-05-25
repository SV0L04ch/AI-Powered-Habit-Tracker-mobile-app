import { useEffect, useMemo, useState } from 'react';
import styles from './CityInsightsPage.module.scss';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import PageLayout from '../../components/PageLayout/PageLayout';
import useAuthUser from '../../store/useAuthStore';
import useDailySummaryStore from '../../store/useDailySummaryStore';

const QUICK_CITIES = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань'];

const CityInsightsPage = () => {
  const profileCity = useAuthUser((state) => state.city);
  const loadProfile = useAuthUser((state) => state.loadProfile);
  const { citySummary, cityLoading, cityError, fetchCitySummary } = useDailySummaryStore();
  const [searchValue, setSearchValue] = useState(profileCity || '');

  useEffect(() => {
    loadProfile();
  }, [loadProfile]);

  useEffect(() => {
    const targetCity = profileCity || searchValue || QUICK_CITIES[0];
    setSearchValue(targetCity);
    fetchCitySummary(targetCity);
  }, [fetchCitySummary, profileCity]);

  const maxUsers = useMemo(() => {
    const stats = citySummary?.popularHabits || [];
    return Math.max(...stats.map((item) => item.userCount || 0), 1);
  }, [citySummary]);

  const handleSearch = (event) => {
    event.preventDefault();
    fetchCitySummary(searchValue);
  };

  const handleCitySelect = (city) => {
    setSearchValue(city);
    fetchCitySummary(city);
  };

  return (
    <PageLayout data-testid="city-insights-page">
      <header className={styles.header} data-testid="city-header">
        <Typography variant="headline1" className={styles.title} data-testid="city-title">
          Город
        </Typography>
        <Typography variant="body1" className={styles.muted} data-testid="city-subtitle">
          Анонимная недельная сводка привычек по выбранному городу.
        </Typography>
      </header>

      <form className={styles.searchContainer} onSubmit={handleSearch} data-testid="city-search-form">
        <Input
          label="Город"
          value={searchValue}
          onChange={(event) => setSearchValue(event.target.value)}
          className={styles.searchInput}
          data-testid="city-search-input"
        />
        <Button type="submit" variant="secondary" loading={cityLoading} data-testid="city-search-button">
          Найти
        </Button>
      </form>

      <section className={styles.popularSection} data-testid="popular-cities-section">
        <Typography variant="headline3" className={styles.popularTitle}>
          Быстрый выбор
        </Typography>
        <div className={styles.horizontalScroll} data-testid="popular-cities-scroll">
          {QUICK_CITIES.map((city) => (
            <button
              key={city}
              className={`${styles.cityButton} ${searchValue === city ? styles.activeCity : ''}`}
              onClick={() => handleCitySelect(city)}
              data-testid={`city-chip-${city}`}
              type="button"
            >
              {city}
            </button>
          ))}
        </div>
      </section>

      {cityError && (
        <p className={styles.error} data-testid="city-error">
          {cityError}
        </p>
      )}

      {cityLoading && (
        <div className={styles.skeletonList} data-testid="city-summary-loader">
          <div />
          <div />
          <div />
        </div>
      )}

      {citySummary && !cityLoading && (
        <section className={styles.statsGrid} data-testid="city-summary-section">
          <article className={styles.cityHero} data-testid="city-summary-card">
            <span>Неделя</span>
            <Typography variant="headline2" data-testid="city-summary-name">
              {citySummary.city}
            </Typography>
            <Typography variant="body2" className={styles.muted} data-testid="city-summary-dates">
              {citySummary.weekStartDate} - {citySummary.weekEndDate}
            </Typography>
          </article>

          <div className={styles.statsList} data-testid="city-habits-list">
            {citySummary.popularHabits?.length ? (
              citySummary.popularHabits.map((item, index) => (
                <article className={styles.statItemCard} key={`${item.habitName}-${index}`} data-testid={`city-habit-${index}`}>
                  <div className={styles.statItemContent}>
                    <div className={styles.rank} data-testid={`city-habit-${index}-rank`}>
                      {index + 1}
                    </div>
                    <div className={styles.statItemTexts}>
                      <Typography variant="headline3" data-testid={`city-habit-${index}-name`}>
                        {item.habitName}
                      </Typography>
                      <Typography variant="body2" className={styles.muted} data-testid={`city-habit-${index}-users`}>
                        {item.userCount} из {item.totalUsers} пользователей
                      </Typography>
                      <div className={styles.bar} data-testid={`city-habit-${index}-bar`}>
                        <span style={{ width: `${Math.round(((item.userCount || 0) / maxUsers) * 100)}%` }} />
                      </div>
                    </div>
                  </div>
                </article>
              ))
            ) : (
              <article className={styles.emptyCard} data-testid="city-empty-state">
                <Typography variant="headline3">Данных пока нет</Typography>
                <Typography variant="body2" className={styles.muted}>
                  Когда пользователи отметят привычки в этом городе, топ появится здесь.
                </Typography>
              </article>
            )}
          </div>
        </section>
      )}
    </PageLayout>
  );
};

export default CityInsightsPage;
