import {
  ChangeDetectionStrategy,
  Component,
  ChangeDetectorRef,
  inject,
  OnInit,
  OnDestroy,
  computed,
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
import { ThemeService } from '../../services/theme.service';
import { LanguageService, Language } from '../../services/language.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);
  private validationService = inject(ValidationService);
  private fb = inject(FormBuilder);
  private themeService = inject(ThemeService);
  private languageService = inject(LanguageService);

  loginForm!: FormGroup;
  showPassword: boolean = false;
  isLoading: boolean = false;
  currentLanguage = this.languageService.currentLanguage;
  languages = this.languageService.getAvailableLanguages();
  private languageSubscription?: Subscription;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      welcome: this.languageService.translate('login.welcome'),
      description: this.languageService.translate('login.description'),
      secureAccess: this.languageService.translate('login.features.secureAccess'),
      easyManagement: this.languageService.translate('login.features.easyManagement'),
      detailedReporting: this.languageService.translate('login.features.detailedReporting'),
      title: this.languageService.translate('login.title'),
      email: this.languageService.translate('login.email'),
      emailPlaceholder: this.languageService.translate('login.emailPlaceholder'),
      emailRequired: this.languageService.translate('login.emailRequired'),
      emailInvalid: this.languageService.translate('login.emailInvalid'),
      password: this.languageService.translate('login.password'),
      passwordPlaceholder: this.languageService.translate('login.passwordPlaceholder'),
      passwordRequired: this.languageService.translate('login.passwordRequired'),
      rememberMe: this.languageService.translate('login.rememberMe'),
      loginButton: this.languageService.translate('login.loginButton'),
      loggingIn: this.languageService.translate('login.loggingIn'),
      loginSuccess: this.languageService.translate('login.loginSuccess'),
      loginFailed: this.languageService.translate('login.loginFailed'),
    };
  });

  ngOnInit() {
    this.loginForm = this.fb.group({
      email: [
        '',
        [
          Validators.required,
          this.validationService.emailValidator(),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
        ],
      ],
      rememberMe: [false],
    });

    if (this.authService.isAuthenticated()) {
      const returnUrl =
        this.route.snapshot.queryParams['returnUrl'] || 'dashboard';
      this.router.navigate([returnUrl]);
    }

    this.languageSubscription = toObservable(this.languageService.currentLanguage).subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  t(key: string): string {
    return this.languageService.translate(key);
  }

  changeLanguage(language: Language) {
    this.languageService.setLanguage(language);
    this.cdr.markForCheck();
  }

  get f() {
    return this.loginForm.controls;
  }

  hasError(field: string, errorType: string): boolean {
    return this.validationService.hasError(
      this.loginForm.get(field),
      errorType
    );
  }

  getErrorMessage(field: string): string {
    const control = this.loginForm.get(field);
    const fieldLabel = this.validationService.getFieldLabel(field);
    return this.validationService.getErrorMessage(control, fieldLabel);
  }

  onLogin() {
    if (this.loginForm.invalid) {
      this.validationService.markFormGroupTouched(this.loginForm);

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

    const loginRequest: LoginRequest = {
      email: this.loginForm.value.email.trim(),
      password: this.loginForm.value.password,
    };

    const rememberMe = this.loginForm.value.rememberMe || false;

    this.authService.login(loginRequest, rememberMe).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.cdr.markForCheck();

        this.toastService.success(this.t('login.loginSuccess'));

        const returnUrl =
          this.route.snapshot.queryParams['returnUrl'] || 'dashboard';

        setTimeout(() => {
          this.router.navigate([returnUrl], { replaceUrl: true });
        }, 500);
      },
      error: (error: any) => {
        this.isLoading = false;
        this.cdr.markForCheck();

        if (error.errorMessage && Array.isArray(error.errorMessage)) {
          this.toastService.showErrorMessages(error.errorMessage);
        } else if (error.error?.errorMessage) {
          const errorMessages = Array.isArray(error.error.errorMessage)
            ? error.error.errorMessage
            : [error.error.errorMessage];
          this.toastService.showErrorMessages(errorMessages);
        } else if (error.message) {
          this.toastService.error(error.message);
        } else {
          this.toastService.error(this.t('login.loginFailed'));
        }

        this.loginForm.patchValue({ password: '' });

        this.cdr.markForCheck();
      },
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
}
