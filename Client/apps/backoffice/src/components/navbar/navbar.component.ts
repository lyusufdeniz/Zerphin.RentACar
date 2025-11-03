import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  OnDestroy,
  Output,
  EventEmitter,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
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
  
  currentTime: string = '';
  currentDate: string = '';
  private intervalSubscription?: Subscription;

  constructor(private cdr: ChangeDetectorRef) {}

  get user() {
    return this.authService.user;
  }

  get isAuthenticated() {
    return this.authService.isAuthenticated();
  }

  getRoleDisplayName(): string {
    const user = this.user;
    if (!user || !user.role) return 'Kullanıcı';
    return UserRoleNames[user.role as keyof typeof UserRoleNames] || user.roleName || 'Kullanıcı';
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
  }

  ngOnDestroy() {
    if (this.intervalSubscription) {
      this.intervalSubscription.unsubscribe();
    }
  }

  private updateTime() {
    const now = new Date();
    this.currentTime = now.toLocaleTimeString('tr-TR', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });
    this.currentDate = now.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
    });
  }
}

