import React from 'react'
import styles from './Button.module.scss'

const Button = ({ children, variant, disabled = false, onClick}) => {
    const className = `${styles.btn} ${styles[`${variant}`|| '']}`
  return (
    <button className={className} disabled = {disabled} onClick={onClick}>{children}</button>
  )
}

export default Button