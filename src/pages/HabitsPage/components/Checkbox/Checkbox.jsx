import styles from "./Checkbox.module.scss";
import icons from "../../../../lib/icons";

<<<<<<< HEAD
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
=======
const Checkbox = ({ checked, onChange }) => {
  return (
    <label>
      <input type="checkbox" checked={checked} onChange={onChange} />
      {checked && <span> ✓</span>}
>>>>>>> feature/frontend-city-insights-page
    </label>
  );
};

<<<<<<< HEAD
export default Checkbox;
=======
export default Checkbox;
>>>>>>> feature/frontend-city-insights-page
