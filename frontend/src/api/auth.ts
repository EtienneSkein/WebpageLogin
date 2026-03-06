import client from './client';

export interface RegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface LoginData {
  email: string;
  password: string;
}

export const register = (data: RegisterData) => {
  return client.post('/api/auth/register', data);
};

export const login = (data: LoginData) => {
  return client.post('/api/auth/login', data);
};
