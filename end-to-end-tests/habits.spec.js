import { test, expect } from '@playwright/test';

test('Add new correct habit', async ({ page }) => {
    await page.goto('/habits');

    await page.fill('input[name="name"]', 'meditation');

    const addButton = page.locator('._addButton_13rtf_7');
    await addButton.waitFor({ state: 'visible' });
    await addButton.click();

    await expect(page).toHaveURL(/\/habits\/new/);

    const timeButton = page.locator('_btn_1mium_1 _primary_1mium_29');
    await timeButton.waitFor({ state: 'visible' });
    await timeButton.click();

    const difficultyButton = page.locator('_typography_ohgns_1 _caption_ohgns_41 _basicText_v5cd6_23 _desc_v5cd6_26');
    await difficultyButton.waitFor({ state: 'visible' });
    await timeButton.click();

    await page.fill('input[name="trigger_value"]', '08:00');

    const createButton = page.locator('_btn_1mium_1 _primary_1mium_29');
    await createButton.waitFor({ state: 'visible' });
    await createButton.click();

    await expect(page).toHaveURL(/\/habits/);
});