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
  ChangePasswordCommand,
} from '../models/user';
import { forkJoin } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private httpService = inject(HttpService);
  private baseUrl = '/Users';

  searchUsers(params?: UserSearchParams): Observable<PaginatedUserResponse> {
    return this.httpService.get<PaginatedUserResponse>(
      `${this.baseUrl}/search`,
      {
        params: params as any,
      }
    );
  }

  getUserById(id: string): Observable<User> {
    return this.httpService.get<User>(`${this.baseUrl}/${id}`);
  }

  createUser(command: CreateUserCommand): Observable<User> {
    return this.httpService.post<User>(this.baseUrl, command);
  }

  updateUser(command: UpdateUserCommand): Observable<User> {
    return this.httpService.put<User>(this.baseUrl, command);
  }

  deleteUser(id: string): Observable<void> {
    const command: DeleteUserCommand = { id };
    return this.httpService.delete<void>(this.baseUrl, {
      params: command as any,
    });
  }

  changePassword(command: ChangePasswordCommand): Observable<void> {
    return this.httpService.put<void>(`${this.baseUrl}/change-password`, command);
  }
}
