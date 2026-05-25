import React from 'react';
import styles from './Button.module.scss';

const Button = ({
  children,
  variant = 'primary',
  disabled = false,
  type = 'button',
  onClick,
  loading = false,
  className = '',
  ...rest
}) => {
  const classes = [
    styles.btn,
    styles[variant] || '',
    loading ? styles.loading : '',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  return (
    <button
      type={type}
      className={classes}
      disabled={disabled || loading}
      onClick={onClick}
      aria-busy={loading}
      {...rest}
    >
      {loading && <span className={styles.spinner} aria-hidden="true" />}
      <span className={styles.content}>{children}</span>
    </button>
  );
};

export default Button;
