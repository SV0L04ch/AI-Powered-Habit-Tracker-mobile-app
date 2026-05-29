import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import styles from './CityInsightsPage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';

const CityInsightsPage = () => {
  const navigate = useNavigate();
  const [selectedCity, setSelectedCity] = useState('Москва');
  const [searchValue, setSearchValue] = useState('');
  const [cityStats, setCityStats] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const popularCities = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань'];

  // Функция для получения токена
  const getToken = () => {
    try {
      const authStorage = localStorage.getItem('auth-storage');
      if (authStorage) {
        const parsed = JSON.parse(authStorage);
        return parsed.state?.token || null;
      }
    } catch (e) {
      console.error('Ошибка при чтении токена:', e);
    }
    return null;
  };

  // Загрузка статистики при выборе города
  useEffect(() => {
    loadCityStats(selectedCity);
  }, [selectedCity]);

  const loadCityStats = async (city) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const token = getToken();
      const response = await axios.get(`/api/stats/city-summary?city=${encodeURIComponent(city)}`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {}
      });
      
      // Обрабатываем ответ в зависимости от формата
      if (response.data && response.data.stats) {
        setCityStats(response.data.stats);
      } else if (Array.isArray(response.data)) {
        setCityStats(response.data);
      } else {
        setCityStats([]);
      }
    } catch (err) {
      console.error('Ошибка загрузки статистики:', err);
      setError('Не удалось загрузить статистику для этого города');
      setCityStats([]);
    }
    
    setIsLoading(false);
  };

  const handleCitySelect = (city) => {
    setSelectedCity(city);
  };

  const handleSearch = () => {
    if (!searchValue.trim()) return;
    setSelectedCity(searchValue.trim());
    setSearchValue('');
  };

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>Статистика города</Typography>
      
      <div className={styles.searchContainer}>
        <div style={{ display: 'flex', gap: '10px' }}>
          <Input
            placeholder="Найти свой город:"
            value={searchValue}
            onChange={(e) => setSearchValue(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
            className={styles.searchInput}
          />
          <Button onClick={handleSearch} variant="primary">
            Найти
          </Button>
        </div>
      </div>

      <div className={styles.popularSection}>
        <Typography variant="headline3" className={styles.popularTitle}>Популярные города</Typography>
        <div className={styles.horizontalScroll}>
          <div className={styles.cityList}>
            {popularCities.map((city) => (
              <button
                key={city}
                className={`${styles.cityButton} ${selectedCity === city ? styles.activeCity : ''}`}
                onClick={() => handleCitySelect(city)}
              >
                {city}
              </button>
            ))}
          </div>
        </div>
      </div>

      <div className={styles.statsGrid}>
        {isLoading && <Typography>Загрузка статистики...</Typography>}
        
        {error && (
          <Typography variant="body1" style={{ color: 'red' }}>
            {error}
          </Typography>
        )}
        
        {!isLoading && !error && cityStats.length === 0 && (
          <Typography>Нет данных для города "{selectedCity}"</Typography>
        )}
        
        <div className={styles.statsList}>
          {cityStats.map((item, idx) => (
            <Substrate key={idx} variant="form" className={styles.statItemCard}>
              <div className={styles.statItemContent}>
                <div className={styles.statItemTexts}>
                  <Typography variant="headline2" className={styles.statItemPercent}>
                    {item.percent}%
                  </Typography>
                  <Typography variant="body2" className={styles.statItemDescription}>
                    {item.description}
                  </Typography>
                </div>
              </div>
            </Substrate>
          ))}
        </div>
      </div>
    </div>
  );
};

export default CityInsightsPage;