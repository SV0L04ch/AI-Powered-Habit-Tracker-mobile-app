import styles from "./AddButton.module.scss";
import icons from "../../../../lib/icons";

const AddButton = ({ click, ...rest }) => {
  return (
    <button className={styles.addButton} onClick={click} {...rest}>
<<<<<<< HEAD
      <p className={styles.icon}>
        <icons.Plus />
      </p>
=======
      <span style={{ fontSize: 30, color: 'white' }}>+</span>
>>>>>>> feature/frontend-city-insights-page
    </button>
  );
};

<<<<<<< HEAD
export default AddButton;
=======
export default AddButton;
>>>>>>> feature/frontend-city-insights-page
