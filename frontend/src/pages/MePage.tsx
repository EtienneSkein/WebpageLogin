import React, { useEffect, useState } from 'react';
import { getMe, UserResponse } from '../api/users';
import {
  Box,
  Text,
  Title,
  Loader,
  Container,
  Button,
  Group,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { Link } from 'react-router-dom';

const MePage: React.FC = () => {
  const [user, setUser] = useState<UserResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getMe()
      .then((resp) => setUser(resp.data))
      .catch((err) => {
        showNotification({
          title: 'Error',
          message: err.response?.data?.message || 'Failed to load',
          color: 'red',
        });
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading)
    return (
      <Container size="sm" sx={{ textAlign: 'center', marginTop: 40 }}>
        <Loader />
      </Container>
    );
  if (!user) return null;

  return (
    <Container size="sm">
      <Title order={2} mb="md">
        Your Profile
      </Title>
      <Text>First Name: {user.firstName}</Text>
      <Text>Last Name: {user.lastName}</Text>
      <Text>Email: {user.email}</Text>
      <Group mt="lg" position="center" spacing="md">
        <Button component={Link} to="/login" variant="outline">
          Login
        </Button>
        <Button component={Link} to="/register" variant="outline">
          Register
        </Button>
      </Group>
    </Container>
  );
};

export default MePage;