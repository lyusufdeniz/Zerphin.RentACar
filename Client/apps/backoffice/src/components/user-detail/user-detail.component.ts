import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  ChangeDetectorRef,
  inject,
  computed,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  User,
  UserRole,
  UserRoleNames,
} from '../../models/user';
import { UserService } from '../../services/user.service';
import { ToastService } from '../../services/toast.service';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-detail.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserDetailComponent implements OnInit {
  @Input() userId!: string;

  private userService = inject(UserService);
  private toastService = inject(ToastService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  user: User | null = null;
  isLoading = true;

  roleNames = UserRoleNames;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      loading: this.languageService.translate('messages.details.user.loading'),
      notFound: this.languageService.translate('messages.details.notFound.user'),
      personalInfo: this.languageService.translate('messages.details.user.personalInfo'),
      firstName: this.languageService.translate('messages.details.user.firstName'),
      lastName: this.languageService.translate('messages.details.user.lastName'),
      email: this.languageService.translate('messages.details.user.email'),
      phone: this.languageService.translate('messages.details.user.phone'),
      identityNumber: this.languageService.translate('messages.details.user.identityNumber'),
      birthDate: this.languageService.translate('messages.details.user.birthDate'),
      address: this.languageService.translate('messages.details.user.address'),
      accountInfo: this.languageService.translate('messages.details.user.accountInfo'),
      role: this.languageService.translate('messages.details.user.role'),
      status: this.languageService.translate('messages.details.user.status'),
      createdAt: this.languageService.translate('messages.details.user.createdAt'),
      updatedAt: this.languageService.translate('messages.details.user.updatedAt'),
      customerInfo: this.languageService.translate('messages.details.user.customerInfo'),
      licenseNumber: this.languageService.translate('messages.details.user.licenseNumber'),
      licenseExpiryDate: this.languageService.translate('messages.details.user.licenseExpiryDate'),
      licenseClass: this.languageService.translate('messages.details.user.licenseClass'),
      emergencyContactName: this.languageService.translate('messages.details.user.emergencyContactName'),
      emergencyContactPhone: this.languageService.translate('messages.details.user.emergencyContactPhone'),
      isVerified: this.languageService.translate('messages.details.user.isVerified'),
      verificationDate: this.languageService.translate('messages.details.user.verificationDate'),
      creditScore: this.languageService.translate('messages.details.user.creditScore'),
      hasInsurance: this.languageService.translate('messages.details.user.hasInsurance'),
      insuranceCompany: this.languageService.translate('messages.details.user.insuranceCompany'),
      insurancePolicyNumber: this.languageService.translate('messages.details.user.insurancePolicyNumber'),
      specialNotes: this.languageService.translate('messages.details.user.specialNotes'),
      active: this.languageService.translate('common.active'),
      inactive: this.languageService.translate('common.inactive'),
      yes: this.languageService.translate('common.yes'),
      no: this.languageService.translate('common.no'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    if (this.userId) {
      this.loadUser();
    }
  }

  loadUser() {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.userService.getUserById(this.userId).subscribe({
      next: (user) => {
        this.user = user;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      },
    });
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return date.toLocaleDateString(locale, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  formatDateTime(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return date.toLocaleString(locale, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  getRoleName(role: number): string {
    return this.roleNames[role as UserRole] || '-';
  }
}
