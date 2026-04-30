import { useNavigate } from "react-router-dom";
import Typography from "../../components/Typography/Typography";
import Substrate from "../../components/Substrate/Substrate";
import AddButton from "./components/AddButton/AddButton";
import PageLayout from "../../components/PageLayout/PageLayout";
import Checkbox from "./components/Checkbox/Checkbox";
import ContextMenu from "./components/ContextMenu/ContextMenu";
import styles from "./HabitsPage.module.scss"

function HabitsPage() {
  const navigate = useNavigate();

  const handleClick = () => {
    navigate("/habits/new");
  };
  return (
    <PageLayout>
      <Typography variant="headline1">Главная</Typography>
      <Typography variant="body1">
        Твой прогресс сегодня: 2/5 привычек
      </Typography>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Активные привычки</Typography>
        <Substrate variant="secondary">
            <div className={styles.checkDesc}>
            <Checkbox />
              <div className={styles.desc}>
                <Typography variant="Head3">Утренняя медитация</Typography>
                <div className={styles.captions}>
                  <Typography variant="caption">12 дней</Typography>
                  <Typography variant="caption">08:00</Typography>
                </div>
              </div>
            </div>
            <ContextMenu/>
        </Substrate>
      </div>
      <div className={styles.blockHabits}>
        <Typography variant="headline2">Завершены</Typography>
        <Substrate variant="secondary">
            <div className={styles.checkDesc}>
            <Checkbox checked={true} />
              <div className={styles.desc}>
                <Typography variant="Head3">Утренняя медитация</Typography>
                <div className={styles.captions}>
                  <Typography variant="caption">12 дней</Typography>
                  <Typography variant="caption">08:00</Typography>
                </div>
              </div>
            </div>
            <ContextMenu/>
        </Substrate>
      </div>
      <AddButton click={handleClick}></AddButton>
    </PageLayout>
  );
}

export default HabitsPage;
