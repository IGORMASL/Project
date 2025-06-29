import { Navigate, useLocation } from 'react-router-dom';

function ProtectedRoute({ user, requiredRole, children }) {
  const location = useLocation(); 
  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (requiredRole && user.Role !== requiredRole) {
    return <Navigate to="/" replace/>;
  }

  return children;
}

export default ProtectedRoute;
