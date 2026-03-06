import React, { createContext, useState, useContext, ReactNode } from 'react';

interface AuthContextType {
  user: string | null;
  login: (email: string, token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<string | null>(() => {
    const token = localStorage.getItem('token');
    const email = localStorage.getItem('email');
    return token && email ? email : null;
  });

  const login = (email: string, token: string) => {
    localStorage.setItem('token', token);
    localStorage.setItem('email', email);
    setUser(email);
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
