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
import { RentalService } from '../../services/rental.service';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
import { RentalFormComponent } from '../../components/rental-form/rental-form.component';
import { RentalDetailComponent } from '../../components/rental-detail/rental-detail.component';
import {
  Rental,
  RentalStatus,
  RentalStatusNames,
  RentalSearchParams,
  PaginatedRentalResponse,
} from '../../models/rental';
import { Vehicle } from '../../models/vehicle';

@Component({
  selector: 'app-rentals',
  standalone: true,
  imports: [CommonModule, FormsModule, FavoriteButtonComponent, DatepickerComponent],
  templateUrl: './rentals.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RentalsComponent implements OnInit {
  private rentalService = inject(RentalService);
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private cdr = inject(ChangeDetectorRef);

  // Data
  rentals: Rental[] = [];
  totalCount = 0;
  isLoading = false;
  vehicles: Vehicle[] = []; // For vehicle dropdown in filters

  // Pagination
  currentPage = 1;
  pageSize = 100;
  totalPages = 0;

  // Filters
  searchParams: RentalSearchParams = {
    PageNumber: 1,
    PageSize: 100,
  };

  // Filter values
  searchCustomerId = '';
  searchVehicleId = '';
  selectedStatus?: RentalStatus;
  startDateFrom?: string;
  startDateTo?: string;
  endDateFrom?: string;
  endDateTo?: string;

  // Enums for template
  statuses = Object.values(RentalStatus).filter(
    (v) => typeof v === 'number'
  ) as RentalStatus[];
  statusNames = RentalStatusNames;

  // Order
  orderBy = 'Id';
  isDescending = false;

  ngOnInit() {
    this.loadRentals();
    this.loadVehiclesForFilter();
  }

  /**
   * Load rentals with current search params
   */
  loadRentals() {
    this.isLoading = true;
    this.cdr.markForCheck();

    // Build search params
    const params: RentalSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchCustomerId) params.CustomerId = this.searchCustomerId;
    if (this.searchVehicleId) params.VehicleId = this.searchVehicleId;
    if (this.selectedStatus) params.Status = this.selectedStatus;
    if (this.startDateFrom) params.StartDateFrom = this.startDateFrom;
    if (this.startDateTo) params.StartDateTo = this.startDateTo;
    if (this.endDateFrom) params.EndDateFrom = this.endDateFrom;
    if (this.endDateTo) params.EndDateTo = this.endDateTo;

    this.rentalService.searchRentals(params).subscribe({
      next: (response) => {
        this.rentals = response.rentals || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || 0;
        this.currentPage = response.pageNumber || 1;
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
   * Load vehicles for filter dropdown
   */
  loadVehiclesForFilter() {
    this.vehicleService.searchVehicles({ PageSize: 9999 }).subscribe({
      next: (response) => {
        this.vehicles = response.vehicles || [];
        this.cdr.markForCheck();
      },
      error: () => {
        // Error is already handled by exception interceptor
      },
    });
  }

  /**
   * Search with filters
   */
  search() {
    this.currentPage = 1;
    this.loadRentals();
  }

  /**
   * Clear all filters
   */
  clearFilters() {
    this.searchCustomerId = '';
    this.searchVehicleId = '';
    this.selectedStatus = undefined;
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
      this.loadRentals();
    }
  }

  /**
   * Change page size
   */
  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadRentals();
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
    this.loadRentals();
  }

  /**
   * Delete rental
   */
  deleteRental(rental: Rental) {
    this.swalService
      .confirm(
        'Kiralama Sil',
        `Bu kiralamayı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`,
        'Sil',
        'İptal'
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.rentalService.deleteRental(rental.id).subscribe({
            next: () => {
              this.toastService.success('Kiralama başarıyla silindi');
              this.loadRentals();
            },
            error: () => {
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
  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
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
   * Get status badge class
   */
  getStatusClass(status: RentalStatus): string {
    switch (status) {
      case RentalStatus.Active:
        return 'status-active'; // Yeşil
      case RentalStatus.Completed:
        return 'status-info'; // Mavi
      case RentalStatus.Cancelled:
        return 'status-expired'; // Kırmızı
      default:
        return '';
    }
  }

  /**
   * Open add rental modal
   */
  openAddRentalModal() {
    const { close, contentRef } = this.modalService.open(RentalFormComponent, {
      title: 'Yeni Kiralama Ekle',
      size: 'large',
    });

    // Listen for saved event
    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as RentalFormComponent;

      const savedSub = formComponent.saved.subscribe((rental: Rental) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadRentals();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  /**
   * Edit rental
   */
  editRental(rental: Rental) {
    const { close, contentRef } = this.modalService.open(RentalFormComponent, {
      title: 'Kiralama Düzenle',
      size: 'large',
      inputs: { rental },
    });

    // Listen for saved event
    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as RentalFormComponent;

      const savedSub = formComponent.saved.subscribe((updatedRental: Rental) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadRentals();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  /**
   * View rental details
   */
  viewRentalDetails(rental: Rental) {
    this.modalService.open(RentalDetailComponent, {
      title: 'Kiralama Detayları',
      size: 'large',
      inputs: { rentalId: rental.id },
    });
  }
}
