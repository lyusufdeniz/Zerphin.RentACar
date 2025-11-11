import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  computed,
  OnDestroy,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { FavoritePagesService } from '../../services/favorite-pages.service';
import { StorageService } from '../../services/storage.service';
import { ToastService } from '../../services/toast.service';
import { SwalService } from '../../services/swal.service';
import { ChangePasswordCommand } from '../../models/user';
import { ThemeService, Theme } from '../../services/theme.service';
import { LanguageService, Language } from '../../services/language.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FavoriteButtonComponent],
  templateUrl: './settings.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsComponent implements OnInit, OnDestroy {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private favoritePagesService = inject(FavoritePagesService);
  private storageService = inject(StorageService);
  private toastService = inject(ToastService);
  private swalService = inject(SwalService);
  private themeService = inject(ThemeService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);

  passwordForm!: FormGroup;
  isLoading = false;
  showPassword = false;
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

  activeTab: 'password' | 'appearance' | 'language' | 'favorites' = 'password';

  currentTheme = this.themeService.currentTheme;
  currentLanguage = this.languageService.currentLanguage;
  languages = this.languageService.getAvailableLanguages();

  translations = computed(() => {

    const currentLang = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('settings.title'),
      tabs: {
        password: this.languageService.translate('settings.tabs.password'),
        appearance: this.languageService.translate('settings.tabs.appearance'),
        language: this.languageService.translate('settings.tabs.language'),
        favorites: this.languageService.translate('settings.tabs.favorites'),
      },
      password: {
        change: this.languageService.translate('settings.password.change'),
        description: this.languageService.translate('settings.password.description'),
        current: this.languageService.translate('settings.password.current'),
        currentPlaceholder: this.languageService.translate('settings.password.currentPlaceholder'),
        currentRequired: this.languageService.translate('settings.password.currentRequired'),
        new: this.languageService.translate('settings.password.new'),
        newPlaceholder: this.languageService.translate('settings.password.newPlaceholder'),
        newRequired: this.languageService.translate('settings.password.newRequired'),
        newMinLength: this.languageService.translate('settings.password.newMinLength'),
        newPattern: this.languageService.translate('settings.password.newPattern'),
        confirm: this.languageService.translate('settings.password.confirm'),
        confirmPlaceholder: this.languageService.translate('settings.password.confirmPlaceholder'),
        confirmRequired: this.languageService.translate('settings.password.confirmRequired'),
        confirmMismatch: this.languageService.translate('settings.password.confirmMismatch'),
        changeButton: this.languageService.translate('settings.password.changeButton'),
        changing: this.languageService.translate('settings.password.changing'),
      },
      theme: {
        title: this.languageService.translate('settings.theme.title'),
        description: this.languageService.translate('settings.theme.description'),
        light: this.languageService.translate('settings.theme.light'),
        dark: this.languageService.translate('settings.theme.dark'),
      },
      language: {
        title: this.languageService.translate('settings.language.title'),
        description: this.languageService.translate('settings.language.description'),
      },
      favorites: {
        title: this.languageService.translate('settings.favorites.title'),
        description: this.languageService.translate('settings.favorites.description'),
        clear: this.languageService.translate('settings.favorites.clear'),
      },
    };
  });

  themes = computed(() => {

    const currentLang = this.languageService.currentLanguage();
    return [
      { value: 'light' as Theme, label: this.languageService.translate('settings.theme.light') },
      { value: 'dark' as Theme, label: this.languageService.translate('settings.theme.dark') },
    ];
  });

  constructor() {

    effect(() => {

      const _ = this.languageService.currentLanguage();

      this.cdr.markForCheck();
    });
  }

  setActiveTab(tab: 'password' | 'appearance' | 'language' | 'favorites') {
    this.activeTab = tab;
    this.cdr.markForCheck();
  }

  ngOnInit() {
    this.initPasswordForm();

    this.cdr.detectChanges();
  }

  ngOnDestroy() {

  }

  initPasswordForm() {
    this.passwordForm = this.fb.group(
      {
        currentPassword: ['', [Validators.required]],
        newPassword: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            Validators.pattern(
              /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/
            ),
          ],
        ],
        confirmPassword: ['', [Validators.required]],
      },
      {
        validators: this.passwordMatchValidator,
      }
    );
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    if (newPassword && confirmPassword && newPassword !== confirmPassword) {
      form.get('confirmPassword')?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  changePassword() {
    if (this.passwordForm.invalid) {
      this.markFormGroupTouched(this.passwordForm);
      return;
    }

    const user = this.authService.user;
    if (!user) {
      this.toastService.error(this.languageService.translate('common.error'));
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const command: ChangePasswordCommand = {
      userId: user.id,
      currentPassword: this.passwordForm.value.currentPassword,
      newPassword: this.passwordForm.value.newPassword,
      confirmPassword: this.passwordForm.value.confirmPassword,
    };

    this.userService.changePassword(command).subscribe({
      next: () => {
        this.toastService.success(this.languageService.translate('settings.password.success'));
        this.passwordForm.reset();
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      },
    });
  }

  changeTheme(theme: Theme) {
    this.themeService.setTheme(theme);
    this.toastService.success(this.languageService.translate('settings.theme.changed'));
  }

  changeLanguage(language: Language) {
    this.languageService.setLanguage(language);
    this.toastService.success(this.languageService.translate('settings.language.changed'));
    this.cdr.markForCheck();
  }

  clearFavorites() {
    this.swalService
      .confirm(
        this.languageService.translate('settings.favorites.clear'),
        this.languageService.translate('settings.favorites.clearConfirm'),
        this.languageService.translate('common.reset'),
        this.languageService.translate('common.cancel')
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.favoritePagesService.clearFavorites();
          this.toastService.success(this.languageService.translate('settings.favorites.cleared'));
        }
      });
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  get f() {
    return this.passwordForm.controls;
  }
}
