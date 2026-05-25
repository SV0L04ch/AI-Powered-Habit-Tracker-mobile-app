import React from 'react';
import styles from './Substrate.module.scss';
import images from '../../lib/images';
import icons from '../../lib/icons';

const Substrate = ({
  children,
  variant = 'main',
  icon,
  image,
  alt = '',
  className = '',
  ...rest
}) => {
  const classes = `${styles.sub} ${styles[variant] || ''} ${className}`.trim();
  const imgSrc = images[image];
  const IconComponent = icons[icon];

  return (
    <div className={classes} {...rest}>
      {IconComponent && <IconComponent className={styles.icon} />}
      {imgSrc && <img src={imgSrc} alt={alt} className={styles.image} />}
      {children}
    </div>
  );
};

export default Substrate;
