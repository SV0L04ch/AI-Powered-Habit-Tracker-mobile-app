import { test, expect } from '@playwright/test';

test('Login page UI renders', async ({ page }) => {
  await page.goto('/login');

  await expect(
    page.locator('input[name="email"]')
  ).toBeVisible();

  await expect(
    page.locator('input[name="password"]')
  ).toBeVisible();

  await expect(
    page.getByTestId('login-button')
  ).toBeVisible();
});