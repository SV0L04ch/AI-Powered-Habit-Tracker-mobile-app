import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import useAuthStore from '../../store/useAuthStore';
import styles from './RegisterPage.module.scss';

const steps = ['Account', 'Security', 'Interests'];

export default function RegisterPage() {
  const [step, setStep] = useState(0);
  const [email, setEmail] = useState('');
  const [city, setCity] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [selectedInterests, setSelectedInterests] = useState([]);
  const { register, isLoading, error, clearError } = useAuthStore();
  const navigate = useNavigate();

  const interests = [
    '🧘 Meditation', '📚 Reading', '💧 Hydration', '💪 Fitness',
    '📝 Journaling', '😴 Sleep', '🥗 Nutrition', '💻 Coding',
    '📵 No Social Media', '🌬️ Breathing', '🎯 Productivity', '💤 Rest',
  ];

  const toggleInterest = (interest) => {
    setSelectedInterests(prev =>
      prev.includes(interest) ? prev.filter(i => i !== interest) : [...prev, interest]
    );
  };

  const passwordStrength = password.length >= 8 ? 'strong' : password.length >= 6 ? 'medium' : 'weak';

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (step === 0) { setStep(1); return; }
    if (step === 1) { if (password !== confirmPassword) return; setStep(2); return; }
    clearError();
    await register(email, city, password);
    navigate('/login');
  };

  return (
    <div className={styles.page}>
      <div className={styles.left}>
        <div className={styles.blob1} />
        <div className={styles.blob2} />
        <div className={styles.blob3} />
        <div className={styles.leftContent}>
          <h1>Start your journey.</h1>
          <p>Join thousands building better habits.</p>
        </div>
      </div>

      <div className={styles.right}>
        <div className={styles.formCard}>
          <h2>Create account</h2>
          <div className={styles.progressBar}>
            {steps.map((s, i) => (
              <div key={i} className={`${styles.progressStep} ${i <= step ? styles.active : ''}`}>
                <div className={styles.stepDot}>{i < step ? '✓' : i + 1}</div>
                <span>{s}</span>
              </div>
            ))}
          </div>

          {error && <div className={styles.error}>{error}</div>}

          <form onSubmit={handleSubmit}>
            {step === 0 && (
              <div className={styles.stepContent}>
                <div className={styles.field}>
                  <label>Email</label>
                  <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="you@gmail.com" required />
                </div>
                <div className={styles.field}>
                  <label>City</label>
                  <input type="text" value={city} onChange={(e) => setCity(e.target.value)} placeholder="Your city" required />
                </div>
              </div>
            )}

            {step === 1 && (
              <div className={styles.stepContent}>
                <div className={styles.field}>
                  <label>Password</label>
                  <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Min 6 characters" required />
                  <div className={`${styles.strength} ${styles[passwordStrength]}`}>
                    {passwordStrength}
                  </div>
                </div>
                <div className={styles.field}>
                  <label>Confirm Password</label>
                  <input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} placeholder="Repeat password" required />
                </div>
              </div>
            )}

            {step === 2 && (
              <div className={styles.stepContent}>
                <p className={styles.interestsHint}>Pick at least 3 interests:</p>
                <div className={styles.interests}>
                  {interests.map((interest) => (
                    <button
                      key={interest}
                      type="button"
                      className={`${styles.interestBtn} ${selectedInterests.includes(interest) ? styles.selected : ''}`}
                      onClick={() => toggleInterest(interest)}
                    >
                      {interest}
                    </button>
                  ))}
                </div>
              </div>
            )}

            <button type="submit" className={styles.submitBtn} disabled={isLoading}>
              {step === 2 ? (isLoading ? 'Creating...' : 'Create Account') : 'Continue'}
            </button>
          </form>

          {step > 0 && (
            <button className={styles.backBtn} onClick={() => setStep(step - 1)}>← Back</button>
          )}

          <p className={styles.switchText}>
            Already have an account? <Link to="/login">Sign in</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
