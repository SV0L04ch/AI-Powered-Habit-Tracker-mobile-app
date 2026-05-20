import React from 'react';
import styles from './Button.module.scss';

<<<<<<< HEAD
const Button = ({ children, variant, disabled = false, type = "button", onClick, ...rest}) => {
    const className = `${styles.btn} ${styles[`${variant}`|| '']}`
  return (
    <button type={type} className={className} disabled = {disabled} onClick={onClick} {...rest}>{children}</button>
  )
}
=======
const Button = ({ children, variant, disabled = false, onClick, className, ...rest }) => {
  const classes = `${styles.btn} ${styles[variant] || ''} ${className || ''}`.trim();
  return (
    <button className={classes} disabled={disabled} onClick={onClick} {...rest}>
      {children}
    </button>
  );
};
>>>>>>> feature/frontend-city-insights-page

export default Button;