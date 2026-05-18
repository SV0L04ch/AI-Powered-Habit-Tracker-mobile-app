import React from 'react';
import styles from './Button.module.scss';

const Button = ({ children, variant, disabled = false, type = "button", onClick, ...rest}) => {
    const className = `${styles.btn} ${styles[`${variant}`|| '']}`
  return (
    <button type={type} className={className} disabled = {disabled} onClick={onClick} {...rest}>{children}</button>
  )
}

export default Button;