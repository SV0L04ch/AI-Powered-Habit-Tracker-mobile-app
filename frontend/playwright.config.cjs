import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './end-to-end-tests',

  timeout: 60000,

  fullyParallel: false,

  workers: 1,

  retries: process.env.CI ? 2 : 0,

  use: {
    baseURL: 'http://localhost:5173',
    headless: true,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },

  projects: [
    // FULL E2E
    {
      name: 'chromium',
      testMatch: /scenarious\/.*\.spec\.js/,
      use: {
        ...devices['Desktop Chrome'],
      },
    },

    // FIREFOX SMOKE
    {
      name: 'firefox',
      testMatch: /smoke\/.*\.spec\.js/,
      use: {
        ...devices['Desktop Firefox'],
      },
    },

    // WEBKIT SMOKE
    {
      name: 'webkit',
      testMatch: /smoke\/.*\.spec\.js/,
      use: {
        ...devices['Desktop Safari'],
      },
    },

    // MOBILE
    {
      name: 'mobile-chrome',
      testMatch: /smoke\/.*\.spec\.js/,
      use: {
        ...devices['Pixel 5'],
      },
    },
  ],
});