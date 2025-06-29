export interface LoginResponse {
  access_token: string;
  refresh_token?: string;
  expires_in: number;
  user: User;
  requiresTwoFactor?: boolean;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  employeeId?: string;
  department?: string;
  position?: string;
  profilePictureUrl?: string;
  status: string;
  roles: string[];
}

export interface RegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phoneNumber?: string;
  employeeId?: string;
  department?: string;
  position?: string;
}
