import { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Typography from '../../components/Typography/Typography';
import AddButton from './components/AddButton/AddButton';
import PageLayout from '../../components/PageLayout/PageLayout';
import Checkbox from './components/Checkbox/Checkbox';
import ContextMenu from './components/ContextMenu/ContextMenu';
import EditHabitModal from '../../components/EditHabitModal/EditHabitModal';
import Modal from '../../components/Modal/Modal';
import Button from '../../components/Button/Button';
import useAuthUser from '../../store/useAuthStore';
import useHabits from '../../store/useHabitsStore';
import useInsight from '../../store/useInsightStore';
import useDailySummaryStore from '../../store/useDailySummaryStore';
import images from '../../lib/images';
import styles from './HabitsPage.module.scss';

const ROW_HEIGHT = 132;
const OVERSCAN = 5;

const getWeatherImage = (condition) => {
  const normalized = String(condition || '').toLowerCase();
  if (normalized.includes('rain')) return images.Rain;
  if (normalized.includes('cloud')) return images.Clouds;
  return images.Sun;
};

const getTriggerLabel = (habit) => {
  if (Number(habit.triggerType) === 1) return `В ${habit.triggerValue}`;
  return `${habit.triggerValue} раз в день`;
};

const getProgress = (habit, entry) => {
  if (entry?.status === 1) return 100;
  if (entry?.status === 2 && Number(habit.triggerType) === 2) {
    const target = Number(habit.triggerValue) || 1;
    return Math.min(99, Math.round(((entry.partialValue || 0) / target) * 100));
  }
  return 0;
};

function HabitsPage() {
  const [editingHabit, setEditingHabit] = useState(null);
  const [menuData, setMenuData] = useState(null);
  const [showInsightModal, setShowInsightModal] = useState(false);
  const [scrollTop, setScrollTop] = useState(0);
  const listRef = useRef(null);

  const {
    message: insightMessage,
    isLoading: isInsightLoading,
    error: insightError,
    fetchSupport,
    clearInsight,
  } = useInsight();
  const navigate = useNavigate();
  const habits = useHabits((state) => state.habits);
  const entriesByHabitId = useHabits((state) => state.entriesByHabitId);
  const isHabitsLoading = useHabits((state) => state.isLoading);
  const actionLoadingId = useHabits((state) => state.actionLoadingId);
  const habitsError = useHabits((state) => state.error);
  const getHabits = useHabits((state) => state.getHabits);
  const deleteHabit = useHabits((state) => state.deleteHabit);
  const markHabitCompleted = useHabits((state) => state.markHabitCompleted);
  const clearError = useHabits((state) => state.clearError);
  const isAuthenticated = useAuthUser((state) => state.isAuthenticated);
  const city = useAuthUser((state) => state.city);
  const loadProfile = useAuthUser((state) => state.loadProfile);
  const { summary, isLoading: summaryLoading, fetchStats } = useDailySummaryStore();

  useEffect(() => {
    clearError();
    if (isAuthenticated) {
      loadProfile();
      getHabits({ force: true });
      fetchStats();
    }
  }, [clearError, fetchStats, getHabits, isAuthenticated, loadProfile]);

  useEffect(() => {
    if (insightMessage || insightError || isInsightLoading) {
      setShowInsightModal(true);
    }
  }, [insightError, insightMessage, isInsightLoading]);

  const completedCount = useMemo(
    () => habits.filter((habit) => entriesByHabitId[habit.id]?.status === 1).length,
    [entriesByHabitId, habits],
  );
  const completionRate = habits.length ? Math.round((completedCount / habits.length) * 100) : 0;

  const visibleRange = useMemo(() => {
    const viewport = listRef.current?.clientHeight || 520;
    const start = Math.max(0, Math.floor(scrollTop / ROW_HEIGHT) - OVERSCAN);
    const visibleCount = Math.ceil(viewport / ROW_HEIGHT) + OVERSCAN * 2;
    return { start, end: Math.min(habits.length, start + visibleCount) };
  }, [habits.length, scrollTop]);

  const visibleHabits = habits.slice(visibleRange.start, visibleRange.end);
  const topSpacer = visibleRange.start * ROW_HEIGHT;
  const bottomSpacer = Math.max(0, (habits.length - visibleRange.end) * ROW_HEIGHT);

  const handleCloseInsight = () => {
    setShowInsightModal(false);
    clearInsight();
  };

  const handleMenuClick = (event, habitId) => {
    event.stopPropagation();
    setMenuData(menuData?.habitId === habitId ? null : { habitId });
  };

  const closeMenu = () => setMenuData(null);

  const handleDelete = async (id) => {
    const confirmed = window.confirm('Удалить привычку?');
    if (confirmed) {
      await deleteHabit(id);
      await fetchStats();
      closeMenu();
    }
  };

  const handleCheck = async (habit) => {
    await markHabitCompleted(habit);
    await fetchStats();
  };

  const renderHabitCard = (habit, index) => {
    const entry = entriesByHabitId[habit.id];
    const isCompleted = entry?.status === 1;
    const progress = getProgress(habit, entry);
    const testPrefix = isCompleted ? 'completed' : 'active';

    return (
      <article
        key={habit.id}
        className={styles.habitCard}
        style={{ animationDelay: `${Math.min(index, 8) * 45}ms` }}
        data-testid={`${testPrefix}-habit-${habit.id}`}
      >
        <div className={styles.habitMain}>
          <Checkbox
            checked={isCompleted}
            onChange={() => handleCheck(habit)}
            loading={actionLoadingId === habit.id}
            data-testid={`${testPrefix}-habit-${habit.id}-checkbox`}
          />
          <div className={styles.habitContent}>
            <div className={styles.habitTopline}>
              <Typography variant="headline3" data-testid={`${testPrefix}-habit-${habit.id}-name`}>
                {habit.name}
              </Typography>
              <button
                className={styles.menuButton}
                onClick={(event) => handleMenuClick(event, habit.id)}
                data-testid={`options-menu-btn-${habit.id}`}
                aria-label="Действия привычки"
              >
                ⋯
              </button>
            </div>
            <div className={styles.habitMeta}>
              <span data-testid={`${testPrefix}-habit-${habit.id}-trigger`}>{getTriggerLabel(habit)}</span>
              <span data-testid={`${testPrefix}-habit-${habit.id}-type`}>
                {habit.isPositive ? 'полезная' : 'контроль срывов'}
              </span>
            </div>
            <div className={styles.progressTrack} data-testid={`${testPrefix}-habit-${habit.id}-progress`}>
              <span style={{ width: `${progress}%` }} />
            </div>
          </div>
        </div>

        {menuData?.habitId === habit.id && (
          <ContextMenu
            items={[
              {
                label: 'Редактировать',
                onClick: () => {
                  setEditingHabit(habit);
                  closeMenu();
                },
                testId: 'edit-habit-btn',
              },
              {
                label: 'Удалить',
                onClick: () => handleDelete(habit.id),
                testId: 'delete-habit-btn',
              },
              {
                label: 'AI-совет',
                onClick: () => {
                  fetchSupport(habit.id, habit.isPositive ? (isCompleted ? 'skip' : 'lazy') : 'relapse');
                  closeMenu();
                },
                testId: 'daily-tip-btn',
              },
            ]}
            onClose={closeMenu}
            data-testid="context-menu"
          />
        )}
      </article>
    );
  };

  return (
    <PageLayout data-testid="habits-page">
      <header className={styles.header} data-testid="dashboard-header">
        <div>
          <Typography variant="headline1" data-testid="dashboard-title">
            Сегодня
          </Typography>
          <Typography variant="body1" className={styles.muted} data-testid="dashboard-city">
            {city ? `Город: ${city}` : 'Город можно указать в профиле'}
          </Typography>
        </div>
        <div className={styles.rateBadge} data-testid="completion-rate">
          {completionRate}%
        </div>
      </header>

      <section className={styles.summaryGrid} data-testid="summary-grid">
        <div className={styles.summaryCard} data-testid="progress-summary-card">
          <span className={styles.summaryLabel}>Выполнено</span>
          <strong>{completedCount}/{habits.length}</strong>
          <div className={styles.progressTrack}>
            <span style={{ width: `${completionRate}%` }} />
          </div>
        </div>
        <div className={styles.summaryCard} data-testid="weather-summary-card">
          {summaryLoading ? (
            <div className={styles.skeletonBlock} data-testid="daily-summary-loader" />
          ) : (
            <>
              <img src={getWeatherImage(summary?.weather?.condition)} alt="" />
              <span className={styles.summaryLabel}>{summary?.weather?.condition || 'Погода'}</span>
              <strong>
                {summary?.weather?.temperatureCelsius != null
                  ? `${summary.weather.temperatureCelsius}°C`
                  : 'нет данных'}
              </strong>
            </>
          )}
        </div>
      </section>

      {habitsError && (
        <p className={styles.error} data-testid="server-error">
          {habitsError}
        </p>
      )}

      <section className={styles.listSection} data-testid="active-habits-section">
        <div className={styles.sectionHeader}>
          <Typography variant="headline2">Привычки</Typography>
          <span data-testid="habits-count">{habits.length}</span>
        </div>

        {isHabitsLoading && (
          <div className={styles.skeletonList} data-testid="data-loading">
            {Array.from({ length: 4 }).map((_, index) => (
              <div className={styles.skeletonCard} key={index} />
            ))}
          </div>
        )}

        {!isHabitsLoading && habits.length === 0 && (
          <div className={styles.emptyState} data-testid="empty-habits-state">
            <Typography variant="headline3">Привычек пока нет</Typography>
            <Typography variant="body2" className={styles.muted}>
              Создайте первую привычку, и дашборд начнет собирать прогресс.
            </Typography>
            <Button variant="secondary" onClick={() => navigate('/habits/new')} data-testid="empty-add-button">
              Добавить привычку
            </Button>
          </div>
        )}

        {!isHabitsLoading && habits.length > 0 && (
          <div
            ref={listRef}
            className={styles.virtualList}
            onScroll={(event) => setScrollTop(event.currentTarget.scrollTop)}
            data-testid="habits-virtual-list"
          >
            <div style={{ height: topSpacer }} />
            {visibleHabits.map((habit, index) => renderHabitCard(habit, visibleRange.start + index))}
            <div style={{ height: bottomSpacer }} />
          </div>
        )}
      </section>

      <AddButton click={() => navigate('/habits/new')} data-testid="add-button" />

      <EditHabitModal isOpen={!!editingHabit} onClose={() => setEditingHabit(null)} habit={editingHabit} />

      <Modal isOpen={showInsightModal} onClose={handleCloseInsight} data-testid="insight-modal">
        <div className={styles.insightContainer}>
          <Typography variant="headline3" data-testid="insight-title">
            AI-совет
          </Typography>
          {isInsightLoading && <div className={styles.skeletonBlock} data-testid="insight-loader" />}
          {insightError && (
            <p className={styles.error} data-testid="insight-error">
              {insightError}
            </p>
          )}
          {insightMessage && (
            <Typography variant="body1" data-testid="insight-message">
              {insightMessage}
            </Typography>
          )}
          <Button variant="primary" onClick={handleCloseInsight} data-testid="insight-close-button">
            Понятно
          </Button>
        </div>
      </Modal>
    </PageLayout>
  );
}

export default HabitsPage;
