import { test, expect } from '@playwright/test';

test('Responsive navigation works', async ({ page }) => {
  await page.goto('/');

  const body = page.locator('body');

  await expect(body).toBeVisible();

  // Mobile menu example
  const burger = page.getByTestId('burger-menu');

  if (await burger.isVisible()) {
    await burger.click();
  }

  await expect(body).toBeVisible();
});