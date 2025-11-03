import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { UserService } from '../../services/user.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { UserDetailComponent } from '../../components/user-detail/user-detail.component';
import {
  User,
  UserRole,
  UserRoleNames,
  UserSearchParams,
  PaginatedUserResponse,
} from '../../models/user';

// For template usage
export const UserRoleEnum = UserRole;

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, FavoriteButtonComponent],
  templateUrl: './users.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private cdr = inject(ChangeDetectorRef);

  // Data
  users: User[] = [];
  totalCount = 0;
  isLoading = false;

  // Pagination
  currentPage = 1;
  pageSize = 100;
  totalPages = 0;

  // Filters
  searchEmail = '';
  searchFirstName = '';
  searchLastName = '';
  searchPhoneNumber = '';
  searchIdentityNumber = '';
  searchLicenseNumber = '';
  selectedRole?: UserRole;
  selectedIsActive?: boolean;
  selectedIsVerified?: boolean;

  // Enums for template
  roles = Object.values(UserRole).filter(
    (v) => typeof v === 'number'
  ) as UserRole[];
  roleNames = UserRoleNames;
  UserRole = UserRole; // Export to template

  // Order
  orderBy = 'Id';
  isDescending = false;

  ngOnInit() {
    this.loadUsers();
  }

  /**
   * Load users with current search params
   */
  loadUsers() {
    this.isLoading = true;
    this.cdr.markForCheck();

    // Build search params
    const params: UserSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchEmail) params.Email = this.searchEmail;
    if (this.searchFirstName) params.FirstName = this.searchFirstName;
    if (this.searchLastName) params.LastName = this.searchLastName;
    if (this.searchPhoneNumber) params.PhoneNumber = this.searchPhoneNumber;
    if (this.searchIdentityNumber) params.IdentityNumber = this.searchIdentityNumber;
    if (this.searchLicenseNumber) params.LicenseNumber = this.searchLicenseNumber;
    if (this.selectedRole !== undefined) params.Role = this.selectedRole;
    if (this.selectedIsActive !== undefined) params.IsActive = this.selectedIsActive;
    if (this.selectedIsVerified !== undefined) params.IsVerified = this.selectedIsVerified;

    this.userService.searchUsers(params).subscribe({
      next: (response) => {
        this.users = response.users || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || 0;
        this.currentPage = response.pageNumber || 1;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        // Error is already handled by exception interceptor
      },
    });
  }

  /**
   * Search with filters
   */
  search() {
    this.currentPage = 1;
    this.loadUsers();
  }

  /**
   * Clear all filters
   */
  clearFilters() {
    this.searchEmail = '';
    this.selectedRole = undefined;
    this.selectedIsActive = undefined;
    this.orderBy = 'Id';
    this.isDescending = false;
    this.search();
  }

  /**
   * Change page
   */
  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }

  /**
   * Change page size
   */
  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadUsers();
  }

  /**
   * Sort by column
   */
  sortBy(column: string) {
    if (this.orderBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.orderBy = column;
      this.isDescending = false;
    }
    this.loadUsers();
  }

  /**
   * Get page numbers for pagination
   */
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPages - 1);

    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  /**
   * Format date for display
   */
  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
  }

  /**
   * Get role name
   */
  getRoleName(role: number): string {
    return this.roleNames[role as UserRole] || '-';
  }

  /**
   * Open add user modal
   */
  openAddUserModal() {
    const { close, contentRef } = this.modalService.open(UserFormComponent, {
      title: 'Yeni Kullanıcı Ekle',
      size: 'large',
    });

    // Listen for saved event
    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as UserFormComponent;

      const savedSub = formComponent.saved.subscribe((user: User) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadUsers();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  /**
   * Edit user
   */
  editUser(user: User) {
    const { close, contentRef } = this.modalService.open(UserFormComponent, {
      title: 'Kullanıcı Düzenle',
      size: 'large',
      inputs: { user },
    });

    // Listen for saved event
    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as UserFormComponent;

      const savedSub = formComponent.saved.subscribe((updatedUser: User) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadUsers();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  /**
   * View user detail
   */
  viewUserDetail(user: User) {
    this.modalService.open(UserDetailComponent, {
      title: `${user.firstName} ${user.lastName} - Detaylar`,
      size: 'large',
      inputs: { userId: user.id },
    });
  }

  /**
   * Delete user
   */
  deleteUser(user: User) {
    this.swalService.confirm('Bu kullanıcıyı silmek istediğinizden emin misiniz?').subscribe((result) => {
      if (result.isConfirmed) {
        this.userService.deleteUser(user.id).subscribe({
          next: () => {
            this.toastService.success('Kullanıcı başarıyla silindi');
            this.loadUsers();
          },
          error: () => {
            // Error is already handled by exception interceptor
          },
        });
      }
    });
  }
}

