import React from 'react';
import icons from '../../lib/icons';
import styles from './BottomNav.module.scss';

const BottomNav = ({ activeTab, onTabChange }) => {
  const tabs = [
    { id: 'habits', icon: icons.Home, label: 'Главная', testid: 'nav-button-habits' },
    { id: 'insights/personal', icon: icons.Diagram, label: 'Статистика', testid: 'nav-button-personal' },
    { id: 'insights/city', icon: icons.City, label: 'Город', testid: 'nav-button-city' },
    { id: 'profile', icon: icons.Profile, label: 'Профиль', testid: 'nav-button-profile' },
  ];

  return (
    <nav className={styles.nav} data-testid="bottom-navigation" aria-label="Основная навигация">
      {tabs.map((tab) => {
        const IconComponent = tab.icon;
        const isActive = activeTab === tab.id;

        return (
          <button
            key={tab.id}
            className={`${styles.tab} ${isActive ? styles.active : ''}`}
            onClick={() => onTabChange(tab.id)}
            aria-label={tab.label}
            aria-current={isActive ? 'page' : undefined}
            data-testid={tab.testid}
          >
            <IconComponent className={styles.icon} width={24} height={24} />
          </button>
        );
      })}
    </nav>
  );
};

export default BottomNav;
