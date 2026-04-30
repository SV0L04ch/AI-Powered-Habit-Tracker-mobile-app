import styles from "./Checkbox.module.scss";
import icons from "../../../../lib/icons";

const Checkbox = ({ checked, onChange, label }) => {
  return (
    <label className={styles.label}>
      <input
        type="checkbox"
        className={styles.input}
        checked={checked}
        onChange={onChange}
      />
      <span className={styles.checkmark}>
        <icons.Check className={styles.icon} />
      </span>
      {label && <span>{label}</span>}
    </label>
  );
};

export default Checkbox;
