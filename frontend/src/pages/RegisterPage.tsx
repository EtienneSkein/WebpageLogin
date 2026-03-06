import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { register, RegisterData } from '../api/auth';
import {
  TextInput,
  PasswordInput,
  Button,
  Container,
  Title,
  Box,
  List,
  ThemeIcon,
  Text,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { IconCheck, IconX } from '@tabler/icons-react';
import {
  validatePassword,
  isPasswordValid,
  PasswordRequirements,
  PASSWORD_RULES,
} from '../utils/passwordValidator';

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState<RegisterData>({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
  });
  const [loading, setLoading] = useState(false);
  const [passwordRequirements, setPasswordRequirements] =
    useState<PasswordRequirements | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newPassword = e.target.value;
    setForm({ ...form, password: newPassword });

    if (newPassword.length > 0) {
      const requirements = validatePassword(newPassword);
      setPasswordRequirements(requirements);
    } else {
      setPasswordRequirements(null);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate password before submission
    if (!isPasswordValid(form.password)) {
      showNotification({
        title: 'Invalid password',
        message: 'Password does not meet requirements.',
        color: 'red',
      });
      return;
    }

    setLoading(true);
    try {
      await register(form);
      showNotification({
        title: 'Success',
        message: 'Registration completed, please log in.',
        color: 'green',
      });
      navigate('/login');
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Registration failed';
      const errors = err.response?.data?.errors || [];
      showNotification({
        title: 'Error',
        message: errorMessage + (errors.length ? '\n' + errors.join('\n') : ''),
        color: 'red',
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container size="sm">
      <Title order={2} align="center" mb="md">
        Register
      </Title>
      <Box component="form" onSubmit={handleSubmit}>
        <TextInput
          label="First Name"
          name="firstName"
          value={form.firstName}
          onChange={handleChange}
          required
          mb="sm"
        />
        <TextInput
          label="Last Name"
          name="lastName"
          value={form.lastName}
          onChange={handleChange}
          required
          mb="sm"
        />
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
          onChange={handlePasswordChange}
          required
          mb="sm"
          error={form.password.length > 0 && !isPasswordValid(form.password)}
        />
        {form.password.length > 0 && passwordRequirements && (
          <Box mb="sm">
            <Text weight={500} mb="xs">
              Password Requirements:
            </Text>
            <List spacing="xs" size="sm" withPadding>
              <List.Item
                icon={
                  <ThemeIcon color={
                    passwordRequirements.minLength ? 'green' : 'red'
                  } size={20} radius="xl">
                    {passwordRequirements.minLength ? (
                      <IconCheck size={12} />
                    ) : (
                      <IconX size={12} />
                    )}
                  </ThemeIcon>
                }
              >
                At least {PASSWORD_RULES.minLength} characters
              </List.Item>
              <List.Item
                icon={
                  <ThemeIcon color={
                    passwordRequirements.hasUpperCase ? 'green' : 'red'
                  } size={20} radius="xl">
                    {passwordRequirements.hasUpperCase ? (
                      <IconCheck size={12} />
                    ) : (
                      <IconX size={12} />
                    )}
                  </ThemeIcon>
                }
              >
                At least one uppercase letter (A-Z)
              </List.Item>
              <List.Item
                icon={
                  <ThemeIcon color={
                    passwordRequirements.hasLowerCase ? 'green' : 'red'
                  } size={20} radius="xl">
                    {passwordRequirements.hasLowerCase ? (
                      <IconCheck size={12} />
                    ) : (
                      <IconX size={12} />
                    )}
                  </ThemeIcon>
                }
              >
                At least one lowercase letter (a-z)
              </List.Item>
              <List.Item
                icon={
                  <ThemeIcon color={
                    passwordRequirements.hasNumber ? 'green' : 'red'
                  } size={20} radius="xl">
                    {passwordRequirements.hasNumber ? (
                      <IconCheck size={12} />
                    ) : (
                      <IconX size={12} />
                    )}
                  </ThemeIcon>
                }
              >
                At least one number (0-9)
              </List.Item>
              <List.Item
                icon={
                  <ThemeIcon color={
                    passwordRequirements.hasSpecialChar ? 'green' : 'red'
                  } size={20} radius="xl">
                    {passwordRequirements.hasSpecialChar ? (
                      <IconCheck size={12} />
                    ) : (
                      <IconX size={12} />
                    )}
                  </ThemeIcon>
                }
              >
                At least one special character (!@#$%^&* etc.)
              </List.Item>
            </List>
          </Box>
        )}
        <Button fullWidth type="submit" loading={loading} mt="md">
          Register
        </Button>
      </Box>
    </Container>
  );
};

export default RegisterPage;