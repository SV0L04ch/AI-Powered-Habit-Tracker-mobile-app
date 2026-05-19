import { test, expect } from '@playwright/test';

// Вспомогательная функция регистрации
async function registerUser(page, email, password = 'password1', city = 'Москва') {
  await page.goto('/register');
  await page.fill('input[name="email"]', email);
  await page.fill('input[name="password"]', password);
  await page.fill('input[name="confirm"]', password);
  await page.selectOption('select[name="city"]', city);
  await page.getByTestId('reg-button').click();
  // Ожидание редиректа (предполагаем, что после регистрации попадаем на /habits/new или /habits)
  await expect(page).toHaveURL(/\/login/, { timeout: 10000 });
}

async function loginUser(page, email, password) {
  await page.goto('/login');
  await page.fill('input[name="email"]', email);
  await page.fill('input[name="password"]', password);
  await page.getByTestId('login-button').click();
}

test.describe('Login', () => {
  test.describe('Positive scenarios', () => {
    test('Successful login with confirmed email redirects to /habits', async ({ page }) => {
      const email = 'tester@example.com';
      const password = 'password1';
      
      loginUser(page, email, password);
      await expect(page).toHaveURL(/\/habits/);
    });
  });

  test.describe('Negative scenarios', () => {
    test('login with non-confirmed email shows error and stays on /login', async ({ page }) => {
      const email = 'toster@example.com';
      await registerUser(page, email);// Регистрируем, но не подтверждаем (если подтверждение включено, то после регистрации редирект на логин)

      await expect(page).toHaveURL(/\/login/);

      // Пытаемся войти
      await page.fill('input[name="email"]', email);
      await page.fill('input[name="password"]', 'password1');
      await page.getByTestId('login-button').click();
      await expect(page).toHaveURL(/\/login/);
      
      const nonConfirmedErrorMsg = page.getByTestId('server-error');
      await expect(nonConfirmedErrorMsg).toBeVisible();
      await expect(nonConfirmedErrorMsg).toHaveText(/подтвердите email|confirm your email|401|Unauthorized/i);
    });

    test('Wrong password shows error', async ({ page }) => {
      const email = 'tester@example.com';
      await page.goto('/login');
      await page.fill('input[name="email"]', email);
      await page.fill('input[name="password"]', 'wrongpassword');
      await page.click('button:has-text("Войти")');
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByText(/неверный email или пароль|Request failed with status code 401/i);
      await expect(errorMsg).toBeVisible();
    });

    test('Non-existent email shows error', async ({ page }) => {
      await page.goto('/login');
      await page.fill('input[name="email"]', 'nonexistent@example.com');
      await page.fill('input[name="password"]', 'anypass');
      await page.click('button:has-text("Войти")');
      await expect(page).toHaveURL(/\/login/);
      const errorMsg = page.getByText(/неверный email или пароль|invalid credentials|Request failed with status code 401/i);
      await expect(errorMsg).toBeVisible();
    });

    test('Empty fields trigger validation', async ({ page }) => {
      await page.goto('/login');
      await page.click('button:has-text("Войти")');
      await expect(page).toHaveURL(/\/login/);

      const errorMsg = page.getByText(/Заполните все поля/i);
      await expect(errorMsg).toBeVisible();
    });

    test('Login with very long email (validation)', async ({ page }) => {
      const longEmail = 'a'.repeat(255) + '@example.com';
      await page.goto('/login');
      await page.fill('input[name="email"]', longEmail);
      await page.fill('input[name="password"]', 'password1');
      await page.click('button:has-text("Войти")');
      // Ожидаем, что форма не отправится из-за клиентской валидации
      await expect(page).toHaveURL(/\/login/);
      const longErrorMsg = page.getByTestId('server-error');
      await expect(longErrorMsg).toHaveText(/Email is too long \(max 256 characters\).|Эл. почта не может быть длиннее 256 символов/i);
    });
  });

  test.describe('Authorization protection', () => {
    test('Redirect to /login when accessing /habits without login', async ({ page }) => {
      await page.goto('/habits');
      await expect(page).toHaveURL(/\/register/);
    });

    test('Access to /habits after successful login', async ({ page }) => {
      const email = 'tester@example.com';
      await page.goto('/login');
      await page.fill('input[name="email"]', email);
      await page.fill('input[name="password"]', 'password1');
      await page.click('button:has-text("Войти")');
      await expect(page).toHaveURL(/\/habits/);
      // Повторный переход (уже залогинен) должен остаться на /habits
      await page.goto('/habits');
      await expect(page).toHaveURL(/\/habits/);
    });
  });
});

test.describe('Logout', () => {
  test.describe('Positive scenarious', () => {
    test('Logout user', async ({ page }) => {
      // Авторизация пользователя
      const email = 'tester@example.com';
      const password = 'password1';
      loginUser(page, email, password);

      await page.getByTestId('nav-button-profile').click();
      await page.click('button:has-text("Выход")');

      await expect(page).toHaveURL(/\/login/);
    });
  });
});