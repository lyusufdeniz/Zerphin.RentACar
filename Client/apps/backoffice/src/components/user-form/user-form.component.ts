import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  Output,
  EventEmitter,
  inject,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  User,
  UserRole,
  UserRoleNames,
  CreateUserCommand,
} from '../../models/user';
import { UserService } from '../../services/user.service';
import { CustomerService } from '../../services/customer.service';
import { ToastService } from '../../services/toast.service';
import {
  CreateCustomerCommand,
} from '../../models/customer';
import { forkJoin } from 'rxjs';
import { DatepickerComponent } from '../datepicker/datepicker.component';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatepickerComponent],
  templateUrl: './user-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserFormComponent implements OnInit {
  @Input() user?: User;
  @Output() saved = new EventEmitter<User>();
  @Output() cancelled = new EventEmitter<void>();

  userForm!: FormGroup;
  isEditMode = false;
  isLoading = false;

  roles = Object.values(UserRole).filter(
    (v) => typeof v === 'number'
  ) as UserRole[];
  roleNames = UserRoleNames;

  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private customerService = inject(CustomerService);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.isEditMode = !!this.user;
    this.initForm();
  }

  /**
   * Initialize form
   */
  initForm() {
    const user = this.user;

    this.userForm = this.fb.group({
      firstName: [
        user?.firstName || '',
        [Validators.required, Validators.maxLength(50)],
      ],
      lastName: [
        user?.lastName || '',
        [Validators.required, Validators.maxLength(50)],
      ],
      email: [
        user?.email || '',
        [Validators.required, Validators.email, Validators.maxLength(100)],
      ],
      phoneNumber: [
        user?.phoneNumber || '',
        [
          Validators.required,
          Validators.pattern(/^\+?[1-9]\d{1,14}$/),
        ],
      ],
      password: [
        '',
        this.isEditMode
          ? []
          : [
              Validators.required,
              Validators.minLength(8),
              Validators.pattern(
                /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/
              ),
            ],
      ],
      role: [
        user?.role || UserRole.Customer,
        Validators.required,
      ],
      address: [
        user?.address || '',
        [Validators.maxLength(200)],
      ],
      identityNumber: [
        user?.identityNumber || '',
        [Validators.maxLength(20)],
      ],
      birthDate: [
        user?.birthDate
          ? this.formatDateForInput(user.birthDate)
          : null,
        [this.validateMinimumAge.bind(this)],
      ],
      // Customer fields
      licenseNumber: ['', [Validators.maxLength(50)]],
      licenseExpiryDate: [null],
      licenseClass: ['', [Validators.maxLength(20)]],
      emergencyContactName: ['', [Validators.maxLength(100)]],
      emergencyContactPhone: ['', [Validators.maxLength(20)]],
      specialNotes: ['', [Validators.maxLength(1000)]],
      isVerified: [false],
      verificationDate: [null],
      verificationDocument: ['', [Validators.maxLength(500)]],
      creditScore: [0, [Validators.min(0), Validators.max(1000)]],
      hasInsurance: [false],
      insuranceCompany: ['', [Validators.maxLength(100)]],
      insurancePolicyNumber: ['', [Validators.maxLength(100)]],
    });

    // Watch role changes to show/hide customer fields
    this.userForm.get('role')?.valueChanges.subscribe((role) => {
      this.updateCustomerFieldsVisibility(role);
      this.cdr.markForCheck();
    });

    // Initialize customer fields visibility
    const currentRole = this.userForm.get('role')?.value;
    this.updateCustomerFieldsVisibility(currentRole);

    this.cdr.markForCheck();
  }

  /**
   * Update customer fields visibility and validators based on role
   */
  updateCustomerFieldsVisibility(role: UserRole) {
    const isCustomer = role === UserRole.Customer;
    
    const customerFields = [
      'licenseNumber',
      'licenseExpiryDate',
      'licenseClass',
      'emergencyContactName',
      'emergencyContactPhone',
      'specialNotes',
      'isVerified',
      'verificationDate',
      'verificationDocument',
      'creditScore',
      'hasInsurance',
      'insuranceCompany',
      'insurancePolicyNumber',
    ];

    customerFields.forEach((field) => {
      const control = this.userForm.get(field);
      if (control) {
        if (isCustomer && field === 'licenseExpiryDate') {
          control.setValidators([Validators.required]);
        } else if (isCustomer && field === 'creditScore') {
          control.setValidators([Validators.required, Validators.min(0), Validators.max(1000)]);
        } else {
          control.clearValidators();
        }
        control.updateValueAndValidity();
      }
    });
  }

  /**
   * Format date for input (YYYY-MM-DD)
   */
  formatDateForInput(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  /**
   * Validate minimum age (18 years old)
   */
  validateMinimumAge(control: any): { [key: string]: any } | null {
    if (!control.value) {
      return null; // If no value, let required validator handle it
    }

    const selectedDate = new Date(control.value);
    const today = new Date();
    const minDate = new Date(today);
    minDate.setFullYear(today.getFullYear() - 18);

    if (selectedDate > minDate) {
      return { minimumAge: true };
    }

    return null;
  }

  /**
   * Get form control
   */
  get f() {
    return this.userForm.controls;
  }

  /**
   * Check if field has error
   */
  hasError(field: string, errorType: string): boolean {
    const control = this.userForm.get(field);
    return !!(
      control &&
      control.hasError(errorType) &&
      (control.touched || control.dirty)
    );
  }

      /**
       * Get error message
       */
      getErrorMessage(field: string): string {
        const control = this.userForm.get(field);
        if (!control || !control.errors) return '';

        if (control.hasError('required')) {
          if (field === 'firstName') return 'First name is required';
          if (field === 'lastName') return 'Last name is required';
          if (field === 'email') return 'Email is required';
          if (field === 'phoneNumber') return 'Phone number is required';
          if (field === 'password') return 'Password is required';
          return 'Bu alan zorunludur';
        }
        if (control.hasError('email')) return 'Invalid email format';
        if (control.hasError('minlength')) {
          if (field === 'password') {
            return 'Password must be at least 8 characters';
          }
          return `Minimum ${control.errors['minlength'].requiredLength} karakter olmalıdır`;
        }
        if (control.hasError('maxlength')) {
          if (field === 'firstName') {
            return 'First name cannot exceed 50 characters';
          }
          if (field === 'lastName') {
            return 'Last name cannot exceed 50 characters';
          }
          if (field === 'email') {
            return 'Email cannot exceed 100 characters';
          }
          if (field === 'address') {
            return 'Address cannot exceed 200 characters';
          }
          if (field === 'identityNumber') {
            return 'Identity number cannot exceed 20 characters';
          }
          return `Maksimum ${control.errors['maxlength'].requiredLength} karakter olmalıdır`;
        }
        if (control.hasError('pattern')) {
          if (field === 'phoneNumber') {
            return 'Invalid phone number format';
          }
          if (field === 'password') {
            return 'Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character';
          }
          return 'Geçersiz format';
        }
        if (control.hasError('minimumAge')) {
          return 'User must be at least 18 years old';
        }
        if (control.hasError('min'))
          return `Minimum değer ${control.errors['min'].min} olmalıdır`;
        if (control.hasError('max'))
          return `Maksimum değer ${control.errors['max'].max} olmalıdır`;

        return 'Geçersiz değer';
      }

  /**
   * Get role name
   */
  getRoleName(role: number): string {
    return this.roleNames[role as UserRole] || '-';
  }

  /**
   * Check if customer fields should be shown
   */
  isCustomerRole(): boolean {
    const role = this.userForm.get('role')?.value;
    return role === UserRole.Customer;
  }

  /**
   * Submit form
   */
  onSubmit() {
    if (this.userForm.invalid) {
      Object.keys(this.userForm.controls).forEach((key) => {
        this.userForm.get(key)?.markAsTouched();
      });
      this.toastService.error('Lütfen tüm zorunlu alanları doldurun');
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.userForm.value;
    const isCustomer = formValue.role === UserRole.Customer;

    if (this.isEditMode && this.user) {
      // Update existing user (edit mode - not implemented in API)
      this.toastService.error('Kullanıcı güncelleme özelliği henüz kullanılamıyor');
      this.isLoading = false;
      this.cdr.markForCheck();
      return;
    } else {
      // Create new user
      const createUserCommand: CreateUserCommand = {
        firstName: formValue.firstName?.trim() || undefined,
        lastName: formValue.lastName?.trim() || undefined,
        email: formValue.email?.trim() || undefined,
        phoneNumber: formValue.phoneNumber?.trim() || undefined,
        password: formValue.password?.trim() || undefined,
        role: Number(formValue.role),
        address: formValue.address?.trim() || undefined,
        identityNumber: formValue.identityNumber?.trim() || undefined,
        birthDate: formValue.birthDate
          ? new Date(formValue.birthDate).toISOString()
          : undefined,
        // Required fields with default values
        isVerified: false,
        creditScore: 0,
        hasInsurance: false,
      };

      if (isCustomer) {
        // Add customer-specific fields to CreateUserCommand
        createUserCommand.licenseNumber = formValue.licenseNumber?.trim() || undefined;
        createUserCommand.licenseExpiryDate = formValue.licenseExpiryDate
          ? new Date(formValue.licenseExpiryDate).toISOString()
          : undefined;
        createUserCommand.licenseClass = formValue.licenseClass?.trim() || undefined;
        createUserCommand.emergencyContactName = formValue.emergencyContactName?.trim() || undefined;
        createUserCommand.emergencyContactPhone = formValue.emergencyContactPhone?.trim() || undefined;
        createUserCommand.specialNotes = formValue.specialNotes?.trim() || undefined;
        createUserCommand.isVerified = Boolean(formValue.isVerified);
        createUserCommand.verificationDate = formValue.verificationDate
          ? new Date(formValue.verificationDate).toISOString()
          : undefined;
        createUserCommand.verificationDocument = formValue.verificationDocument?.trim() || undefined;
        createUserCommand.creditScore = Number(formValue.creditScore) || 0;
        createUserCommand.hasInsurance = Boolean(formValue.hasInsurance);
        createUserCommand.insuranceCompany = formValue.insuranceCompany?.trim() || undefined;
        createUserCommand.insurancePolicyNumber = formValue.insurancePolicyNumber?.trim() || undefined;

        // Create user with customer data (backend will create customer automatically)
        this.userService.createUser(createUserCommand).subscribe({
          next: (user) => {
            this.toastService.success('Kullanıcı ve müşteri başarıyla oluşturuldu');
            this.saved.emit(user);
            this.isLoading = false;
            this.cdr.markForCheck();
          },
          error: () => {
            this.isLoading = false;
            this.cdr.markForCheck();
            // Error is already handled by exception interceptor
          },
        });
      } else {
        // Create regular user (non-customer)
        this.userService.createUser(createUserCommand).subscribe({
          next: (user) => {
            this.toastService.success('Kullanıcı başarıyla oluşturuldu');
            this.saved.emit(user);
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
    }
  }

  /**
   * Cancel form
   */
  onCancel() {
    this.cancelled.emit();
  }
}

