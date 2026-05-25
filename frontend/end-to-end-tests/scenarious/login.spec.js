import { test, expect } from '@playwright/test';
import { registerUser, loginUser } from '../helpers.js';
import { getTestUser } from '../test-data.js';

async function clearState(page) {
  await page.context().clearCookies();
  await page.goto('/login');            // переходим на страницу, где localStorage доступен
  await page.evaluate(() => {
    if (window.localStorage) localStorage.clear();
    if (window.sessionStorage) sessionStorage.clear();
  });
}

async function confirmEmail(page, email) {
  let confirmLink = null;
  for (let i = 0; i < 20; i++) {
    await page.waitForTimeout(1000);
    const response = await page.request.get('http://localhost:8025/api/v2/messages');
    const data = await response.json();
    const message = data.items.find(msg => msg.Content.Headers.To[0]?.includes(email));
    if (message) {
      const html = Buffer.from(message.Content.Body, 'base64').toString('utf-8');
      const match = html.match(/href='(http:\/\/localhost:5093\/api\/auth\/confirm-email\?[^']+)'/);
      if (match) confirmLink = match[1];
      break;
    }
  }
  expect(confirmLink, `Confirmation email for ${email} not found`).toBeDefined();
  await page.goto(confirmLink);
  await expect(page.locator('text=Email confirmed')).toBeVisible();
}

test.describe('Login', () => {
  test.describe('Positive scenarios', () => {
    test('Successful login with confirmed email redirects to /habits', async ({ page }) => {
      const user = getTestUser();
      await page.goto('/register');
      await registerUser(page, user.email, user.password, user.password, user.city);
      await expect(page).toHaveURL(/\/login/);
      await confirmEmail(page, user.email);
      await page.goto('/login');
      await loginUser(page, user.email, user.password);
      await expect(page).toHaveURL(/\/habits/);
    });
  });

  test.describe('Negative scenarios', () => {
    test.beforeEach(async ({ page }) => {
      await clearState(page);
    });

    test('login with non-confirmed email shows error and stays on /login', async ({ page }) => {
      const user = getTestUser();
      await page.goto('/register');
      await registerUser(page, user.email, user.password, user.password, user.city);
      await expect(page).toHaveURL(/\/login/);
      // Не подтверждаем email
      await loginUser(page, user.email, user.password);
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByTestId('server-error');
      await expect(errorMsg).toBeVisible();
      await expect(errorMsg).toHaveText(/подтвердите email|confirm your email|401|Unauthorized/i);
    });

    test('Wrong password shows error', async ({ page }) => {
      const user = getTestUser();
      await page.goto('/register');
      await registerUser(page, user.email, user.password, user.password, user.city);
      await expect(page).toHaveURL(/\/login/);
      await confirmEmail(page, user.email); // подтверждаем, чтобы проверить именно неверный пароль
      await page.goto('/login');
      await loginUser(page, user.email, 'wrongpassword');
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByText(/неверный email или пароль|Request failed with status code 401/i);
      await expect(errorMsg).toBeVisible();
    });

    test('Non-existent email shows error', async ({ page }) => {
      await loginUser(page, 'nonexistent@example.com', 'password1');
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByText(/неверный email или пароль|invalid credentials|Request failed with status code 401/i);
      await expect(errorMsg).toBeVisible();
    });

    test('Empty fields trigger validation', async ({ page }) => {
      await page.click('button:has-text("Войти")');
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByText(/Заполните все поля/i);
      await expect(errorMsg).toBeVisible();
    });

    test('Login with very long email (validation)', async ({ page }) => {
      const longEmail = 'a'.repeat(255) + '@example.com';
      await loginUser(page, longEmail, 'password1');
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByTestId('server-error');
      await expect(errorMsg).toHaveText(/Email is too long \(max 256 characters\)\.|Эл. почта не может быть длиннее 256 символов|400/i);
    });
  });

  test.describe('Authorization protection', () => {
    test('Redirect to /login when accessing /habits without login', async ({ page }) => {
      await clearState(page);
      await page.goto('/habits');
      await expect(page).toHaveURL(/\/login|\/register/);
    });

    test('Access to /habits after successful login', async ({ page }) => {
      const user = getTestUser();
      await page.goto('/register');
      await registerUser(page, user.email, user.password, user.password, user.city);
      await expect(page).toHaveURL(/\/login/);
      await confirmEmail(page, user.email);
      await page.goto('/login');
      await loginUser(page, user.email, user.password);
      await expect(page).toHaveURL(/\/habits/);
      await page.goto('/habits');
      await expect(page).toHaveURL(/\/habits/);
    });
  });
});