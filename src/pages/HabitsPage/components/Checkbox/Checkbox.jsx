import styles from "./Checkbox.module.scss";
import icons from "../../../../lib/icons";

const Checkbox = ({ checked, onChange }) => {
  return (
    <label>
      <input type="checkbox" checked={checked} onChange={onChange} />
      {checked && <span> ✓</span>}
    </label>
  );
};

export default Checkbox;