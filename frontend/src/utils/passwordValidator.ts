export interface PasswordRequirements {
  minLength: boolean;
  hasUpperCase: boolean;
  hasLowerCase: boolean;
  hasNumber: boolean;
  hasSpecialChar: boolean;
}

export const PASSWORD_RULES = {
  minLength: 8,
  requireUpperCase: true,
  requireLowerCase: true,
  requireNumber: true,
  requireSpecialChar: true,
};

export const validatePassword = (password: string): PasswordRequirements => {
  return {
    minLength: password.length >= PASSWORD_RULES.minLength,
    hasUpperCase: /[A-Z]/.test(password),
    hasLowerCase: /[a-z]/.test(password),
    hasNumber: /\d/.test(password),
    hasSpecialChar: /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password),
  };
};

export const isPasswordValid = (password: string): boolean => {
  const requirements = validatePassword(password);
  return (
    requirements.minLength &&
    requirements.hasUpperCase &&
    requirements.hasLowerCase &&
    requirements.hasNumber &&
    requirements.hasSpecialChar
  );
};

export const getPasswordErrorMessage = (): string[] => {
  return [
    `At least ${PASSWORD_RULES.minLength} characters`,
    'At least one uppercase letter',
    'At least one lowercase letter',
    'At least one number',
    'At least one special character (!@#$%^&* etc.)',
  ];
};
