import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  Output,
  EventEmitter,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { User, UserSearchParams, UserRole } from '../../models/user';
import { Customer } from '../../models/customer';

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
  private cdr = inject(ChangeDetectorRef);

  customers: Customer[] = [];
  isLoading = false;
  searchQuery = '';
  currentPage = 1;
  pageSize = 20;
  totalPages = 0;
  totalCount = 0;

  ngOnInit() {
    this.loadCustomers();
  }

  /**
   * Load customers (users with Customer role) with search
   */
  loadCustomers() {
    this.isLoading = true;
    this.cdr.markForCheck();

    const params: UserSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      Role: UserRole.Customer, // Only Customer role
      OrderBy: 'Id',
      IsDescending: false,
    };

    if (this.searchQuery.trim()) {
      // Search by name, email, phone, or license number
      const query = this.searchQuery.trim();
      if (query.includes('@')) {
        params.Email = query;
      } else if (/^\d+$/.test(query)) {
        // If only numbers, try phone number first
        params.PhoneNumber = query;
      } else {
        // Try first name, last name, or license number
        params.FirstName = query;
      }
    }

    this.userService.searchUsers(params).subscribe({
      next: (response) => {
        // Convert Users to Customer format
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
        // Error is already handled by exception interceptor
      },
    });
  }

  /**
   * Convert User to Customer format
   */
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

  /**
   * Search customers
   */
  search() {
    this.currentPage = 1;
    this.loadCustomers();
  }

  /**
   * Clear search
   */
  clearSearch() {
    this.searchQuery = '';
    this.currentPage = 1;
    this.loadCustomers();
  }

  /**
   * Change page
   */
  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadCustomers();
    }
  }

  /**
   * Select customer
   */
  selectCustomer(customer: Customer) {
    this.customerSelected.emit(customer);
  }

  /**
   * Get customer display name
   */
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

  /**
   * Get customer details text
   */
  getCustomerDetailsText(customer: Customer): string {
    const details: string[] = [];
    if (customer.userEmail) {
      details.push(customer.userEmail);
    }
    if (customer.userPhone) {
      details.push(customer.userPhone);
    }
    if (customer.licenseNumber) {
      details.push(`Ehliyet: ${customer.licenseNumber}`);
    }
    return details.join(' • ');
  }
}

