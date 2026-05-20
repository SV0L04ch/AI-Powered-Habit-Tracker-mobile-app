import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './CityInsightsPage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import icons from '../../lib/icons';

const CityInsightsPage = () => {
  const navigate = useNavigate();
  const [selectedCity, setSelectedCity] = useState('Москва');
  const [searchValue, setSearchValue] = useState('');
  const [cityStats, setCityStats] = useState([]);

  const popularCities = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань'];

  const cityDataMap = {
    Москва: {
      stats: [
        { percent: 55, description: 'Бегали в парках до 10 утра', icon: 'Sneakers' },
        { percent: 20, description: 'Читали книги перед сном', icon: 'Book' },
        { percent: 15, description: 'Практиковали осознанность утром', icon: 'Moon' },
      ],
    },
    'Санкт-Петербург': {
      stats: [
        { percent: 40, description: 'Посещали музеи в выходные', icon: 'MapPoint' },
        { percent: 35, description: 'Гуляли по набережным', icon: 'City' },
        { percent: 25, description: 'Пили кофе в уютных кофейнях', icon: 'Notification' },
      ],
    },
    Новосибирск: {
      stats: [
        { percent: 30, description: 'Занимались спортом', icon: 'Sneakers' },
        { percent: 25, description: 'Читали книги', icon: 'Book' },
        { percent: 20, description: 'Медитировали', icon: 'Moon' },
      ],
    },
    Екатеринбург: {
      stats: [
        { percent: 35, description: 'Ходили в тренажёрный зал', icon: 'Sneakers' },
        { percent: 28, description: 'Гуляли на свежем воздухе', icon: 'MapPoint' },
        { percent: 22, description: 'Учились новому', icon: 'Book' },
      ],
    },
    Казань: {
      stats: [
        { percent: 33, description: 'Посещали мероприятия', icon: 'City' },
        { percent: 27, description: 'Занимались творчеством', icon: 'Sneakers' },
        { percent: 20, description: 'Общались с друзьями', icon: 'Notification' },
      ],
    },
  };

  useEffect(() => {
    const currentUser = JSON.parse(localStorage.getItem('currentUser'));
    if (!currentUser) navigate('/login');
    loadCityData(selectedCity);
  }, [selectedCity, navigate]);

  const loadCityData = (city) => {
    const data = cityDataMap[city] || cityDataMap['Москва'];
    setCityStats(data.stats || []);
  };

  const handleCitySelect = (city) => {
    setSelectedCity(city);
    loadCityData(city);
  };

  const handleSearch = () => {
    if (searchValue.trim()) {
      handleCitySelect(searchValue.trim());
      setSearchValue('');
    }
  };

  return (
    <div className={styles.page}>
      <Typography variant="headline1" className={styles.title}>Статистика города</Typography>

      <div className={styles.searchContainer}>
        <Input
          placeholder="Найти свой город:"
          value={searchValue}
          onChange={(e) => setSearchValue(e.target.value)}
          className={styles.searchInput}
        />
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
        <div className={styles.statsList}>
          {cityStats.map((item, idx) => {
            const IconComponent = icons[item.icon];
            return (
              <Substrate key={idx} variant="form" className={styles.statItemCard}>
                <div className={styles.statItemContent}>
                  {IconComponent && <IconComponent className={styles.statIcon} />}
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
            );
          })}
        </div>
      </div>
    </div>
  );
};

export default CityInsightsPage;