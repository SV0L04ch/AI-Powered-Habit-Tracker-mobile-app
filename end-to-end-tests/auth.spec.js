import { test, expect } from '@playwright/test'

test('Register new user with correct data', async ({ page }) => {
    await page.goto('/register');
    await page.fill('input[name="email"]', 'tester@example.com');
    await page.selectOption('select[name="city"]', 'Москва');
    await page.fill('input[name="password"]', 'password1');
    await page.fill('input[name="confirm"]', 'password1');

    await page.click('button:has-text("Регистрация")');

    await expect(page).toHaveURL(/\/habits/);
});

test('Register new user with different passwords', async ({ page }) => {
    await page.goto('/register');
    await page.fill('input[name="email"]', 'tester@example.com');
    await page.selectOption('select[name="city"]', 'Москва');
    await page.fill('input[name="password"]', 'password1');
    await page.fill('input[name="confirm"]', 'password2');

    await page.click('button:has-text("Регистрация")');

    await expect(page).toHaveURL(/\/register/);
    
    const errorMessage = page.getByText('Пароли не совпадают');
    await expect(errorMessage).toBeVisible();
});

test('Register user with old data in database', async ({ page }) => {
    await page.goto('/register');
    await page.fill('input[name="email"]', 'tester@example.com');
    await page.selectOption('select[name="city"]', 'Москва');
    await page.fill('input[name="password"]', 'password1');
    await page.fill('input[name="confirm"]', 'password2');

    await page.click('button:has-text("Регистрация")');

    await expect(page).toHaveURL(/\/register/);
    
    const errorMessage = page.getByText('Пароли не совпадают');
    await expect(errorMessage).toBeVisible();
});

test('Login user with correct data', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[name="email"]', 'tester@example.com');
    await page.fill('input[name="password"]', 'password1');

    await page.click('button:has-text("Войти")');

    await expect(page).toHaveURL(/\/habits/);
});

test('Login user with incorrect data', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[name="email"]', 'toster@example.com');
    await page.fill('input[name="password"]', 'password123');

    await page.click('button:has-text("Войти")');

    await expect(page).toHaveURL(/\/login/);
});

test('Login user without data', async ({ page }) => {
    await page.goto('/login');

    await page.click('button:has-text("Войти")');

    await expect(page).toHaveURL(/\/login/);

    const errorMessage = page.getByText('Заполните все поля');
    await expect(errorMessage).toBeVisible();
});