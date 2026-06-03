import { useState, useEffect } from 'react';
import styles from './CityInsightsPage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import useCitySummaryStore from '../../store/useCitySummaryStore';
import useProfileStore from '../../store/useProfileStore';

const cityMap = {
  Moscow: 'Москва',
  Spb: 'Санкт-Петербург',
  Novosibirsk: 'Новосибирск',
  Ekaterinburg: 'Екатеринбург',
  Kazan: 'Казань',
};

const popularCityKeys = Object.keys(cityMap); // ['moscow', 'spb', ...]

const CityInsightsPage = () => {
  const [selectedCityKey, setSelectedCityKey] = useState('Moscow');
  const [searchValue, setSearchValue] = useState('');
  const profileCity = useProfileStore((state) => state.city);

  const { data, isLoading, error, fetchCitySummary } = useCitySummaryStore();

  useEffect(() => {
    if (profileCity) {
      const foundKey = Object.keys(cityMap).find(
        (key) => cityMap[key].toLowerCase() === profileCity.toLowerCase()
      );
      if (foundKey) {
        setSelectedCityKey(foundKey);
      }
    }
  }, [profileCity]);

  useEffect(() => {
    fetchCitySummary(selectedCityKey);
  }, [selectedCityKey, fetchCitySummary]);

  const handleCitySelect = (cityKey) => {
    setSelectedCityKey(cityKey);
  };

  const handleSearch = () => {
    const trimmed = searchValue.trim();
    if (!trimmed) return;
    // Ищем ключ по русскому названию
    const foundKey = Object.keys(cityMap).find(
      (key) => cityMap[key].toLowerCase() === trimmed.toLowerCase()
    );
    if (foundKey) {
      handleCitySelect(foundKey);
    } else {
      // если город не найден, можно показать уведомление
      alert('Город не найден');
    }
    setSearchValue('');
  };

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>
        Статистика города
      </Typography>

      <div className={styles.searchContainer}>
        <Input
          placeholder="Найти свой город:"
          value={searchValue}
          onChange={(e) => setSearchValue(e.target.value)}
          className={styles.searchInput}
        />
      </div>

      <div className={styles.popularSection}>
        <Typography variant="headline3" className={styles.popularTitle}>
          Популярные города
        </Typography>
        <div className={styles.horizontalScroll}>
          <div className={styles.cityList}>
            {popularCityKeys.map((cityKey) => (
              <button
                key={cityKey}
                className={`${styles.cityButton} ${selectedCityKey === cityKey ? styles.activeCity : ''}`}
                onClick={() => handleCitySelect(cityKey)}
              >
                {cityMap[cityKey]}
              </button>
            ))}
          </div>
        </div>
      </div>

      <div className={styles.statsGrid}>
        {isLoading && <Typography variant="body1">Загрузка...</Typography>}
        {error && (
          <Typography variant="body1" style={{ color: 'red' }}>
            {error}
          </Typography>
        )}

        {data && !isLoading && !error && (
          <div className={styles.statsList}>
            {data.popularHabits.map((habit, idx) => (
              <Substrate key={idx} variant="form" className={styles.statItemCard}>
                <div className={styles.statItemContent}>
                  <div className={styles.statItemTexts}>
                    <Typography variant="headline2" className={styles.statItemPercent}>
                      {habit.percentage}%
                    </Typography>
                    <Typography variant="body2" className={styles.statItemDescription}>
                      {habit.habitName}
                    </Typography>
                    <Typography variant="caption">
                      {habit.userCount} пользоватеaлей
                    </Typography>
                  </div>
                </div>
              </Substrate>
            ))}
          </div>
        )}

        {data && data.popularHabits.length === 0 && (
          <Typography variant="body1">Нет данных по выбранному городу</Typography>
        )}
      </div>
    </div>
  );
};

export default CityInsightsPage;