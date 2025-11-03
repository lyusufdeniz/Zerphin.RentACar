import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DatepickerComponent } from '../../components/datepicker/datepicker.component';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { InsuranceService } from '../../services/insurance.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
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
export class InsurancesComponent implements OnInit {
  private insuranceService = inject(InsuranceService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private cdr = inject(ChangeDetectorRef);

  // Data
  insurances: Insurance[] = [];
  totalCount = 0;
  isLoading = false;

  // Pagination
  currentPage = 1;
  pageSize = 100;
  totalPages = 0;

  // Filters
  searchParams: InsuranceSearchParams = {
    PageNumber: 1,
    PageSize: 100,
  };

  // Filter values
  searchPolicyNumber = '';
  searchLicensePlate = '';
  startDateFrom?: string;
  startDateTo?: string;
  endDateFrom?: string;
  endDateTo?: string;

  // Enums for template
  statuses = Object.values(InsuranceStatus).filter(
    (v) => typeof v === 'number'
  ) as InsuranceStatus[];
  statusNames = InsuranceStatusNames;

  // Order
  orderBy = 'Id';
  isDescending = false;

  ngOnInit() {
    this.loadInsurances();
  }

  /**
   * Load insurances with current search params
   */
  loadInsurances() {
    this.isLoading = true;
    this.cdr.markForCheck();

    // Build search params
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
        // Error is already handled by exception interceptor
      },
    });
  }

  /**
   * Search with filters
   */
  search() {
    this.currentPage = 1;
    this.loadInsurances();
  }

  /**
   * Clear all filters
   */
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

  /**
   * Change page
   */
  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadInsurances();
    }
  }

  /**
   * Change page size
   */
  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadInsurances();
  }

  /**
   * Sort by column
   */
  sortBy(column: string) {
    if (this.orderBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.orderBy = column;
      this.isDescending = false;
    }
    this.loadInsurances();
  }

  /**
   * Delete insurance
   */
  deleteInsurance(insurance: Insurance) {
    this.swalService
      .confirm(
        'Sigorta Sil',
        `${insurance.policyNumber} poliçe numaralı sigortayı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`,
        'Sil',
        'İptal'
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.insuranceService.deleteInsurance(insurance.id).subscribe({
            next: () => {
              this.toastService.success('Sigorta başarıyla silindi');
              this.loadInsurances();
            },
            error: (error) => {
              // Error is already handled by exception interceptor
            },
          });
        }
      });
  }

  /**
   * Get page numbers for pagination
   */
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

  /**
   * Format date for display
   */
  formatDate(dateString: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
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
   * Get status name
   */
  getStatusName(insurance: Insurance): string {
    const status = insurance.status ?? this.calculateStatus(insurance);
    return this.statusNames[status] || '-';
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
   * Open add insurance modal
   */
  openAddInsuranceModal() {
    const { close, contentRef } = this.modalService.open(InsuranceFormComponent, {
      title: 'Yeni Sigorta Ekle',
      size: 'large',
    });

    // Listen for saved event
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

  /**
   * Edit insurance
   */
  editInsurance(insurance: Insurance) {
    const { close, contentRef } = this.modalService.open(InsuranceFormComponent, {
      title: 'Sigorta Düzenle',
      size: 'large',
      inputs: { insurance },
    });

    // Listen for saved event
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

  /**
   * View insurance details
   */
  viewInsuranceDetails(insurance: Insurance) {
    this.modalService.open(InsuranceDetailComponent, {
      title: 'Sigorta Detayları',
      size: 'large',
      inputs: { insuranceId: insurance.id },
    });
  }
}
