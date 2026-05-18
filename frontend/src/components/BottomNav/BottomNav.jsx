import React from 'react'
import icons from '../../lib/icons'
import styles from './BottomNav.module.scss'

const BottomNav = ({activeTab, onTabChange}) => {

  const tabs = [
    { id: 'habits', icon: icons.Home, label: 'Главная'},
    { id: 'insights/personal', icon: icons.Diagram, label: 'Статистика'},
    { id: 'insights/city', icon: icons.City, label: 'Город'},
    { id: 'profile', icon: icons.Profile, label: 'Профиль'}
  ]

  return (
    <nav className={styles.nav}>
      {tabs.map((tab) => {
        const IconComponent = tab.icon
        const isActive = activeTab === tab.id

        return (
          <button
            key={tab.id}
            className={`${styles.tab} ${isActive ? styles.active : ''}`}
            onClick={() => onTabChange(tab.id)}
            aria-label={tab.label}
            >
              <IconComponent className={styles.icon} width={32} height={32}></IconComponent>
          </button>
        )
      })}
    </nav>
  )
}

export default BottomNav