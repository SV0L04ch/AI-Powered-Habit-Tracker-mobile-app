import { motion } from 'framer-motion';
import HeroSection from './components/HeroSection';
import FeaturesSection from './components/FeaturesSection';
import InteractiveDemo from './components/InteractiveDemo';
import VideoShowcase from './components/VideoShowcase';
import AppPreview3D from './components/AppPreview3D';
import HowItWorks from './components/HowItWorks';
import SocialProof from './components/SocialProof';
import CtaSection from './components/CtaSection';
import LandingFooter from './components/LandingFooter';
import styles from './LandingPage.module.scss';

const pageVariants = {
  initial: { opacity: 0 },
  animate: { opacity: 1, transition: { duration: 0.5 } },
  exit: { opacity: 0 },
};

export default function LandingPage() {
  return (
    <motion.div
      className={styles.landing}
      variants={pageVariants}
      initial="initial"
      animate="animate"
      exit="exit"
    >
      <HeroSection />
      <FeaturesSection />
      <InteractiveDemo />
      <VideoShowcase />
      <AppPreview3D />
      <HowItWorks />
      <SocialProof />
      <CtaSection />
      <LandingFooter />
    </motion.div>
  );
}
