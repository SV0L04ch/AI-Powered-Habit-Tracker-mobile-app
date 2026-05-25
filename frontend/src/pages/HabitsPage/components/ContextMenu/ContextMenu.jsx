import { useEffect, useRef } from 'react';
import styles from './ContextMenu.module.scss';

const ContextMenu = ({ items, onClose, position, dataTestId, ...rest }) => {
  const menuRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        onClose();
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [onClose]);

  if (!items || items.length === 0) return null;

  return (
    <div
      ref={menuRef}
      className={styles.menu}
      style={position ? { top: position.y, left: position.x } : undefined}
      data-testid={dataTestId || rest['data-testid']}
      role="menu"
    >
      {items.map((item) => (
        <button
          key={item.testId || item.label}
          className={styles.menuItem}
          onClick={item.onClick}
          data-testid={item.testId}
          type="button"
          role="menuitem"
        >
          {item.label}
        </button>
      ))}
    </div>
  );
};

export default ContextMenu;
