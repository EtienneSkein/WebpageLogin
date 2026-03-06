import React from 'react';
import { AppShell } from '@mantine/core';
import Navbar from './Navbar';

const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return (
    <AppShell padding="md" header={<Navbar />}>
      {children}
    </AppShell>
  );
};

export default Layout;
