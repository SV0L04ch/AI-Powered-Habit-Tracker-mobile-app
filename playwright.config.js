import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './end-to-end-tests',
  use: {
    baseURL: 'http://localhost:5173',
    headless: false
  },
});