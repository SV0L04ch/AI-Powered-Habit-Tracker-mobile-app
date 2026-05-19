import http from 'k6/http';
import { check, sleep } from 'k6';
import encoding from 'k6/encoding';
import { BASE_URL, MAILHOG_URL, CITIES } from './config.js';

// Декодирование Base64 без TextDecoder (только ASCII-символы)
function decodeBase64ToUTF8(base64) {
  const bytes = encoding.b64decode(base64, 'std', false);
  const uint8array = new Uint8Array(bytes);
  let result = '';
  for (let i = 0; i < uint8array.length; i++) {
    result += String.fromCharCode(uint8array[i]);
  }
  return result;
}

// Получение ссылки подтверждения из MailHog
export function getConfirmationLink(email, maxAttempts = 20, delaySec = 1) {
  for (let attempt = 0; attempt < maxAttempts; attempt++) {
    const resp = http.get(`${MAILHOG_URL}/api/v2/messages`);
    if (resp.status !== 200) {
      sleep(delaySec);
      continue;
    }
    const data = resp.json();
    const message = data.items?.find(msg =>
      msg.Content?.Headers?.To?.[0]?.includes(email)
    );
    if (message) {
      const html = decodeBase64ToUTF8(message.Content.Body);
      // Поиск ссылки с одинарными или двойными кавычками
      const match = html.match(/href=["'](http:\/\/localhost:5093\/api\/auth\/confirm-email\?[^"']+)["']/);
      if (match) {
        console.log(`✅ Confirmation link found for ${email}`);
        return match[1];
      }
    }
    sleep(delaySec);
  }
  console.error(`❌ No confirmation link for ${email} after ${maxAttempts} attempts`);
  return null;
}

// Регистрация
export function register(email, password, city) {
  const payload = JSON.stringify({
    email,
    password,
    confirm: password,   // важно! без этого поля будет 400
    city,
  });
  const res = http.post(`${BASE_URL}/auth/register`, payload, {
    headers: { 'Content-Type': 'application/json' },
  });
  const ok = check(res, { 'Registration status 201': (r) => r.status === 201 });
  if (!ok) {
    console.error(`Registration failed for ${email}: ${res.status} ${res.body}`);
  }
  return { ok, res };
}

// Подтверждение email
export function confirmEmail(link) {
  const res = http.get(link);
  const ok = check(res, { 'Confirmation status 200': (r) => r.status === 200 });
  if (!ok) console.error(`Confirmation failed: ${res.status}`);
  return { ok, res };
}

// Логин (куки сохраняются автоматически)
export function login(email, password) {
  const payload = JSON.stringify({ email, password });
  const res = http.post(`${BASE_URL}/auth/login`, payload, {
    headers: { 'Content-Type': 'application/json' },
  });
  const ok = check(res, { 'Login status 200': (r) => r.status === 200 });
  if (!ok) console.error(`Login failed for ${email}: ${res.status}`);
  return { ok, res };
}

export function login(email, password) {
  const payload = JSON.stringify({ email, password });
  const res = http.post(`${BASE_URL}/auth/login`, payload, {
    headers: { 'Content-Type': 'application/json' },
  });
  const ok = check(res, { 'Login status 200': (r) => r.status === 200 });
  if (!ok) console.error(`Login failed for ${email}: ${res.status} ${res.body}`);
  return { ok, res };
}

// Генерация уникальных тестовых данных (принимает __VU и __ITER)
export function generateTestData(vu, iter) {
  const timestamp = Date.now();
  const email = `test_${vu}_${iter}_${timestamp}@example.com`;
  const password = 'Password1!';
  const city = CITIES[Math.floor(Math.random() * CITIES.length)];
  return { email, password, city };
}