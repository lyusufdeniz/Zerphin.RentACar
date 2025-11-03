/**
 * User Statistics Response Models
 */
export interface UserStatisticsResponse {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  newUsersLast30Days: number;
  roleStatistics: UserRoleStatistics[];
}

export interface UserRoleStatistics {
  role: number; // UserRole enum (1-4)
  roleName: string;
  count: number;
}


