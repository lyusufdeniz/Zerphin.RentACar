import {
  ChangeDetectionStrategy,
  Component,
  ChangeDetectorRef,
  inject,
  OnInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { ValidationService } from '../../services/validation.service';
import { LoginRequest } from '../../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);
  private validationService = inject(ValidationService);
  private fb = inject(FormBuilder);

  loginForm!: FormGroup;
  showPassword: boolean = false;
  isLoading: boolean = false;

  ngOnInit() {
    // Initialize form with validators using validation service
    this.loginForm = this.fb.group({
      email: [
        '',
        [
          Validators.required,
          this.validationService.emailValidator(),
          Validators.maxLength(255),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(100),
        ],
      ],
      rememberMe: [false],
    });

    // Check if user is already logged in
    if (this.authService.isAuthenticated()) {
      const returnUrl =
        this.route.snapshot.queryParams['returnUrl'] || 'dashboard';
      this.router.navigate([returnUrl]);
    }
  }

  /**
   * Get form control
   */
  get f() {
    return this.loginForm.controls;
  }

  /**
   * Check if field has error (using validation service)
   */
  hasError(field: string, errorType: string): boolean {
    return this.validationService.hasError(
      this.loginForm.get(field),
      errorType
    );
  }

  /**
   * Get error message for field (using validation service)
   */
  getErrorMessage(field: string): string {
    const control = this.loginForm.get(field);
    const fieldLabel = this.validationService.getFieldLabel(field);
    return this.validationService.getErrorMessage(control, fieldLabel);
  }

  onLogin() {
    // Mark all fields as touched to show validation errors
    if (this.loginForm.invalid) {
      this.validationService.markFormGroupTouched(this.loginForm);

      // Show first error message
      const firstError = this.validationService.getFirstFormError(
        this.loginForm
      );
      if (firstError) {
        this.toastService.error(firstError);
      }

      this.cdr.markForCheck();
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    // Prepare login request
    const loginRequest: LoginRequest = {
      email: this.loginForm.value.email.trim(),
      password: this.loginForm.value.password,
    };

    // Call API
    this.authService.login(loginRequest).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.cdr.markForCheck();

        // Success
        this.toastService.success('Giriş başarılı! Yönlendiriliyorsunuz...');

        // Get return URL from query params or default to dashboard
        const returnUrl =
          this.route.snapshot.queryParams['returnUrl'] || 'dashboard';

        // Navigate after a short delay to show success message
        setTimeout(() => {
          this.router.navigate([returnUrl], { replaceUrl: true });
        }, 500);
      },
      error: (error: any) => {
        this.isLoading = false;
        this.cdr.markForCheck();

        // Handle ServiceResult error format
        if (error.errorMessage && Array.isArray(error.errorMessage)) {
          // ServiceResult format - multiple error messages
          this.toastService.showErrorMessages(error.errorMessage);
        } else if (error.error?.errorMessage) {
          // Nested errorMessage in error object
          const errorMessages = Array.isArray(error.error.errorMessage)
            ? error.error.errorMessage
            : [error.error.errorMessage];
          this.toastService.showErrorMessages(errorMessages);
        } else if (error.message) {
          // Standard error message
          this.toastService.error(error.message);
        } else {
          // Unknown error
          this.toastService.error('Giriş başarısız. Lütfen tekrar deneyin.');
        }

        // Clear password on error (security best practice)
        this.loginForm.patchValue({ password: '' });

        this.cdr.markForCheck();
      },
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
}

