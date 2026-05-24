export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginOption {
  role: string;
  email: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  roles: string[];
}
