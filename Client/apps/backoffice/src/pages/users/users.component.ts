import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  computed,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { UserService } from '../../services/user.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
import { LanguageService } from '../../services/language.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { UserDetailComponent } from '../../components/user-detail/user-detail.component';
import {
  User,
  UserRole,
  UserRoleNames,
  UserSearchParams,
  PaginatedUserResponse,
} from '../../models/user';

export const UserRoleEnum = UserRole;

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, FavoriteButtonComponent],
  templateUrl: './users.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UsersComponent implements OnInit, OnDestroy {
  private userService = inject(UserService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);
  private languageSubscription?: Subscription;

  translations = computed(() => {
    const currentLang = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('pages.users.title'),
      name: this.languageService.translate('pages.users.name'),
      role: this.languageService.translate('pages.users.role'),
      isActive: this.languageService.translate('pages.users.isActive'),
      isVerified: this.languageService.translate('pages.users.isVerified'),
      searchName: this.languageService.translate('pages.users.searchName'),
      all: this.languageService.translate('pages.users.all'),
      loading: this.languageService.translate('pages.users.loading'),
      noData: this.languageService.translate('pages.users.noData'),
      add: this.languageService.translate('pages.users.add'),
      edit: this.languageService.translate('pages.users.edit'),
      delete: this.languageService.translate('pages.users.delete'),
      view: this.languageService.translate('pages.users.view'),
      search: this.languageService.translate('common.search'),
      clear: this.languageService.translate('common.clear'),
      active: this.languageService.translate('common.active'),
      inactive: this.languageService.translate('common.inactive'),
      yes: this.languageService.translate('common.yes'),
      no: this.languageService.translate('common.no'),
      table: {
        firstName: this.languageService.translate('pages.users.table.firstName'),
        lastName: this.languageService.translate('pages.users.table.lastName'),
        email: this.languageService.translate('pages.users.table.email'),
        phone: this.languageService.translate('pages.users.table.phone'),
        role: this.languageService.translate('pages.users.table.role'),
        status: this.languageService.translate('pages.users.table.status'),
        actions: this.languageService.translate('pages.users.table.actions'),
      },
      status: {
        active: this.languageService.translate('pages.users.status.active'),
        inactive: this.languageService.translate('pages.users.status.inactive'),
      },
      pagination: {
        previous: this.languageService.translate('pages.users.pagination.previous'),
        next: this.languageService.translate('pages.users.pagination.next'),
      },
    };
  });

  public languageServicePublic = this.languageService; 

  users: User[] = [];
  totalCount = 0;
  isLoading = false;

  currentPage = 1;
  pageSize = 100;
  totalPages = 0;

  searchCustomerQuery = ''; 
  selectedRole: UserRole | null = null;
  selectedIsActive: boolean | null = null;
  selectedIsVerified: boolean | null = null;

  roles = Object.values(UserRole).filter(
    (v) => typeof v === 'number'
  ) as UserRole[];
  roleNames = UserRoleNames;
  UserRole = UserRole; 

  orderBy = 'Id';
  isDescending = false;

  ngOnInit() {
    this.loadUsers();

    this.languageSubscription = toObservable(this.languageService.currentLanguage).subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  loadUsers() {
    this.isLoading = true;
    this.cdr.detectChanges();

    const params: UserSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchCustomerQuery.trim()) {
      const query = this.searchCustomerQuery.trim();

      const parts = query.split(/\s+/);
      if (parts.length >= 2) {
        params.FirstName = parts[0];
        params.LastName = parts.slice(1).join(' ');
      } else {

        params.FirstName = query;
      }
    }

    if (this.selectedRole !== null) {
      params.Role = this.selectedRole;
    }
    if (this.selectedIsActive !== null) {
      params.IsActive = this.selectedIsActive;
    }
    if (this.selectedIsVerified !== null) {
      params.IsVerified = this.selectedIsVerified;
    }

    this.userService.searchUsers(params).subscribe({
      next: (response) => {
        this.users = response.users || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || 0;
        this.currentPage = response.pageNumber || 1;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  search() {
    this.currentPage = 1;
    this.loadUsers();
  }

  clearFilters() {
    this.searchCustomerQuery = '';
    this.selectedRole = null;
    this.selectedIsActive = null;
    this.selectedIsVerified = null;
    this.orderBy = 'Id';
    this.isDescending = false;
    this.search();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }

  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadUsers();
  }

  sortBy(column: string) {
    if (this.orderBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.orderBy = column;
      this.isDescending = false;
    }
    this.loadUsers();
  }

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

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
  }

  getRoleName(role: number): string {
    return this.roleNames[role as UserRole] || '-';
  }

  openAddUserModal() {
    const { close, contentRef } = this.modalService.open(UserFormComponent, {
      title: this.languageService.translate('messages.modal.user.add'),
      size: 'large',
    });

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

  editUser(user: User) {
    const { close, contentRef } = this.modalService.open(UserFormComponent, {
      title: this.languageService.translate('messages.modal.user.edit'),
      size: 'large',
      inputs: { user },
    });

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

  viewUserDetail(user: User) {
    this.modalService.open(UserDetailComponent, {
      title: this.languageService.translateWithParams('messages.modal.user.details', {
        firstName: user.firstName,
        lastName: user.lastName,
      }),
      size: 'large',
      inputs: { userId: user.id },
    });
  }

  deleteUser(user: User) {
    this.swalService
      .confirm(
        this.languageService.translate('messages.deleteConfirm.user.title'),
        this.languageService.translate('messages.deleteConfirm.user.message'),
        this.languageService.translate('messages.deleteConfirm.user.confirm'),
        this.languageService.translate('messages.deleteConfirm.user.cancel')
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.userService.deleteUser(user.id).subscribe({
            next: () => {
              this.toastService.success(this.languageService.translate('messages.success.user.deleted'));
              this.loadUsers();
            },
            error: () => {
            },
          });
        }
      });
  }
}
