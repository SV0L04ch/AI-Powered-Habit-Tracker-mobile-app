import { useEffect, useRef } from 'react';
import styles from './ContextMenu.module.scss';

const ContextMenu = ({ items, onClose, position }) => {
  const menuRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (menuRef.current && !menuRef.current.contains(e.target)) {
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
      style={{ top: position.y, left: position.x }}
    >
      {items.map((item, idx) => (
        <div key={idx} className={styles.menuItem} onClick={item.onClick}>
          {item.label}
        </div>
      ))}
    </div>
  );
};

export default ContextMenu;
