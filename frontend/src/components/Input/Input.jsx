import React, { useId } from 'react';
import styles from './Input.module.scss';

const Input = ({ icon, label, disabled = false, className, id, ...rest }) => {
  const generatedId = useId();
  const inputId = id || rest.name || generatedId;
  const hasValue = rest.value !== undefined && String(rest.value).length > 0;
  const containerClass = [
    styles.inputContainer,
    hasValue ? styles.hasValue : '',
    disabled ? styles.disabled : '',
    className || '',
  ]
    .filter(Boolean)
    .join(' ');

  return (
    <label className={containerClass} htmlFor={inputId}>
      {icon && <span className={styles.icon}>{icon}</span>}
      <span className={styles.inputWrap}>
        <input id={inputId} disabled={disabled} className={styles.field} {...rest} />
        {label && <span className={styles.label}>{label}</span>}
      </span>
    </label>
  );
};

export default Input;
