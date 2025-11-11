export enum UserRole {
  Customer = 1,
  Employee = 2,
  Manager = 3,
  Admin = 4,
}
export const UserRoleNames: Record<UserRole, string> = {
  [UserRole.Customer]: 'Müşteri',
  [UserRole.Employee]: 'Çalışan',
  [UserRole.Manager]: 'Yönetici',
  [UserRole.Admin]: 'Admin',
};