import { Component } from 'react';
import styles from './ErrorBoundary.module.scss';

export default class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    console.error('ErrorBoundary caught:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className={styles.container}>
          <div className={styles.card}>
            <span className={styles.icon}>⚠️</span>
            <h2 className={styles.title}>Something went wrong</h2>
            <p className={styles.message}>{this.state.error?.message || 'An unexpected error occurred.'}</p>
            <button className={styles.retryBtn} onClick={() => this.setState({ hasError: false, error: null })}>
              Try Again
            </button>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
