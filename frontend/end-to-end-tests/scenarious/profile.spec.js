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
  test('Logout user', async ({ page }) => {
    const profileButton = page.getByTestId('nav-button-profile'); 
    await profileButton.click();

    // Нажимаем кнопку "Выход"
    const logoutButton = page.getByText('Выход');
    await logoutButton.waitFor({ state: 'visible' });
    await logoutButton.click();

    // Проверяем, что произошёл редирект на страницу логина
    await expect(page).toHaveURL(/\/login/);
  });
});