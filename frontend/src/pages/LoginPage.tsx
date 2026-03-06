import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login, LoginData } from '../api/auth';
import { useAuth } from '../auth/AuthContext';
import {
  TextInput,
  PasswordInput,
  Button,
  Container,
  Title,
  Box,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const auth = useAuth();

  const [form, setForm] = useState<LoginData>({
    email: '',
    password: '',
  });
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const resp = await login(form);
      const token = resp.data.token;
      auth.login(form.email, token);
      navigate('/me');
    } catch (err: any) {
      showNotification({
        title: 'Login failed',
        message: err.response?.data?.message || 'Login failed',
        color: 'red',
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container size="sm">
      <Title order={2} align="center" mb="md">
        Login
      </Title>
      <Box component="form" onSubmit={handleSubmit}>
        <TextInput
          label="Email"
          name="email"
          type="email"
          value={form.email}
          onChange={handleChange}
          required
          mb="sm"
        />
        <PasswordInput
          label="Password"
          name="password"
          value={form.password}
          onChange={handleChange}
          required
          mb="sm"
        />
        <Button fullWidth type="submit" loading={loading} mt="md">
          Login
        </Button>
      </Box>
    </Container>
  );
};

export default LoginPage;