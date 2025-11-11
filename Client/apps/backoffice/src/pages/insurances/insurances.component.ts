import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  computed,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DatepickerComponent } from '../../components/datepicker/datepicker.component';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { InsuranceService } from '../../services/insurance.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
import { LanguageService } from '../../services/language.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';
import { InsuranceFormComponent } from '../../components/insurance-form/insurance-form.component';
import { InsuranceDetailComponent } from '../../components/insurance-detail/insurance-detail.component';
import {
  Insurance,
  InsuranceStatus,
  InsuranceStatusNames,
  InsuranceSearchParams,
  PaginatedInsuranceResponse,
} from '../../models/insurance';

@Component({
  selector: 'app-insurances',
  standalone: true,
  imports: [CommonModule, FormsModule, FavoriteButtonComponent, DatepickerComponent],
  templateUrl: './insurances.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InsurancesComponent implements OnInit, OnDestroy {
  private insuranceService = inject(InsuranceService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);
  private languageSubscription?: Subscription;

  translations = computed(() => {
    const currentLang = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('pages.insurances.title'),
      policyNumber: this.languageService.translate('pages.insurances.policyNumber'),
      policyNumberPlaceholder: this.languageService.translate('pages.insurances.policyNumberPlaceholder'),
      licensePlate: this.languageService.translate('pages.insurances.licensePlate'),
      licensePlatePlaceholder: this.languageService.translate('pages.insurances.licensePlatePlaceholder'),
      startDateFrom: this.languageService.translate('pages.insurances.startDateFrom'),
      startDateTo: this.languageService.translate('pages.insurances.startDateTo'),
      endDateFrom: this.languageService.translate('pages.insurances.endDateFrom'),
      endDateTo: this.languageService.translate('pages.insurances.endDateTo'),
      loading: this.languageService.translate('pages.insurances.loading'),
      noData: this.languageService.translate('pages.insurances.noData'),
      add: this.languageService.translate('pages.insurances.add'),
      edit: this.languageService.translate('pages.insurances.edit'),
      delete: this.languageService.translate('pages.insurances.delete'),
      view: this.languageService.translate('pages.insurances.view'),
      search: this.languageService.translate('common.search'),
      clear: this.languageService.translate('common.clear'),
      table: {
        policyNumber: this.languageService.translate('pages.insurances.table.policyNumber'),
        insuranceCompany: this.languageService.translate('pages.insurances.table.insuranceCompany'),
        vehicle: this.languageService.translate('pages.insurances.table.vehicle'),
        startDate: this.languageService.translate('pages.insurances.table.startDate'),
        endDate: this.languageService.translate('pages.insurances.table.endDate'),
        premiumAmount: this.languageService.translate('pages.insurances.table.premiumAmount'),
        status: this.languageService.translate('pages.insurances.table.status'),
        actions: this.languageService.translate('pages.insurances.table.actions'),
      },
      pagination: {
        previous: this.languageService.translate('pages.insurances.pagination.previous'),
        next: this.languageService.translate('pages.insurances.pagination.next'),
      },
    };
  });

  public languageServicePublic = this.languageService; 

  insurances: Insurance[] = [];
  totalCount = 0;
  isLoading = false;

  currentPage = 1;
  pageSize = 100;
  totalPages = 0;

  searchParams: InsuranceSearchParams = {
    PageNumber: 1,
    PageSize: 100,
  };

  searchPolicyNumber = '';
  searchLicensePlate = '';
  startDateFrom?: string;
  startDateTo?: string;
  endDateFrom?: string;
  endDateTo?: string;

  statuses = Object.values(InsuranceStatus).filter(
    (v) => typeof v === 'number'
  ) as InsuranceStatus[];
  statusNames = InsuranceStatusNames;

  orderBy = 'Id';
  isDescending = false;

  ngOnInit() {
    this.loadInsurances();

    this.languageSubscription = toObservable(this.languageService.currentLanguage).subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  loadInsurances() {
    this.isLoading = true;
    this.cdr.markForCheck();

    const params: InsuranceSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchPolicyNumber) params.PolicyNumber = this.searchPolicyNumber;
    if (this.searchLicensePlate) params.LicensePlate = this.searchLicensePlate;
    if (this.startDateFrom) params.StartDateFrom = this.startDateFrom;
    if (this.startDateTo) params.StartDateTo = this.startDateTo;
    if (this.endDateFrom) params.EndDateFrom = this.endDateFrom;
    if (this.endDateTo) params.EndDateTo = this.endDateTo;

    this.insuranceService.searchInsurances(params).subscribe({
      next: (response) => {
        this.insurances = response.insurances || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || 0;
        this.currentPage = response.pageNumber || 1;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.cdr.markForCheck();
      },
    });
  }

  search() {
    this.currentPage = 1;
    this.loadInsurances();
  }

  clearFilters() {
    this.searchPolicyNumber = '';
    this.searchLicensePlate = '';
    this.startDateFrom = undefined;
    this.startDateTo = undefined;
    this.endDateFrom = undefined;
    this.endDateTo = undefined;
    this.orderBy = 'Id';
    this.isDescending = false;
    this.search();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadInsurances();
    }
  }

  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadInsurances();
  }

  sortBy(column: string) {
    if (this.orderBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.orderBy = column;
      this.isDescending = false;
    }
    this.loadInsurances();
  }

  deleteInsurance(insurance: Insurance) {
    this.swalService
      .confirm(
        this.languageService.translate('messages.deleteConfirm.insurance.title'),
        this.languageService.translateWithParams('messages.deleteConfirm.insurance.message', {
          policyNumber: insurance.policyNumber,
        }),
        this.languageService.translate('messages.deleteConfirm.insurance.confirm'),
        this.languageService.translate('messages.deleteConfirm.insurance.cancel')
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.insuranceService.deleteInsurance(insurance.id).subscribe({
            next: () => {
              this.toastService.success(this.languageService.translate('messages.success.insurance.deleted'));
              this.loadInsurances();
            },
            error: (error) => {
            },
          });
        }
      });
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPages - 1);

    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  formatDate(dateString: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
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

  getStatusName(insurance: Insurance): string {
    const status = insurance.status ?? this.calculateStatus(insurance);
    return this.statusNames[status] || '-';
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

  openAddInsuranceModal() {
    const { close, contentRef } = this.modalService.open(InsuranceFormComponent, {
      title: this.languageService.translate('messages.modal.insurance.add'),
      size: 'large',
    });

    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as InsuranceFormComponent;

      const savedSub = formComponent.saved.subscribe((insurance: Insurance) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadInsurances();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  editInsurance(insurance: Insurance) {
    const { close, contentRef } = this.modalService.open(InsuranceFormComponent, {
      title: this.languageService.translate('messages.modal.insurance.edit'),
      size: 'large',
      inputs: { insurance },
    });

    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as InsuranceFormComponent;

      const savedSub = formComponent.saved.subscribe((updatedInsurance: Insurance) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadInsurances();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  viewInsuranceDetails(insurance: Insurance) {
    this.modalService.open(InsuranceDetailComponent, {
      title: this.languageService.translate('messages.modal.insurance.details'),
      size: 'large',
      inputs: { insuranceId: insurance.id },
    });
  }
}
