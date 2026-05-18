import React from 'react';
import styles from './Checkbox.module.scss';
import icons from '../../../../lib/icons';

const Checkbox = ({ checked, onChange, disabled = false, ...rest }) => {
  return (
    <label className={styles.label} {...rest}>
      <input
        type="checkbox"
        className={styles.input}
        checked={checked}
        onChange={(e) => {
          if (!disabled && onChange) {
            onChange(e.target.checked); // Передаём новое значение наружу
          }
        }}
        disabled={disabled}
      />
      <span className={styles.checkmark}>
        {checked && <icons.Check className={styles.icon} />}
      </span>
    </label>
  );
};

export default Checkbox;