import React from 'react';
import styles from './Modal.module.scss';

const Modal = ({ isOpen, onClose, children, className = '', ...rest }) => {
  if (!isOpen) return null;

  return (
    <div className={styles.overlay} onClick={onClose} data-testid="modal-overlay">
      <div
        className={`${styles.content} ${className}`.trim()}
        onClick={(event) => event.stopPropagation()}
        role="dialog"
        aria-modal="true"
        {...rest}
      >
        {children}
      </div>
    </div>
  );
};

export default Modal;
