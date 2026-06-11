import { useState, useEffect } from 'react';
import apiClient from '../../services/apiClient';
import styles from './EconomicsPage.module.scss';
export default function EconomicsPage() {
  const [wallet, setWallet] = useState(null);
  const [transactions, setTransactions] = useState([]);
  useEffect(() => { apiClient.get('/economics/wallet').then(r => setWallet(r.data)); apiClient.get('/economics/transactions').then(r => setTransactions(r.data)); }, []);
  return (
    <div className={styles.page}>
      <h1>Economics</h1>
      <div className={styles.wallet}>
        <div className={styles.balance}><span className={styles.coin}>💰</span><span className={styles.amount}>{wallet?.balance || 0}</span><span className={styles.unit}>HabitCoins</span></div>
        <div className={styles.total}>Total earned: {wallet?.totalEarned || 0}</div>
      </div>
      <h2>Transactions</h2>
      <div className={styles.list}>
        {transactions.map(t => (
          <div key={t.id} className={styles.tx}>
            <span className={styles.txDesc}>{t.description}</span>
            <span className={`${styles.txAmount} ${t.amount > 0 ? styles.positive : styles.negative}`}>{t.amount > 0 ? '+' : ''}{t.amount}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
