import { execSync } from 'child_process';

const testFiles = [
  'end-to-end-tests/scenarious/register.spec.js',
  'end-to-end-tests/scenarious/login.spec.js',
  'end-to-end-tests/scenarious/habits.spec.js',
  'end-to-end-tests/scenarious/profile.spec.js',
];

const smokeTestFiles = [
  'end-to-end-tests/smoke/homepage.spec.js',
  'end-to-end-tests/smoke/login-ui.spec.js',
  'end-to-end-tests/smoke/responsive.spec.js'
];

let hasErrors = false;

for (const file of testFiles) {
  console.log(`\n🚀 Running ${file}...`);
  try {
    execSync(`npx playwright test ${file} --workers=1`, { stdio: 'inherit' });
  } catch (error) {
    console.error(`❌ Tests failed in ${file}`);
    hasErrors = true;
  }
}

if (hasErrors) {
  console.error('❌ Some tests failed. See logs above.');
  process.exit(1);
} else {
  console.log('✅ All tests passed in order!');
}

let smokeHasErrors = false;

for (const file of smokeTestFiles) {
  console.log(`\n🚀 Running ${file}...`);
  try {
    execSync(`npx playwright test ${file} --workers=1`, { stdio: 'inherit' });
  } catch (error) {
    console.error(`❌ Smoke tests failed in ${file}`);
    smokeHasErrors = true;
  }
}

if (hasErrors) {
  console.error('❌ Some smoke tests failed. See logs above.');
  process.exit(1);
} else {
  console.log('✅ All smoke tests passed in order!');
}