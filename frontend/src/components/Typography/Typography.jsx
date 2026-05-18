import React from 'react'
import styles from './Typography.module.scss'

const variantToElement = {
    headline1: 'h1', 
    headline2: 'h2', 
    headline3: 'h3', 
    body1: 'p', 
    body2: 'p', 
    caption: 'span',  
}

const Typography = ({variant = "body1", component, children, className, ...rest}) => {
    const Tag = component || variantToElement[variant] || 'span';
    const classes = `${styles.typography} ${styles[variant]} ${className || ''}`.trim()
  return React.createElement(Tag, { className: classes, ...rest}, children)
}

export default Typography