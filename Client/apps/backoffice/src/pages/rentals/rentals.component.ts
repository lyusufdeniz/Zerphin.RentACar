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
import { RentalService } from '../../services/rental.service';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
import { LanguageService } from '../../services/language.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';
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
export class RentalsComponent implements OnInit, OnDestroy {
  private rentalService = inject(RentalService);
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);
  private languageSubscription?: Subscription;

  translations = computed(() => {
    const currentLang = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('pages.rentals.title'),
      customer: this.languageService.translate('pages.rentals.customer'),
      vehicle: this.languageService.translate('pages.rentals.vehicle'),
      status: this.languageService.translate('pages.rentals.status'),
      searchCustomer: this.languageService.translate('pages.rentals.searchCustomer'),
      startDateFrom: this.languageService.translate('pages.rentals.startDateFrom'),
      startDateTo: this.languageService.translate('pages.rentals.startDateTo'),
      endDateFrom: this.languageService.translate('pages.rentals.endDateFrom'),
      endDateTo: this.languageService.translate('pages.rentals.endDateTo'),
      all: this.languageService.translate('pages.rentals.all'),
      loading: this.languageService.translate('pages.rentals.loading'),
      noData: this.languageService.translate('pages.rentals.noData'),
      add: this.languageService.translate('pages.rentals.add'),
      edit: this.languageService.translate('pages.rentals.edit'),
      delete: this.languageService.translate('pages.rentals.delete'),
      view: this.languageService.translate('pages.rentals.view'),
      search: this.languageService.translate('common.search'),
      clear: this.languageService.translate('common.clear'),
      table: {
        startDate: this.languageService.translate('pages.rentals.table.startDate'),
        endDate: this.languageService.translate('pages.rentals.table.endDate'),
        customer: this.languageService.translate('pages.rentals.table.customer'),
        vehicle: this.languageService.translate('pages.rentals.table.vehicle'),
        dailyPrice: this.languageService.translate('pages.rentals.table.dailyPrice'),
        totalAmount: this.languageService.translate('pages.rentals.table.totalAmount'),
        status: this.languageService.translate('pages.rentals.table.status'),
        actions: this.languageService.translate('pages.rentals.table.actions'),
      },
      pagination: {
        previous: this.languageService.translate('pages.rentals.pagination.previous'),
        next: this.languageService.translate('pages.rentals.pagination.next'),
      },
    };
  });

  public languageServicePublic = this.languageService; 

  rentals: Rental[] = [];
  totalCount = 0;
  isLoading = false;
  vehicles: Vehicle[] = []; 

  currentPage = 1;
  pageSize = 100;
  totalPages = 0;

  searchParams: RentalSearchParams = {
    PageNumber: 1,
    PageSize: 100,
  };

  searchCustomerId = '';
  searchCustomerQuery = ''; 
  searchVehicleId = '';
  selectedStatus: RentalStatus | null = null;
  startDateFrom?: string;
  startDateTo?: string;
  endDateFrom?: string;
  endDateTo?: string;

  statuses = Object.values(RentalStatus).filter(
    (v) => typeof v === 'number'
  ) as RentalStatus[];
  statusNames = RentalStatusNames;

  orderBy = 'Id';
  isDescending = false;

  ngOnInit() {
    this.loadRentals();
    this.loadVehiclesForFilter();

    this.languageSubscription = toObservable(this.languageService.currentLanguage).subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  loadRentals() {
    this.isLoading = true;
    this.cdr.markForCheck();

    const params: RentalSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchCustomerQuery.trim()) {
      const query = this.searchCustomerQuery.trim();

      if (/^\d+$/.test(query)) {

        if (query.length === 11) {
          params.CustomerIdentityNumber = query;
        } else {
          params.CustomerLicenseNumber = query;
        }
      } else {

        const parts = query.split(/\s+/);
        if (parts.length >= 2) {
          params.CustomerFirstName = parts[0];
          params.CustomerLastName = parts.slice(1).join(' ');
        } else {

          params.CustomerFirstName = query;
        }
      }
    } else if (this.searchCustomerId) {
      params.CustomerId = this.searchCustomerId;
    }
    if (this.searchVehicleId) params.VehicleId = this.searchVehicleId;
    if (this.selectedStatus !== null) {
      params.Status = this.selectedStatus;
    }
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
      },
    });
  }

  loadVehiclesForFilter() {
    this.vehicleService.searchVehicles({ PageSize: 9999 }).subscribe({
      next: (response) => {
        this.vehicles = response.vehicles || [];
        this.cdr.markForCheck();
      },
      error: () => {
      },
    });
  }

  search() {
    this.currentPage = 1;
    this.loadRentals();
  }

  clearFilters() {
    this.searchCustomerId = '';
    this.searchCustomerQuery = '';
    this.searchVehicleId = '';
    this.selectedStatus = null;
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
      this.loadRentals();
    }
  }

  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadRentals();
  }

  sortBy(column: string) {
    if (this.orderBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.orderBy = column;
      this.isDescending = false;
    }
    this.loadRentals();
  }

  deleteRental(rental: Rental) {
    this.swalService
      .confirm(
        this.languageService.translate('messages.deleteConfirm.rental.title'),
        this.languageService.translate('messages.deleteConfirm.rental.message'),
        this.languageService.translate('messages.deleteConfirm.rental.confirm'),
        this.languageService.translate('messages.deleteConfirm.rental.cancel')
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.rentalService.deleteRental(rental.id).subscribe({
            next: () => {
              this.toastService.success(this.languageService.translate('messages.success.rental.deleted'));
              this.loadRentals();
            },
            error: () => {
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

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
  }

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

  getStatusClass(status: RentalStatus): string {
    switch (status) {
      case RentalStatus.Active:
        return 'status-active'; 
      case RentalStatus.Completed:
        return 'status-info'; 
      case RentalStatus.Cancelled:
        return 'status-expired'; 
      default:
        return '';
    }
  }

  openAddRentalModal() {
    const { close, contentRef } = this.modalService.open(RentalFormComponent, {
      title: this.languageService.translate('messages.modal.rental.add'),
      size: 'large',
    });

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

  editRental(rental: Rental) {
    const { close, contentRef } = this.modalService.open(RentalFormComponent, {
      title: this.languageService.translate('messages.modal.rental.edit'),
      size: 'large',
      inputs: { rental },
    });

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

  viewRentalDetails(rental: Rental) {
    this.modalService.open(RentalDetailComponent, {
      title: this.languageService.translate('messages.modal.rental.details'),
      size: 'large',
      inputs: { rentalId: rental.id },
    });
  }
}
