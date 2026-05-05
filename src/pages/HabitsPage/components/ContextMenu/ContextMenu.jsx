import styles from "./ContextMenu.module.scss";

const ContextMenu = ({...rest}) => {
  return (
    <div className={styles.background} {...rest}>
      <div className={styles.circle} />
      <div className={styles.circle} />
      <div className={styles.circle} />
    </div>
  );
};

export default ContextMenu;
