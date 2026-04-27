import React from 'react'
import styles from './CreateHabitPage.module.scss'
import PageLayout from '../../components/PageLayout/PageLayout'
import Typography from '../../components/Typography/Typography'
import Input from '../../components/Input/Input'
import Button from '../../components/Button/Button'
import icons from '../../lib/icons'

function CreateHabitPage() {


  const descClass = `${styles.basicText} ${styles.desc}`
  return (
    <PageLayout>
      <Typography variant='headline1' className={styles.mainText}>Создать привычку</Typography>

      <div className={styles.blocktext}>
        <Typography variant='headline2' className={styles.mainText}>Название привычки</Typography>
        <Input placeholder='Например: Медицтация'></Input>
      </div>

      <div className={styles.block}>

        <div className={styles.blocktext}>
          <Typography variant='headline2' className={styles.mainText}>Тип контроля</Typography>
          <div className={styles.buttons}>
            <Button variant='secondary'>
              <icons.Wristwatch />
              <Typography variant='body2' className={styles.basicText}>Время</Typography>
              <Typography variant='caption' className={descClass}>Напоминание</Typography>
            </Button>
            <Button variant='secondary'>
              <icons.Count className={styles.basicText}/>
              <Typography variant='body2' className={styles.basicText}>Повторы</Typography>
              <Typography variant='caption' className={descClass}>Счетчик</Typography>
            </Button>
          </div>
        </div>

        <div className={styles.blocktext}>
          <Typography variant='headline2' className={styles.mainText}>Сложность</Typography>
          <div className={styles.buttons}>
            <Button variant='secondary'>
              <div className={styles.stars}>
                <icons.FillStar />
                <icons.FillStar />
                <icons.FillStar />
              </div>
              <Typography variant='body2' className={styles.basicText}>Тяжело</Typography>
              <Typography variant='caption' className={descClass}>Штрафы<br/> за пропуск</Typography>
            </Button>
            <Button variant='secondary'>
              <div className={styles.stars}>
                <icons.EmptyStar />
                <icons.EmptyStar />
                <icons.EmptyStar />
              </div>
              <Typography variant='body2' className={styles.basicText}>Легко</Typography>
              <Typography variant='caption' className={descClass}>Нету штрафов<br/> за пропуск</Typography>
            </Button>
          </div>
        </div>

      </div>

    <div className={styles.block}>

      <div className={styles.blocktext}>
        <Typography variant='headline2' className={styles.mainText}>Время напоминания</Typography>
        <Input icon={<icons.Watch />} />
      </div>

      <div className={styles.blocktext}>
        <Typography variant='headline2' className={styles.mainText}>Теги</Typography>
        <Input icon={<icons.Count />} />
      </div>

    </div>


      <Button variant='primary'>Создать привычку</Button>
    </PageLayout>
  )
}

export default CreateHabitPage