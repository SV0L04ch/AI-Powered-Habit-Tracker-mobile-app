import { Navigate } from 'react-router-dom'
import useAuthUser from '../../store/useAuthStore'

const GuestRoute = ({children}) => {
    const isAuth = useAuthUser((state) => state.isAuthenticated)

    if (isAuth){
        return <Navigate to="/habits" replace />
    }

    
  return children
}

export default GuestRoute