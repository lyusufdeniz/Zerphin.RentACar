import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  User,
  CreateUserCommand,
  UpdateUserCommand,
  UserSearchParams,
  PaginatedUserResponse,
  DeleteUserCommand,
} from '../models/user';
import { forkJoin } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private httpService = inject(HttpService);
  private baseUrl = '/Users';

  /**
   * Search users with filters
   */
  searchUsers(params?: UserSearchParams): Observable<PaginatedUserResponse> {
    return this.httpService.get<PaginatedUserResponse>(
      `${this.baseUrl}/search`,
      {
        params: params as any,
      }
    );
  }

  /**
   * Get user by ID
   */
  getUserById(id: string): Observable<User> {
    return this.httpService.get<User>(`${this.baseUrl}/${id}`);
  }

  /**
   * Create new user
   */
  createUser(command: CreateUserCommand): Observable<User> {
    return this.httpService.post<User>(this.baseUrl, command);
  }

  /**
   * Update user
   */
  updateUser(command: UpdateUserCommand): Observable<User> {
    return this.httpService.put<User>(this.baseUrl, command);
  }

  /**
   * Delete user
   */
  deleteUser(id: string): Observable<void> {
    const command: DeleteUserCommand = { id };
    return this.httpService.delete<void>(this.baseUrl, {
      params: command as any,
    });
  }
}

