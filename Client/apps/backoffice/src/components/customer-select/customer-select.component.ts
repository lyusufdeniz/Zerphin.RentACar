import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  Output,
  EventEmitter,
  computed,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { User, UserSearchParams, UserRole } from '../../models/user';
import { Customer } from '../../models/customer';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-customer-select',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerSelectComponent implements OnInit {
  @Output() customerSelected = new EventEmitter<Customer>();

  private userService = inject(UserService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  customers: Customer[] = [];
  isLoading = false;
  searchQuery = '';
  currentPage = 1;
  pageSize = 20;
  totalPages = 0;
  totalCount = 0;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      searchPlaceholder: this.languageService.translate('messages.select.customer.searchPlaceholder'),
      searchButton: this.languageService.translate('messages.select.customer.searchButton'),
      clearButton: this.languageService.translate('messages.select.customer.clearButton'),
      loading: this.languageService.translate('messages.select.customer.loading'),
      notFound: this.languageService.translate('messages.select.customer.notFound'),
      licensePrefix: this.languageService.translate('messages.select.customer.licensePrefix'),
      previous: this.languageService.translate('messages.select.customer.previous'),
      next: this.languageService.translate('messages.select.customer.next'),
      pageInfo: this.languageService.translateWithParams('messages.select.customer.pageInfo', {
        currentPage: this.currentPage,
        totalPages: this.totalPages,
        totalCount: this.totalCount,
      }),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    this.loadCustomers();
  }

  loadCustomers() {
    this.isLoading = true;
    this.cdr.markForCheck();

    const params: UserSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      Role: UserRole.Customer, 
      OrderBy: 'Id',
      IsDescending: false,
    };

    if (this.searchQuery.trim()) {

      const query = this.searchQuery.trim();
      if (query.includes('@')) {
        params.Email = query;
      } else if (/^\d+$/.test(query)) {

        params.PhoneNumber = query;
      } else {

        params.FirstName = query;
      }
    }

    this.userService.searchUsers(params).subscribe({
      next: (response) => {

        this.customers = (response.users || []).map((user) => this.userToCustomer(user));
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || 0;
        this.currentPage = response.pageNumber || 1;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      },
    });
  }

  private userToCustomer(user: User): Customer {
    return {
      id: user.id,
      userId: user.id,
      userName: user.email,
      userEmail: user.email,
      userPhone: user.phoneNumber,
      userFirstName: user.firstName,
      userLastName: user.lastName,
      licenseNumber: user.licenseNumber,
      licenseExpiryDate: user.licenseExpiryDate,
      licenseClass: user.licenseClass,
      emergencyContactName: user.emergencyContactName,
      emergencyContactPhone: user.emergencyContactPhone,
      specialNotes: user.specialNotes,
      isVerified: user.isVerified || false,
      verificationDate: user.verificationDate,
      verificationDocument: user.verificationDocument,
      creditScore: user.creditScore || 0,
      hasInsurance: user.hasInsurance || false,
      insuranceCompany: user.insuranceCompany,
      insurancePolicyNumber: user.insurancePolicyNumber,
      createdAt: user.createdAt,
      updatedAt: user.updatedAt,
    };
  }

  search() {
    this.currentPage = 1;
    this.loadCustomers();
  }

  clearSearch() {
    this.searchQuery = '';
    this.currentPage = 1;
    this.loadCustomers();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadCustomers();
    }
  }

  selectCustomer(customer: Customer) {
    this.customerSelected.emit(customer);
  }

  getCustomerDisplayName(customer: Customer): string {
    if (customer.userFirstName && customer.userLastName) {
      return `${customer.userFirstName} ${customer.userLastName}`;
    }
    if (customer.userName) {
      return customer.userName;
    }
    if (customer.userEmail) {
      return customer.userEmail;
    }
    return customer.id;
  }

  getCustomerDetailsText(customer: Customer): string {
    const details: string[] = [];
    if (customer.userEmail) {
      details.push(customer.userEmail);
    }
    if (customer.userPhone) {
      details.push(customer.userPhone);
    }
    if (customer.licenseNumber) {
      details.push(`${this.languageService.translate('messages.select.customer.licensePrefix')} ${customer.licenseNumber}`);
    }
    return details.join(' • ');
  }
}
