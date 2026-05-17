import React from 'react';
import styles from './Input.module.scss';

const Input = ({ icon, disabled = false, ...rest }) => {
  const { className, ...inputProps } = rest;
  const containerClass = `${styles.inputContainer} ${className || ''}`.trim();
  return (
    <label className={containerClass}>
      {icon && <span className={styles.icon}>{icon}</span>}
      <input
        type="text"
        placeholder="Введите текст"
        disabled={disabled}
        className={styles.field}
        {...inputProps}
      />
    </label>
  );
};

export default Input;