
export interface UserStatisticsResponse {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  newUsersLast30Days: number;
  roleStatistics: UserRoleStatistics[];
}
export interface UserRoleStatistics {
  role: number; 
  roleName: string;
  count: number;
}