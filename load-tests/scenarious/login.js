import { options } from '../config.js';
import {
  generateTestData,
  register,
  getConfirmationLink,
  confirmEmail,
  login,
} from '../helpers.js';

export { options };

export function setup() {
  const numberOfUsers = 20;
  const users = [];
  for (let i = 0; i < numberOfUsers; i++) {
    const { email, password, city } = generateTestData(999, i);
    const { ok: regOk } = register(email, password, city);
    if (!regOk) continue;
    const link = getConfirmationLink(email);
    if (!link) continue;
    const { ok: confOk } = confirmEmail(link);
    if (!confOk) continue;
    users.push({ email, password });
  }
  console.log(`Created ${users.length} users`);
  return users;
}

export default function (data) {
  if (data.length === 0) return;
  const user = data[Math.floor(Math.random() * data.length)];
  login(user.email, user.password);
  // sleep(0.5);
}