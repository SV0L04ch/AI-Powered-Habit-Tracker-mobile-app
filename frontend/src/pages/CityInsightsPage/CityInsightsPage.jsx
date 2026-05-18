import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './CityInsightsPage.module.scss';
import Substrate from '../../components/Substrate/Substrate';
import Button from '../../components/Button/Button';
import Typography from '../../components/Typography/Typography';
import Input from '../../components/Input/Input';
import icons from '../../lib/icons';

const CityInsightsPage = () => {
  const navigate = useNavigate();
  const [selectedCity, setSelectedCity] = useState('Москва');
  const [searchValue, setSearchValue] = useState('');
  const [stats, setStats] = useState({ totalUsers: 0, completionRate: 0 });
  const [cityStats, setCityStats] = useState([]);

  const popularCities = ['Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань'];

  
  const cityDataMap = {
    Москва: {
      stats: [
        { percent: 55, description: 'Бегали в парках до 10 утра' , icons: 'Sneakers'},
        { percent: 20, description: 'Читали книги перед сном', icons: 'Book' },
        { percent: 15, description: 'Практиковали осознанность утром', icons: 'Moon' },
      ],
    },
    'Санкт-Петербург': {
      
      stats: [
        { percent: 40, description: 'Посещали музеи в выходные' },
        { percent: 35, description: 'Гуляли пяо набережным' },
        { percent: 25, description: 'Пили кофе в уютных кофейнях' },
      ],
    },
    Новосибирск: {
      
      stats: [
        { percent: 30, description: 'Занимались спортом' },
        { percent: 25, description: 'Читали книги' },
        { percent: 20, description: 'Медитировали' },
      ],
    },
    Екатеринбург: {
      
      stats: [
        { percent: 35, description: 'Ходили в тренажёрный зал' },
        { percent: 28, description: 'Гуляли на свежем воздухе' },
        { percent: 22, description: 'Учились новому' },
      ],
    },
    Казань: {
      
      stats: [
        { percent: 33, description: 'Посещали мероприятия' },
        { percent: 27, description: 'Занимались творчеством' },
        { percent: 20, description: 'Общались с друзьями' },
      ],
    },
  };

  useEffect(() => {
    const currentUser = JSON.parse(localStorage.getItem('currentUser'));

    loadCityData(selectedCity);
  }, [navigate, selectedCity]);

  const loadCityData = (city) => {
    const data = cityDataMap[city] || cityDataMap['Москва'];
    setStats({
      totalUsers: data.totalUsers,
      completionRate: data.completionRate,
    });
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