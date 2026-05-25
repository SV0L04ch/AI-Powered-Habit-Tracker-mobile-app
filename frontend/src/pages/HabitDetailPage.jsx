import { useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '../components/Button/Button';
import PageLayout from '../components/PageLayout/PageLayout';
import Typography from '../components/Typography/Typography';
import { fetchHabitEntries } from '../services/habitService';
import useHabits from '../store/useHabitsStore';
import { getErrorMessage } from '../utils/handleServerError';
import styles from './HabitDetailPage.module.scss';

const getStatusLabel = (entry) => {
  if (entry.status === 1) return 'Выполнено';
  if (entry.status === 2) return 'Частично';
  if (entry.status === 3) return 'Пропущено';
  if (entry.relapseCount) return `${entry.relapseCount} срыв`;
  return 'Без отметки';
};

function HabitDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const habits = useHabits((state) => state.habits);
  const getHabits = useHabits((state) => state.getHabits);
  const [entries, setEntries] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    getHabits();
  }, [getHabits]);

  useEffect(() => {
    const loadEntries = async () => {
      setIsLoading(true);
      setError('');
      try {
        const toDate = new Date().toISOString().slice(0, 10);
        const from = new Date();
        from.setDate(from.getDate() - 30);
        const fromDate = from.toISOString().slice(0, 10);
        const data = await fetchHabitEntries(id, { fromDate, toDate });
        setEntries(data);
      } catch (err) {
        setError(getErrorMessage(err));
      } finally {
        setIsLoading(false);
      }
    };

    if (id) loadEntries();
  }, [id]);

  const habit = useMemo(() => habits.find((item) => item.id === id), [habits, id]);
  const completedCount = entries.filter((entry) => entry.status === 1).length;
  const percent = entries.length ? Math.round((completedCount / entries.length) * 100) : 0;

  return (
    <PageLayout data-testid="habit-detail-page">
      <header className={styles.header} data-testid="habit-detail-header">
        <Button variant="ghost" onClick={() => navigate('/habits')} data-testid="habit-detail-back-button">
          Назад
        </Button>
        <Typography variant="headline1" data-testid="habit-detail-title">
          {habit?.name || 'Привычка'}
        </Typography>
        <Typography variant="body1" className={styles.muted} data-testid="habit-detail-subtitle">
          История отметок за последние 30 дней.
        </Typography>
      </header>

      <section className={styles.summary} data-testid="habit-detail-summary">
        <article data-testid="habit-detail-percent">
          <span>Выполнено</span>
          <strong>{percent}%</strong>
        </article>
        <article data-testid="habit-detail-entries-count">
          <span>Отметок</span>
          <strong>{entries.length}</strong>
        </article>
      </section>

      {isLoading && (
        <div className={styles.skeletonList} data-testid="habit-detail-loader">
          <div />
          <div />
          <div />
        </div>
      )}

      {error && (
        <p className={styles.error} data-testid="habit-detail-error">
          {error}
        </p>
      )}

      {!isLoading && !error && (
        <section className={styles.entries} data-testid="habit-detail-entries">
          {entries.length ? (
            entries.map((entry) => (
              <article key={entry.id} className={styles.entryCard} data-testid={`habit-entry-${entry.id}`}>
                <div>
                  <Typography variant="headline3" data-testid={`habit-entry-${entry.id}-date`}>
                    {entry.date}
                  </Typography>
                  {entry.note && (
                    <Typography variant="body2" className={styles.muted} data-testid={`habit-entry-${entry.id}-note`}>
                      {entry.note}
                    </Typography>
                  )}
                </div>
                <span data-testid={`habit-entry-${entry.id}-status`}>{getStatusLabel(entry)}</span>
              </article>
            ))
          ) : (
            <article className={styles.empty} data-testid="habit-detail-empty-state">
              <Typography variant="headline3">Отметок пока нет</Typography>
              <Typography variant="body2" className={styles.muted}>
                Отметьте привычку на главной, и история появится здесь.
              </Typography>
            </article>
          )}
        </section>
      )}
    </PageLayout>
  );
}

export default HabitDetailPage;
