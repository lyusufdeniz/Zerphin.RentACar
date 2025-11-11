import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  ChangeDetectorRef,
  inject,
  computed,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Insurance,
  InsuranceStatus,
  InsuranceStatusNames,
} from '../../models/insurance';
import { InsuranceService } from '../../services/insurance.service';
import { ToastService } from '../../services/toast.service';
import { LanguageService } from '../../services/language.service';

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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  insurance: Insurance | null = null;
  isLoading = true;

  statusNames = InsuranceStatusNames;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      loading: this.languageService.translate('messages.details.insurance.loading'),
      notFound: this.languageService.translate('messages.details.notFound.insurance'),
      policyInfo: this.languageService.translate('messages.details.insurance.policyInfo'),
      policyNumber: this.languageService.translate('messages.details.insurance.policyNumber'),
      insuranceCompany: this.languageService.translate('messages.details.insurance.insuranceCompany'),
      status: this.languageService.translate('messages.details.insurance.status'),
      dateInfo: this.languageService.translate('messages.details.insurance.dateInfo'),
      startDate: this.languageService.translate('messages.details.insurance.startDate'),
      endDate: this.languageService.translate('messages.details.insurance.endDate'),
      createdAt: this.languageService.translate('messages.details.insurance.createdAt'),
      updatedAt: this.languageService.translate('messages.details.insurance.updatedAt'),
      financialInfo: this.languageService.translate('messages.details.insurance.financialInfo'),
      premiumAmount: this.languageService.translate('messages.details.insurance.premiumAmount'),
      coverageLimit: this.languageService.translate('messages.details.insurance.coverageLimit'),
      deductible: this.languageService.translate('messages.details.insurance.deductible'),
      coverageInfo: this.languageService.translate('messages.details.insurance.coverageInfo'),
      coverageType: this.languageService.translate('messages.details.insurance.coverageType'),
      coverageDetails: this.languageService.translate('messages.details.insurance.coverageDetails'),
      vehicleInfo: this.languageService.translate('messages.details.insurance.vehicleInfo'),
      vehicleId: this.languageService.translate('messages.details.insurance.vehicleId'),
      licensePlate: this.languageService.translate('messages.details.insurance.licensePlate'),
      brandModel: this.languageService.translate('messages.details.insurance.brandModel'),
      contactInfo: this.languageService.translate('messages.details.insurance.contactInfo'),
      contact: this.languageService.translate('messages.details.insurance.contact'),
      notes: this.languageService.translate('messages.details.insurance.notes'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    if (this.insuranceId) {
      this.loadInsurance();
    }
  }

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
      },
    });
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return date.toLocaleDateString(locale, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  formatDateTime(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return date.toLocaleString(locale, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  formatCurrency(amount: number | undefined): string {
    if (amount === undefined || amount === null) return '-';
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency: 'TRY',
    }).format(amount);
  }

  calculateStatus(insurance: Insurance): InsuranceStatus {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const startDate = new Date(insurance.startDate);
    startDate.setHours(0, 0, 0, 0);

    const endDate = new Date(insurance.endDate);
    endDate.setHours(0, 0, 0, 0);

    const daysUntilExpiration = Math.ceil((endDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));

    if (today > endDate) {
      return InsuranceStatus.Expired;
    }

    if (today < startDate) {
      return InsuranceStatus.Active;
    }

    if (daysUntilExpiration <= 30 && daysUntilExpiration > 0) {
      return InsuranceStatus.ExpiringSoon;
    }

    return InsuranceStatus.Active;
  }

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

  getStatusName(insurance: Insurance): string {
    const status = insurance.status ?? this.calculateStatus(insurance);
    return this.statusNames[status] || '-';
  }
}
