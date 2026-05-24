import { test, expect } from '@playwright/test';
import { registerUser, loginUser, confirmEmail } from '../helpers.js';
import { getTestUser } from '../config.js';

test.beforeEach(async ({ page }) => {
  const user = getTestUser();
  await page.goto('/register');
  await registerUser(page, user.email, user.password, user.password, user.city);
  await confirmEmail(page, user.email);
  await page.goto('/login');
  await loginUser(page, user.email, user.password);
  await expect(page).toHaveURL(/\/habits/);
});

test.describe('Positive scenarios', () => {
  test('Add new correct habit', async ({ page }) => {
    await page.goto('/habits');

    const addButton = page.getByTestId('add-button');
    await addButton.waitFor({ state: 'visible' });
    await addButton.click();

    await expect(page).toHaveURL(/\/habits\/new/);
    const habitName = `meditation_${Date.now()}`;
    await page.fill('input[name="name"]', habitName);
    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();
    await page.fill('input[name="trigger_value"]', '08:00');

    // Перехватываем ответ на создание привычки
    const responsePromise = page.waitForResponse(
      res => res.url().includes('/api/habits') && res.status() === 201,
      { timeout: 10000 }
    );
    const createButton = page.getByTestId('submit-button');
    await createButton.click();
    const response = await responsePromise;
    expect(response.status(), 'Habit creation failed').toBe(201);

    await expect(page).toHaveURL(/\/habits/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator(`text=${habitName}`)).toBeVisible({ timeout: 15000 });
  });
});

test.describe('Negative scenarios', () => {
  test('Create habit without title', async ({ page }) => {
    await page.goto('/habits');

    const addButton = page.getByTestId('add-button');
    await addButton.click();

    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();
    await page.fill('input[name="trigger_value"]', '08:00');

    // Ожидаем ошибку валидации (нет ответа 201)
    const submitButton = page.getByTestId('submit-button');
    await submitButton.click();

    const nonTitleErrorMsg = page.getByTestId('validation-error');
    await expect(nonTitleErrorMsg).toBeVisible();
    await expect(nonTitleErrorMsg).toHaveText(/Введите название привычки/i);
  });

  test('Create habit with time control type and without time', async ({ page }) => {
    await page.goto('/habits');

    const addButton = page.getByTestId('add-button');
    await addButton.click();

    await page.fill('input[name="name"]', 'meditation');
    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();

    const submitButton = page.getByTestId('submit-button');
    await submitButton.click();

    const errorMsg = page.getByTestId('validation-error');
    await expect(errorMsg).toBeVisible();
    await expect(errorMsg).toHaveText(/Введите значение \(время или количество\)/i);
  });

  test('Create habit with repeat control type and without repeat value', async ({ page }) => {
    await page.goto('/habits');

    const addButton = page.getByTestId('add-button');
    await addButton.click();

    await page.fill('input[name="name"]', 'meditation');
    const counterButton = page.getByTestId('controlType-counter-button');
    await counterButton.click();
    
    const submitButton = page.getByTestId('submit-button');
    await submitButton.click();

    const errorMsg = page.getByTestId('validation-error');
    await expect(errorMsg).toBeVisible();
    await expect(errorMsg).toHaveText(/Введите значение \(время или количество\)/i);
  });
});

test.describe('Edit habit', () => {
  test('Edit habit name', async ({ page }) => {
    await page.goto('/habits');

    const originalName = `to_edit_${Date.now()}`;
    const addButton = page.getByTestId('add-button');
    await addButton.click();
    await page.fill('input[name="name"]', originalName);
    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();
    await page.fill('input[name="trigger_value"]', '08:00');
    const responsePromise = page.waitForResponse(res => res.url().includes('/api/habits') && res.status() === 201);
    const createButton = page.getByTestId('submit-button');
    await createButton.click();
    const createResp = await responsePromise;
    expect(createResp.status(), 'Failed to create habit for edit').toBe(201);
    await expect(page).toHaveURL(/\/habits/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator(`text=${originalName}`)).toBeVisible({ timeout: 15000 });

    const newName = `${originalName}_updated`;
    const habitCard = page.locator(`div:has-text("${originalName}"):has([data-testid^="options-menu-btn-"])`).first();
    await expect(habitCard).toBeVisible();

    await habitCard.locator('[data-testid^="options-menu-btn-"]').click();
    await page.getByTestId('edit-habit-btn').click();

    await page.getByTestId('habit-name').fill(newName);
    await page.getByTestId('save-button').click();

    await expect(page).toHaveURL(/\/habits/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator(`text=${newName}`)).toBeVisible({ timeout: 15000 });
  });
});

/*
test.describe('Habit actions', () => {
  test('Toggle habit completion (active -> inactive)', async ({ page }) => {
    await page.goto('/habits');

    const habitName = `toggle_${Date.now()}`;
    const addButton = page.getByTestId('add-button');
    await addButton.click();
    await page.fill('input[name="name"]', habitName);
    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();
    await page.fill('input[name="trigger_value"]', '08:00');
    const responsePromise = page.waitForResponse(res => res.url().includes('/api/habits') && res.status() === 201);
    const createButton = page.getByTestId('submit-button');
    await createButton.click();
    const createResp = await responsePromise;
    expect(createResp.status(), 'Failed to create habit for toggle').toBe(201);
    await expect(page).toHaveURL(/\/habits/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator(`text=${habitName}`)).toBeVisible({ timeout: 15000 });

    const habitCard = page.locator(`div[data-testid^="active-habit-"]:has-text("${habitName}"):has([data-testid^="options-menu-btn-"])`).first();
    await expect(habitCard).toBeVisible();

    const checkbox = habitCard.getByRole('checkbox');
    await checkbox.check();

    await expect(habitCard).not.toBeVisible();
    const inactiveCard = page.locator(`div[data-testid^="inactive-habit-"]:has-text("${habitName}")`);
    await expect(inactiveCard).toBeVisible({ timeout: 5000 });
  });
});

test.describe('AI insight', () => {
  test('Get daily tip from context menu', async ({ page }) => {
    await page.goto('/habits');

    const habitName = `tip_${Date.now()}`;
    const addButton = page.getByTestId('add-button');
    await addButton.click();
    await page.fill('input[name="name"]', habitName);
    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();
    await page.fill('input[name="trigger_value"]', '08:00');
    const responsePromise = page.waitForResponse(res => res.url().includes('/api/habits') && res.status() === 201);
    const createButton = page.getByTestId('submit-button');
    await createButton.click();
    const createResp = await responsePromise;
    expect(createResp.status(), 'Failed to create habit for AI tip').toBe(201);
    await expect(page).toHaveURL(/\/habits/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator(`text=${habitName}`)).toBeVisible({ timeout: 15000 });

    const habitCard = page.locator(`div:has-text("${habitName}"):has([data-testid^="options-menu-btn-"])`).first();
    await expect(habitCard).toBeVisible();

    await habitCard.locator('[data-testid^="options-menu-btn-"]').click();
    const dailyTipButton = page.getByTestId('daily-tip-btn');
    await expect(dailyTipButton).toBeVisible({ timeout: 3000 });
    await dailyTipButton.click();

    await expect(page.getByTestId('insight-modal')).toBeVisible();
    await expect(page.getByTestId('insight-message')).toBeVisible();
    await page.getByTestId('insight-close-button').click();
    await expect(page.getByTestId('insight-modal')).not.toBeVisible();
  });
});
*/

test.describe('Delete habit', () => {
  test('Delete habit via context menu', async ({ page }) => {
    await page.goto('/habits');

    const habitName = `delete_${Date.now()}`;
    const addButton = page.getByTestId('add-button');
    await addButton.click();
    await page.fill('input[name="name"]', habitName);
    const timeButton = page.getByTestId('controlType-time-button');
    await timeButton.click();
    await page.fill('input[name="trigger_value"]', '08:00');
    const responsePromise = page.waitForResponse(res => res.url().includes('/api/habits') && res.status() === 201);
    const createButton = page.getByTestId('submit-button');
    await createButton.click();
    const createResp = await responsePromise;
    expect(createResp.status(), 'Failed to create habit for delete').toBe(201);
    await expect(page).toHaveURL(/\/habits/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator(`text=${habitName}`)).toBeVisible({ timeout: 15000 });

    const habitCard = page.locator(`div:has-text("${habitName}"):has([data-testid^="options-menu-btn-"])`).first();
    await expect(habitCard).toBeVisible();

    await habitCard.locator('[data-testid^="options-menu-btn-"]').click();
    const deleteButton = page.getByTestId('delete-habit-btn');
    await expect(deleteButton).toBeVisible({ timeout: 3000 });

    page.on('dialog', async dialog => {
      await dialog.accept();
    });

    await deleteButton.click();
    await expect(page.locator(`text=${habitName}`)).not.toBeVisible({ timeout: 5000 });
  });
});