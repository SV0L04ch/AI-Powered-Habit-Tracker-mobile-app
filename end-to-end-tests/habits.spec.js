import { test, expect } from '@playwright/test';

test('Add new correct habit', async ({ page }) => {
  // 1. Логин
  await page.goto('/login');
  await page.fill('input[name="email"]', 'tester@example.com');
  await page.fill('input[name="password"]', 'password1');
  await page.click('button:has-text("Войти")');
  await expect(page).toHaveURL(/\/habits/);

  // 2. Нажать кнопку добавления привычки
  const addButton = page.locator('._addButton_13rtf_7');
  await addButton.waitFor({ state: 'visible' });
  await addButton.click();

  // 3. Ждём, когда откроется форма создания привычки
  await expect(page).toHaveURL(/\/habits\/new/);

  // 4. Заполняем форму
  await page.fill('input[name="name"]', 'meditation');

  // 5. Выбираем время (если есть такой элемент)
  const timeButton = page.locator('button:has-text("Время")');
  await timeButton.click();
  await page.fill('input[name="trigger_value"]', '08:00');

  // 6. Нажимаем кнопку сохранения
  const createButton = page.locator('button:has-text("Создать")');
  await createButton.click();

  // 7. Проверяем, что вернулись на страницу привычек
  await expect(page).toHaveURL(/\/habits/);

  // 8. Убеждаемся, что новая привычка отображается
  await expect(page.locator('text=meditation')).toBeVisible();
});