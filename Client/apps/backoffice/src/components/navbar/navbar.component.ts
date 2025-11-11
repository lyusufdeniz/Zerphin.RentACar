import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  OnDestroy,
  Output,
  EventEmitter,
  inject,
  computed,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { LanguageService } from '../../services/language.service';
import { UserRoleNames } from '../../models/user/user-role.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavbarComponent implements OnInit, OnDestroy {
  @Output() menuToggle = new EventEmitter<void>();
  private authService = inject(AuthService);
  private router = inject(Router);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  currentTime: string = '';
  currentDate: string = '';
  private intervalSubscription?: Subscription;

  translations = computed(() => {

    const currentLang = this.languageService.currentLanguage();
    return {
      dashboard: this.languageService.translate('dashboard.title'),
      user: this.languageService.translate('common.user'),
      logout: this.languageService.translate('common.logout'),
    };
  });

  constructor() {

    effect(() => {

      const _ = this.languageService.currentLanguage();

      this.updateTime();

      this.cdr.markForCheck();
    });
  }

  get user() {
    return this.authService.user;
  }

  get isAuthenticated() {
    return this.authService.isAuthenticated();
  }

  getRoleDisplayName(): string {
    const user = this.user;
    if (!user || !user.role) return this.translations().user;
    return UserRoleNames[user.role as keyof typeof UserRoleNames] || user.roleName || this.translations().user;
  }

  onMenuToggle() {
    this.menuToggle.emit();
  }

  onLogout() {
    this.authService.logout();
  }

  ngOnInit() {
    this.updateTime();
    this.intervalSubscription = interval(1000).subscribe(() => {
      this.updateTime();
      this.cdr.markForCheck();
    });

    this.cdr.detectChanges();
  }

  ngOnDestroy() {
    if (this.intervalSubscription) {
      this.intervalSubscription.unsubscribe();
    }
  }

  private updateTime() {
    const now = new Date();
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';

    this.currentTime = now.toLocaleTimeString(locale, {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });
    this.currentDate = now.toLocaleDateString(locale, {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
    });
  }
}
