import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Insurance,
  InsuranceStatus,
  InsuranceStatusNames,
} from '../../models/insurance';
import { InsuranceService } from '../../services/insurance.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-insurance-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './insurance-detail.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InsuranceDetailComponent implements OnInit {
  @Input() insuranceId!: string;

  private insuranceService = inject(InsuranceService);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  insurance: Insurance | null = null;
  isLoading = true;

  statusNames = InsuranceStatusNames;

  ngOnInit() {
    if (this.insuranceId) {
      this.loadInsurance();
    }
  }

  /**
   * Load insurance details
   */
  loadInsurance() {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.insuranceService.getInsuranceById(this.insuranceId).subscribe({
      next: (insurance) => {
        this.insurance = insurance;
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
   * Format date for display
   */
  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  /**
   * Format date-time for display
   */
  formatDateTime(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  /**
   * Format currency
   */
  formatCurrency(amount: number | undefined): string {
    if (amount === undefined || amount === null) return '-';
    return new Intl.NumberFormat('tr-TR', {
      style: 'currency',
      currency: 'TRY',
    }).format(amount);
  }

  /**
   * Calculate status based on dates
   */
  calculateStatus(insurance: Insurance): InsuranceStatus {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const startDate = new Date(insurance.startDate);
    startDate.setHours(0, 0, 0, 0);

    const endDate = new Date(insurance.endDate);
    endDate.setHours(0, 0, 0, 0);

    // Calculate days until expiration
    const daysUntilExpiration = Math.ceil((endDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));

    // If end date has passed
    if (today > endDate) {
      return InsuranceStatus.Expired;
    }

    // If start date hasn't arrived yet
    if (today < startDate) {
      return InsuranceStatus.Active; // Will be active when start date arrives
    }

    // If expiring within 30 days
    if (daysUntilExpiration <= 30 && daysUntilExpiration > 0) {
      return InsuranceStatus.ExpiringSoon;
    }

    // Otherwise active
    return InsuranceStatus.Active;
  }

  /**
   * Get status badge class
   */
  getStatusClass(insurance: Insurance): string {
    const status = insurance.status ?? this.calculateStatus(insurance);
    
    switch (status) {
      case InsuranceStatus.Active:
        return 'status-active';
      case InsuranceStatus.ExpiringSoon:
        return 'status-warning';
      case InsuranceStatus.Expired:
        return 'status-expired';
      case InsuranceStatus.Cancelled:
        return 'status-cancelled';
      default:
        return '';
    }
  }

  /**
   * Get status name
   */
  getStatusName(insurance: Insurance): string {
    const status = insurance.status ?? this.calculateStatus(insurance);
    return this.statusNames[status] || '-';
  }
}

