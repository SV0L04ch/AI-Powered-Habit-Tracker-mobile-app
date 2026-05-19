// config.js
export const BASE_URL = __ENV.BASE_URL || 'http://localhost:5093/api';
export const MAILHOG_URL = __ENV.MAILHOG_URL || 'http://localhost:8025';
export const CITIES = ['Москва', 'Санкт-Петербург', 'Екатеринбург', 'Казань'];

export const options = {
  stages: [
    { duration: '1m', target: 20 },   // подъём до 20 VU
    { duration: '3m', target: 20 },   // стабильная нагрузка 20 VU
    { duration: '1m', target: 50 },   // подъём до 50 VU
    { duration: '3m', target: 50 },   // пик 50 VU
    { duration: '1m', target: 0 },    // спад
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'],
    http_req_failed: ['rate<0.01'],
  },
};