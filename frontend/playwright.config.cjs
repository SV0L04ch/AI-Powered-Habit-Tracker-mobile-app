const { defineConfig, devices } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './end-to-end-tests/scenarious',

  timeout: 60000,
  expect: {
    timeout: 10000,
  },

  fullyParallel: false,

  forbidOnly: !!process.env.CI,

  retries: process.env.CI ? 2 : 0,

  workers: 1,

  use: {
    baseURL: 'http://localhost:5173',
    headless: !!process.env.CI,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
      },
    },
  ],

  reporter: [
    ['html'],
    ['list'],
  ]
});