import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

const RequireAuth: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const auth = useAuth();
  if (!auth.user) {
    return <Navigate to="/login" replace />;
  }
  return children;
};

export default RequireAuth;
