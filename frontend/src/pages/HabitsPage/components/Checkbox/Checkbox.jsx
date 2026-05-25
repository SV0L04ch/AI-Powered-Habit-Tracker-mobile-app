import React from 'react';
import styles from './Checkbox.module.scss';
import icons from '../../../../lib/icons';

const Checkbox = ({ checked, onChange, disabled = false, loading = false, ...rest }) => {
  const testId = rest['data-testid'];

  return (
    <label
      className={`${styles.label} ${loading ? styles.loading : ''}`.trim()}
      data-testid={testId}
    >
      <input
        type="checkbox"
        className={styles.input}
        checked={checked}
        onChange={(event) => {
          if (!disabled && !loading && onChange) {
            onChange(event.target.checked);
          }
        }}
        disabled={disabled || loading}
        data-testid={testId ? `${testId}-native` : undefined}
      />
      <span className={styles.checkmark}>
        {loading ? <span className={styles.spinner} /> : checked && <icons.Check className={styles.icon} />}
      </span>
    </label>
  );
};

export default Checkbox;
