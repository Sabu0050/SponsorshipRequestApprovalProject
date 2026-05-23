export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResult {
  accessToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  roles: string[];
}
