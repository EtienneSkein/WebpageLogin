import React from 'react';
import { Header, Group, Button, Text, Menu, Container } from '@mantine/core';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

const Navbar: React.FC = () => {
  const auth = useAuth();

  return (
    <Header height={60} p="xs">
      <Container size="sm" sx={{ display: 'flex', alignItems: 'center', height: '100%' }}>
        <Group position="apart" style={{ width: '100%' }}>
          <Text weight={700}>WebAppLogin</Text>
          {auth.user ? (
            <Menu>
              <Menu.Target>
                <Button variant="subtle">{auth.user}</Button>
              </Menu.Target>
              <Menu.Dropdown>
                <Menu.Item onClick={auth.logout}>Logout</Menu.Item>
              </Menu.Dropdown>
            </Menu>
          ) : (
            <Group>
              <Button component={Link} to="/login" variant="subtle">
                Login
              </Button>
              <Button component={Link} to="/register" variant="subtle">
                Register
              </Button>
            </Group>
          )}
        </Group>
      </Container>
    </Header>
  );
};

export default Navbar;
