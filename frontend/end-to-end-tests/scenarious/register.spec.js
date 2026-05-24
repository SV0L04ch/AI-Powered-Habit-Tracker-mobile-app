import { test, expect } from '@playwright/test';
import { registerUser, confirmEmail } from '../helpers.js';
import { getTestUser } from '../config.js';

test.describe('Positive scenarious', () => {
    test('Register new user with correct data (only registration)', async ({ page }) => {
        const user = getTestUser();
        
        await page.goto('/register');
        await registerUser(page, user.email, user.password, user.password, user.city);

        await expect(page).toHaveURL(/\/login/, { timeout: 10000 });
    });

    test('Confirm email for existing user via MailHog', async ({ page }) => {
        const user = getTestUser();
        
        await page.goto('/register');
        await registerUser(page, user.email, user.password, user.password, user.city)
        await confirmEmail(page, user.email);
        await expect(page.locator('text=Email confirmed')).toBeVisible();
    });
});

test.describe('Negative scenarious', () => {
    test('Register with existing email (duplicate)', async ({ page }) => {
        const user = getTestUser();

        // Успешная регистрация
        await page.goto('/register');
        await registerUser(page, user.email, user.password, user.password, user.city);

        // Безуспешная регистрация с ошибкой 409
        await page.goto('/register');
        await registerUser(page, user.email, user.password, user.password, user.city);

        // Ожидание, что страница не перезагрузилась и показана ошибка
        await expect(page).toHaveURL(/\/register/);
        const errorMsg = page.getByTestId('server-error');
        await expect(errorMsg).toBeVisible();
        await expect(errorMsg).toHaveText(/уже существует|already exists|Request failed with status code 409/i);
    });

    test('Register with different passwords', async ({ page }) => {
        const email = 'test@example.com';
        const passowrd = 'password1';
        const confirmPassword = 'password2';
        const city = 'Москва';
        
        await page.goto('/register');
        await registerUser(page, email,  passowrd, confirmPassword, city);

        await expect(page).toHaveURL(/\/register/);
        const errorMsg = page.getByTestId('confPass-error');

        await expect(errorMsg).toBeVisible();
        await expect(errorMsg).toHaveText('Пароли не совпадают');
    });

    test('Register with invalid email format', async ({ page }) => {
        const email = 'invalid-email';
        const password = 'password1';
        const city = 'Москва';

        await page.goto('/register');
        await registerUser(page, email, password, password, city);

        // Ожидание, что форма не отправится (валидация на клиенте)
        await expect(page).toHaveURL(/\/register/);
        const emailInput = page.locator('input[name="email"]');
        var emailErrorMsg = await emailInput.evaluate(el => el.validationMessage);
        await expect(emailErrorMsg).not.toBe('Адрес электрнонной почты должен содержать символ "@". В адресе "invalid-email" отсутсвует символ "@".');
    });

    test('Register with too short password', async ({ page }) => {
        const email = 'short@example.com';
        const passowrd = '123';
        const city = 'Москва';
        
        await page.goto('/register');
        await registerUser(page, email, passowrd, passowrd, city);

        await expect(page).toHaveURL(/\/register/);
        const errorMsg = page.getByTestId('password-error');

        await expect(errorMsg).toBeVisible();
        await expect(errorMsg).toHaveText('Минимум 6 символов');
    });

    test('Register with empty fields', async ({ page }) => {
        await page.goto('/register');
        await page.getByTestId('reg-button').click();

        // Проврека, что ошибки показываются пользователю

        const emailErrorMsg = page.getByTestId('error-email');
        await expect(emailErrorMsg).toBeVisible();
        await expect(emailErrorMsg).toHaveText('Эл. почта обязательна');

        const cityErrorMsg = page.getByTestId('city-error');
        await expect(cityErrorMsg).toBeVisible();
        await expect(cityErrorMsg).toHaveText('Выберите город');

        const passwordErrorMsg = page.getByTestId('password-error');
        await expect(passwordErrorMsg).toBeVisible();
        await expect(passwordErrorMsg).toHaveText('Пароль обязателен');
    });

    test('Register with very long email', async ({ page }) => {
        const longEmail = 'a'.repeat(256) + '@example.com';
        const password = 'passowrd1';
        const city = 'Москва';
        
        await page.goto('/register');
        await registerUser(page, longEmail, password, password, city);

        // Ожидание, что сервер вернёт 400 или клиентская валидация сработает
        await expect(page).toHaveURL(/\/register/);
        const error = page.getByTestId('server-error');
        await expect(error).toBeVisible();
        await expect(error).toHaveText(/too long|превышает|Request failed with status code 400/i);
    });
});