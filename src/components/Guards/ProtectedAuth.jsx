import useAuthUser from "../../store/useAuthStore"
import { Navigate } from "react-router-dom"

const ProtectedAuth = ({children}) => {
    const isAuth = useAuthUser((state) => state.isAuthenticated)
    
    if(!isAuth){
        return <Navigate to="/register" replace />
    }
    
    return children
}

export default ProtectedAuth