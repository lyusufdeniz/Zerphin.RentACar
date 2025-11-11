import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  Output,
  EventEmitter,
  inject,
  ChangeDetectorRef,
  computed,
  effect,
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
import { LanguageService } from '../../services/language.service';
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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      firstName: this.languageService.translate('modals.user.firstName'),
      firstNamePlaceholder: this.languageService.translate('modals.user.firstNamePlaceholder'),
      lastName: this.languageService.translate('modals.user.lastName'),
      lastNamePlaceholder: this.languageService.translate('modals.user.lastNamePlaceholder'),
      email: this.languageService.translate('modals.user.email'),
      emailPlaceholder: this.languageService.translate('modals.user.emailPlaceholder'),
      phoneNumber: this.languageService.translate('modals.user.phoneNumber'),
      phoneNumberPlaceholder: this.languageService.translate('modals.user.phoneNumberPlaceholder'),
      password: this.languageService.translate('modals.user.password'),
      passwordPlaceholder: this.languageService.translate('modals.user.passwordPlaceholder'),
      role: this.languageService.translate('modals.user.role'),
      address: this.languageService.translate('modals.user.address'),
      addressPlaceholder: this.languageService.translate('modals.user.addressPlaceholder'),
      identityNumber: this.languageService.translate('modals.user.identityNumber'),
      identityNumberPlaceholder: this.languageService.translate('modals.user.identityNumberPlaceholder'),
      birthDate: this.languageService.translate('modals.user.birthDate'),
      customerInfo: this.languageService.translate('modals.user.customerInfo'),
      licenseNumber: this.languageService.translate('modals.user.licenseNumber'),
      licenseNumberPlaceholder: this.languageService.translate('modals.user.licenseNumberPlaceholder'),
      licenseExpiryDate: this.languageService.translate('modals.user.licenseExpiryDate'),
      licenseClass: this.languageService.translate('modals.user.licenseClass'),
      licenseClassPlaceholder: this.languageService.translate('modals.user.licenseClassPlaceholder'),
      emergencyContactName: this.languageService.translate('modals.user.emergencyContactName'),
      emergencyContactNamePlaceholder: this.languageService.translate('modals.user.emergencyContactNamePlaceholder'),
      emergencyContactPhone: this.languageService.translate('modals.user.emergencyContactPhone'),
      emergencyContactPhonePlaceholder: this.languageService.translate('modals.user.emergencyContactPhonePlaceholder'),
      creditScore: this.languageService.translate('modals.user.creditScore'),
      creditScorePlaceholder: this.languageService.translate('modals.user.creditScorePlaceholder'),
      isVerified: this.languageService.translate('modals.user.isVerified'),
      hasInsurance: this.languageService.translate('modals.user.hasInsurance'),
      insuranceCompany: this.languageService.translate('modals.user.insuranceCompany'),
      insuranceCompanyPlaceholder: this.languageService.translate('modals.user.insuranceCompanyPlaceholder'),
      insurancePolicyNumber: this.languageService.translate('modals.user.insurancePolicyNumber'),
      insurancePolicyNumberPlaceholder: this.languageService.translate('modals.user.insurancePolicyNumberPlaceholder'),
      specialNotes: this.languageService.translate('modals.user.specialNotes'),
      specialNotesPlaceholder: this.languageService.translate('modals.user.specialNotesPlaceholder'),
      cancel: this.languageService.translate('modals.common.cancel'),
      save: this.languageService.translate('modals.common.save'),
      update: this.languageService.translate('modals.common.update'),
      saving: this.languageService.translate('modals.common.saving'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    this.isEditMode = !!this.user;
    this.initForm();
  }

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

    this.userForm.get('role')?.valueChanges.subscribe((role) => {
      this.updateCustomerFieldsVisibility(role);
      this.cdr.markForCheck();
    });

    const currentRole = this.userForm.get('role')?.value;
    this.updateCustomerFieldsVisibility(currentRole);

    this.cdr.markForCheck();
  }

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

  formatDateForInput(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  validateMinimumAge(control: any): { [key: string]: any } | null {
    if (!control.value) {
      return null; 
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

  get f() {
    return this.userForm.controls;
  }

  hasError(field: string, errorType: string): boolean {
    const control = this.userForm.get(field);
    return !!(
      control &&
      control.hasError(errorType) &&
      (control.touched || control.dirty)
    );
  }

      getErrorMessage(field: string): string {
        const control = this.userForm.get(field);
        if (!control || !control.errors) return '';

        if (control.hasError('required')) {
          if (field === 'firstName') return this.languageService.translate('messages.errors.userForm.firstNameRequired');
          if (field === 'lastName') return this.languageService.translate('messages.errors.userForm.lastNameRequired');
          if (field === 'email') return this.languageService.translate('messages.errors.userForm.emailRequired');
          if (field === 'phoneNumber') return this.languageService.translate('messages.errors.userForm.phoneRequired');
          if (field === 'password') return this.languageService.translate('messages.errors.userForm.passwordRequired');
          return this.languageService.translate('messages.errors.validation.required');
        }
        if (control.hasError('email')) return this.languageService.translate('messages.errors.userForm.emailInvalid');
        if (control.hasError('minlength')) {
          if (field === 'password') {
            return this.languageService.translate('messages.errors.userForm.passwordMinLength');
          }
          return this.languageService.translateWithParams('messages.errors.validation.maxlength', { length: control.errors['minlength'].requiredLength.toString() });
        }
        if (control.hasError('maxlength')) {
          if (field === 'firstName') {
            return this.languageService.translate('messages.errors.userForm.firstNameMaxLength');
          }
          if (field === 'lastName') {
            return this.languageService.translate('messages.errors.userForm.lastNameMaxLength');
          }
          if (field === 'email') {
            return this.languageService.translate('messages.errors.userForm.emailMaxLength');
          }
          if (field === 'address') {
            return this.languageService.translate('messages.errors.userForm.addressMaxLength');
          }
          if (field === 'identityNumber') {
            return this.languageService.translate('messages.errors.userForm.identityNumberMaxLength');
          }
          return this.languageService.translateWithParams('messages.errors.validation.maxlength', { length: control.errors['maxlength'].requiredLength.toString() });
        }
        if (control.hasError('pattern')) {
          if (field === 'phoneNumber') {
            return this.languageService.translate('messages.errors.userForm.phoneInvalid');
          }
          if (field === 'password') {
            return this.languageService.translate('messages.errors.userForm.passwordPattern');
          }
          return this.languageService.translate('messages.errors.validation.invalidFormat');
        }
        if (control.hasError('minimumAge')) {
          return this.languageService.translate('messages.errors.userForm.minimumAge');
        }
        if (control.hasError('min'))
          return this.languageService.translateWithParams('messages.errors.validation.min', { min: control.errors['min'].min.toString() });
        if (control.hasError('max'))
          return this.languageService.translateWithParams('messages.errors.validation.max', { max: control.errors['max'].max.toString() });

        return this.languageService.translate('messages.errors.validation.invalid');
      }

  getRoleName(role: number): string {
    return this.roleNames[role as UserRole] || '-';
  }

  isCustomerRole(): boolean {
    const role = this.userForm.get('role')?.value;
    return role === UserRole.Customer;
  }

  onSubmit() {
    if (this.userForm.invalid) {
      Object.keys(this.userForm.controls).forEach((key) => {
        this.userForm.get(key)?.markAsTouched();
      });
      this.toastService.error(this.languageService.translate('messages.errors.validation.fillAllRequired'));
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.userForm.value;
    const isCustomer = formValue.role === UserRole.Customer;

    if (this.isEditMode && this.user) {
      this.toastService.error(this.languageService.translate('messages.success.user.updateNotAvailable'));
      this.isLoading = false;
      this.cdr.markForCheck();
      return;
    } else {
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
        isVerified: false,
        creditScore: 0,
        hasInsurance: false,
      };

      if (isCustomer) {
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

        this.userService.createUser(createUserCommand).subscribe({
          next: (user) => {
            this.toastService.success(this.languageService.translate('messages.success.user.createdWithCustomer'));
            this.saved.emit(user);
            this.isLoading = false;
            this.cdr.markForCheck();
          },
          error: () => {
            this.isLoading = false;
            this.cdr.markForCheck();
          },
        });
      } else {
        this.userService.createUser(createUserCommand).subscribe({
          next: (user) => {
            this.toastService.success(this.languageService.translate('messages.success.user.created'));
            this.saved.emit(user);
            this.isLoading = false;
            this.cdr.markForCheck();
          },
          error: () => {
            this.isLoading = false;
            this.cdr.markForCheck();
          },
        });
      }
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
