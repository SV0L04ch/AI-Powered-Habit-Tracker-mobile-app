import { options } from '../config.js';
import { generateTestData, register, getConfirmationLink, confirmEmail } from '../helpers.js';

export { options };

export default function () {
  const { email, password, city } = generateTestData(__VU, __ITER);

  const { ok: regOk } = register(email, password, city);
  if (!regOk) return;

  const confirmLink = getConfirmationLink(email);
  if (!confirmLink) {
    console.error(`No confirmation link for ${email}`);
    return;
  }

  const { ok: confOk } = confirmEmail(confirmLink);
  if (!confOk) console.error(`Confirmation failed for ${email}`);
}