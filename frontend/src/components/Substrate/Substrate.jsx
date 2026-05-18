import React from 'react'
import styles from './Substrate.module.scss'
import images from '../../lib/images'
import icons from '../../lib/icons'

const Substrate = ({children, variant = "main", icon, image, alt = "Картинка:)"}) => {
    const className = `${styles.sub} ${styles[variant] || ''}`
    const imgSrc = images[image]
    const IconComponent = icons[icon]

  return (
    <div className={className}>

      {IconComponent && <IconComponent className={styles.icon}/>}

      {imgSrc && <img src={imgSrc} alt = {alt} className={styles.image}></img>}
      
      {children}
      </div>
  )
}

export default Substrate