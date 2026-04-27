import React from 'react'
import styles from './PageLayout.module.scss'

const PageLayout = ({children, className, ...rest}) => {
  return (
    <div className = {`${styles.page} ${className || ''}`.trim()} {...rest}>{children}</div>
  )
}

export default PageLayout