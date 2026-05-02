import styles from "./AddButton.module.scss";
import icons from "../../../../lib/icons";

const AddButton = ({ click, ...rest }) => {
  return (
    <button className={styles.addButton} onClick={click} {...rest}>
      <p className={styles.icon}>
        <icons.Plus />
      </p>
    </button>
  );
};

export default AddButton;
