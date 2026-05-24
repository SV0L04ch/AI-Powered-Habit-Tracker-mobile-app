import { expect } from '@playwright/test'

export async function registerUser(page, email, password, confirmPassword, city) {
  await page.fill('input[name="email"]', email);
  await page.fill('input[name="password"]', password);
  await page.fill('input[name="confirm"]', confirmPassword);
  await page.selectOption('select[name="city"]', city);
  await page.getByTestId('reg-button').click();
}

export async function loginUser(page, email, password) {
    await page.fill('input[name="email"]', email);
    await page.fill('input[name="password"]', password);
    await page.getByTestId('login-button').click();
}

export async function confirmEmail(page, email, timeout = 20000, interval = 1000) {
  const startTime = Date.now();
  let confirmLink = null;

  while (Date.now() - startTime < timeout) {
    const response = await page.request.get('http://localhost:8025/api/v2/messages');
    const data = await response.json();
    const message = data.items.find(msg => msg.Content.Headers.To[0]?.includes(email));

    if (message) {
      const html = Buffer.from(message.Content.Body, 'base64').toString('utf-8');
      const match = html.match(/href='(http:\/\/localhost:5093\/api\/auth\/confirm-email\?[^']+)'/);
      if (match) {
        confirmLink = match[1];
        break;
      }
    }
    await page.waitForTimeout(interval);
  }

  if (!confirmLink) {
    throw new Error(`Confirmation email for ${email} not found within ${timeout}ms`);
  }

  await page.goto(confirmLink);
  await page.waitForSelector('text=Email confirmed', { timeout: 5000 });
}