import { test, expect } from '@playwright/test'

// Вспомогательная функция для регистрации пользователя
async function registerUser(page, email, password = 'password1', confirmPassword = 'password1', city = 'Москва') {
  await page.fill('input[name="email"]', email);
  await page.fill('input[name="password"]', password);
  await page.fill('input[name="confirm"]', confirmPassword);
  await page.selectOption('select[name="city"]', city);
  await page.getByTestId('reg-button').click();
}

test.describe('Positive scenarious', () => {
    test('Register new user with correct data (only registration)', async ({ page }) => {
        const email = `tester@example.com`;

        await page.goto('/register');
        registerUser(page, email);

        await expect(page).toHaveURL(/\/login/, { timeout: 10000 });
    });

    test('Confirm email for existing user via MailHog', async ({ page }) => {
        const email = 'tester@example.com';

        let confirmLink = null;
        for (let i = 0; i < 20; i++) {
            await page.waitForTimeout(1000);
            const response = await page.request.get('http://localhost:8025/api/v2/messages');
            const data = await response.json();
            const message = data.items.find(
            (msg) =>
                msg.Content.Headers.To[0]?.includes(email)
            );

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
    });
});

test.describe('Negative scenarious', () => {
    test('Register with existing email (duplicate)', async ({ page }) => {
        const email = 'tester@example.com';
        
        await page.goto('/register');
        registerUser(page, email);

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
        
        await page.goto('/register');
        registerUser(page, email,  passowrd, confirmPassword);

        await expect(page).toHaveURL(/\/register/);
        const errorMsg = page.getByTestId('confPass-error');

        await expect(errorMsg).toBeVisible();
        await expect(errorMsg).toHaveText('Пароли не совпадают');
    });

    test('Register with invalid email format', async ({ page }) => {
        const email = 'invalid-email';

        await page.goto('/register');
        registerUser(page, email);

        // Ожидание, что форма не отправится (валидация на клиенте)
        await expect(page).toHaveURL(/\/register/);
        const emailInput = page.locator('input[name="email"]');
        var emailErrorMsg = await emailInput.evaluate(el => el.validationMessage);
        await expect(emailErrorMsg).not.toBe('Адрес электрнонной почты должен содержать символ "@". В адресе "invalid-email" отсутсвует символ "@".');
    });

    test('Register with too short password', async ({ page }) => {
        const email = 'short@example.com';
        const passowrd = '123';
        
        await page.goto('/register');
        registerUser(page, email, passowrd, passowrd);

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
        
        await page.goto('/register');
        registerUser(longEmail);

        // Ожидание, что сервер вернёт 400 или клиентская валидация сработает
        await expect(page).toHaveURL(/\/register/);
        const error = page.getByTestId('server-error');
        await expect(error).toBeVisible();
        await expect(error).toHaveText(/too long|превышает|Request failed with status code 400/i);
    });
});