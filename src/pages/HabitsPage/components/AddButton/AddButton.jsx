import styles from "./AddButton.module.scss";
import icons from "../../../../lib/icons";

const AddButton = ({ click, ...rest }) => {
  return (
    <button className={styles.addButton} onClick={click} {...rest}>
      <span style={{ fontSize: 30, color: 'white' }}>+</span>
    </button>
  );
};

export default AddButton;