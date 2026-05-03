import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5093', // внешний порт API из docker-compose
        changeOrigin: true,
        secure: false,
      },
    },
  },
});