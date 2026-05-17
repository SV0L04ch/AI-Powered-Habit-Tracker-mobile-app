import React from 'react';
import styles from './Button.module.scss';

const Button = ({ children, variant, disabled = false, onClick, className, ...rest }) => {
  const classes = `${styles.btn} ${styles[variant] || ''} ${className || ''}`.trim();
  return (
    <button className={classes} disabled={disabled} onClick={onClick} {...rest}>
      {children}
    </button>
  );
};

export default Button;