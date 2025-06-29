export interface LoginCredentials {
  email: string;
  password: string;
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
}

export interface AuthResponse {
  token: string;
  refreshToken?: string;
  user: User;
}
