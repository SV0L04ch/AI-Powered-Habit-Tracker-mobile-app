import { test, expect } from '@playwright/test';

async function loginUser(page, email, password) {
    await page.fill('input[name="email"]', 'tester@example.com');
    await page.fill('input[name="password"]', 'password1');
    await page.click('button:has-text("Войти")');
}

test.describe('Positive scenarious', () => {
  test('Add new correct habit', async ({ page }) => {
    const email = 'tester@example.com';
    const password = 'password1';
    
    // 1. Логин
    await page.goto('/login');
    loginUser(page, email, password);
    await expect(page).toHaveURL(/\/habits/);

    // 2. Нажать кнопку добавления привычки
    const addButton = page.getByTestId('add-button');
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
    const createButton = page.getByTestId('submit-button');
    await createButton.click();

    // 7. Проверяем, что вернулись на страницу привычек
    await expect(page).toHaveURL(/\/habits/);

    // 8. Убеждаемся, что новая привычка отображается
    await expect(page.locator('text=meditation')).toBeVisible();
  });

  test('Update habit with correct data', async ({ page }) => {
    const email = 'tester@example.com';
    const password = 'password1';
    
    // 1. Логин
    await page.goto('/login');
    loginUser(page, email, password);
    await expect(page).toHaveURL(/\/habits/);

    await page.getByTestId('active-habit-88114b52-670c-4b84-a36f-3015116a1d79-edit-button').click();

    await page.getByTestId('habit-name').fill('test-habit');
    await page.getByTestId('controlType-counter-button').click();
    await page.getByTestId('categoryType-hard-button').click();
    await page.getByTestId('trigger-type').fill(5);

    await page.getByTestId('save-button').click();

    await expect(page).toHaveURL(/\/habits/);

    await expect(page.locator('text=test-habit')).toBeVisible();
  });
});