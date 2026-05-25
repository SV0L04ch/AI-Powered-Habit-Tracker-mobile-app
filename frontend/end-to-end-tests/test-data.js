import crypto from 'crypto';

export const CITIES = [
    'Москва', 
    'Санкт-Петербург', 
    'Новосибирск', 
    'Екатеринбург',
    'Казань'
]

export function getTestUser() {
  const uniqueSuffix = crypto.randomUUID();
  const randomCity = CITIES[Math.floor(Math.random() * CITIES.length)];

  return {
    email: `test_${uniqueSuffix}@example.com`,
    password: 'password1',
    city: randomCity
  };
}