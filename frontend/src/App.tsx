import React from 'react';
import { Routes, Route } from 'react-router-dom';
import RegisterPage from './pages/RegisterPage';
import LoginPage from './pages/LoginPage';
import MePage from './pages/MePage';
import Layout from './components/Layout';
import RequireAuth from './auth/RequireAuth';

const App: React.FC = () => {
  return (
    <Layout>
      <Routes>
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/me"
          element={
            <RequireAuth>
              <MePage />
            </RequireAuth>
          }
        />
      </Routes>
    </Layout>
  );
};

export default App;
