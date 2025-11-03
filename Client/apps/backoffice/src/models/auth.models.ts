import { UserRole } from './user/user-role.model';
import { CreateUserCommand } from './user/user-request.model';
import { ServiceResult } from './api/api-response.model';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: LoginUserResponse;
}

export interface LoginUserResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roleName: string;
}

// Backward compatibility - keep UserResponse as alias
export type UserResponse = LoginUserResponse;

// RegisterRequest uses the same structure as CreateUserCommand
export type RegisterRequest = CreateUserCommand;

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
}

// Re-export for backward compatibility
export { UserRole };
export type { ServiceResult };
