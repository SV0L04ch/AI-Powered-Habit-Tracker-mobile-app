import React from 'react'
import styles from './Input.module.scss'

const Input = ({placeholder = "Введите текст", type = "text", icon, disabled = false, ...rest}) => {
  return (
    <label className={styles.inputContainer}>
        {icon && (
            <span className={styles.icon} aria-hidden='true'>
                {icon}
            </span>
            )}
        <input type={type} placeholder={placeholder} disabled = {disabled} className={styles.field} {...rest}/>
    </label>
  )
}

export default Input