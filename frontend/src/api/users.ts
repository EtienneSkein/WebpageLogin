import client from './client';

export interface UserResponse {
  firstName: string;
  lastName: string;
  email: string;
}

export const getMe = () => {
  return client.get<UserResponse>('/api/users/me');
};
