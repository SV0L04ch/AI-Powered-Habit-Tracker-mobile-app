export const CITIES = [
    'Москва', 'Санкт-Петербург', 'Новосибирск', 'Екатеринбург', 'Казань'
]

export function getTestUser() {
  const uniqueSuffix = `${Date.now()}_${Math.random().toString(36).substring(2, 8)}`;
  const randomCity = CITIES[Math.floor(Math.random() * CITIES.length)];

  return {
    email: `test_${uniqueSuffix}@example.com`,
    password: 'password1',
    city: randomCity
  };
}