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
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
import { LanguageService } from '../../services/language.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';
import { VehicleFormComponent } from '../../components/vehicle-form/vehicle-form.component';
import { VehicleDetailComponent } from '../../components/vehicle-detail/vehicle-detail.component';
import {
  Vehicle,
  VehicleCategory,
  VehicleStatus,
  VehicleCategoryNames,
  VehicleStatusNames,
  VehicleSearchParams,
  PaginatedVehicleResponse,
} from '../../models/vehicle';

@Component({
  selector: 'app-cars',
  standalone: true,
  imports: [CommonModule, FormsModule, FavoriteButtonComponent],
  templateUrl: './cars.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CarsComponent implements OnInit, OnDestroy {
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private languageService = inject(LanguageService);
  public languageServicePublic = this.languageService; 
  private cdr = inject(ChangeDetectorRef);
  private languageSubscription?: Subscription;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('pages.cars.title'),
      brand: this.languageService.translate('pages.cars.brand'),
      model: this.languageService.translate('pages.cars.model'),
      licensePlate: this.languageService.translate('pages.cars.licensePlate'),
      category: this.languageService.translate('pages.cars.category'),
      status: this.languageService.translate('pages.cars.status'),
      searchBrand: this.languageService.translate('pages.cars.searchBrand'),
      searchModel: this.languageService.translate('pages.cars.searchModel'),
      searchLicensePlate: this.languageService.translate('pages.cars.searchLicensePlate'),
      minPrice: this.languageService.translate('pages.cars.minPrice'),
      maxPrice: this.languageService.translate('pages.cars.maxPrice'),
      min: this.languageService.translate('pages.cars.min'),
      max: this.languageService.translate('pages.cars.max'),
      all: this.languageService.translate('pages.cars.all'),
      loading: this.languageService.translate('pages.cars.loading'),
      noData: this.languageService.translate('pages.cars.noData'),
      add: this.languageService.translate('pages.cars.add'),
      edit: this.languageService.translate('pages.cars.edit'),
      delete: this.languageService.translate('pages.cars.delete'),
      view: this.languageService.translate('pages.cars.view'),
      search: this.languageService.translate('common.search'),
      clear: this.languageService.translate('common.clear'),
      table: {
        image: this.languageService.translate('pages.cars.table.image'),
        brand: this.languageService.translate('pages.cars.table.brand'),
        model: this.languageService.translate('pages.cars.table.model'),
        licensePlate: this.languageService.translate('pages.cars.table.licensePlate'),
        category: this.languageService.translate('pages.cars.table.category'),
        status: this.languageService.translate('pages.cars.table.status'),
        dailyPrice: this.languageService.translate('pages.cars.table.dailyPrice'),
        year: this.languageService.translate('pages.cars.table.year'),
        km: this.languageService.translate('pages.cars.table.km'),
        actions: this.languageService.translate('pages.cars.table.actions'),
      },
      pagination: {
        previous: this.languageService.translate('pages.cars.pagination.previous'),
        next: this.languageService.translate('pages.cars.pagination.next'),
      },
    };
  });

  vehicles: Vehicle[] = [];
  totalCount = 0;
  isLoading = false;

  currentPage = 1;
  pageSize = 100; 
  totalPages = 0;

  searchParams: VehicleSearchParams = {
    PageNumber: 1,
    PageSize: 100,
  };

  searchBrand = '';
  searchModel = '';
  searchLicensePlate = '';
  selectedCategory: VehicleCategory | null = null;
  selectedStatus: VehicleStatus | null = null;
  minPrice?: number;
  maxPrice?: number;

  categories = Object.values(VehicleCategory).filter(
    (v) => typeof v === 'number'
  ) as VehicleCategory[];
  statuses = Object.values(VehicleStatus).filter(
    (v) => typeof v === 'number'
  ) as VehicleStatus[];
  categoryNames = VehicleCategoryNames;
  statusNames = VehicleStatusNames;

  orderBy = 'brand';
  isDescending = false;

  ngOnInit() {
    this.loadVehicles();

    this.languageSubscription = toObservable(this.languageService.currentLanguage).subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  loadVehicles() {
    this.isLoading = true;
    this.cdr.markForCheck();

    const params: VehicleSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchBrand) params.Brand = this.searchBrand;
    if (this.searchModel) params.Model = this.searchModel;
    if (this.searchLicensePlate) params.LicensePlate = this.searchLicensePlate;
    if (this.selectedCategory !== null) {
      params.Category = this.selectedCategory;
    }
    if (this.selectedStatus !== null) {
      params.Status = this.selectedStatus;
    }
    if (this.minPrice) params.MinPrice = this.minPrice;
    if (this.maxPrice) params.MaxPrice = this.maxPrice;

    this.vehicleService.searchVehicles(params).subscribe({
      next: (response) => {
        this.vehicles = response.vehicles || [];
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
    this.loadVehicles();
  }

  clearFilters() {
    this.searchBrand = '';
    this.searchModel = '';
    this.searchLicensePlate = '';
    this.selectedCategory = null;
    this.selectedStatus = null;
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.orderBy = 'brand';
    this.isDescending = false;
    this.search();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadVehicles();
    }
  }

  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadVehicles();
  }

  sortBy(column: string) {
    if (this.orderBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.orderBy = column;
      this.isDescending = false;
    }
    this.loadVehicles();
  }

  getStatusClass(status: VehicleStatus): string {
    switch (status) {
      case VehicleStatus.Available:
        return 'status-active';
      case VehicleStatus.Rented:
        return 'status-warning';
      default:
        return '';
    }
  }

  openAddVehicleModal() {
    const { close, contentRef } = this.modalService.open(VehicleFormComponent, {
      title: this.languageService.translate('messages.modal.vehicle.add'),
      size: 'large',
    });

    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as VehicleFormComponent;

      const savedSub = formComponent.saved.subscribe((vehicle: Vehicle) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadVehicles();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  editVehicle(vehicle: Vehicle) {
    const { close, contentRef } = this.modalService.open(VehicleFormComponent, {
      title: this.languageService.translate('messages.modal.vehicle.edit'),
      size: 'large',
      inputs: { vehicle },
    });

    if (contentRef && contentRef.instance) {
      const formComponent = contentRef.instance as VehicleFormComponent;

      const savedSub = formComponent.saved.subscribe((updatedVehicle: Vehicle) => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
        this.loadVehicles();
      });

      const cancelledSub = formComponent.cancelled.subscribe(() => {
        savedSub.unsubscribe();
        cancelledSub.unsubscribe();
        close();
      });
    }
  }

  viewVehicleDetail(vehicle: Vehicle) {
    this.modalService.open(VehicleDetailComponent, {
      title: this.languageService.translateWithParams('messages.modal.vehicle.details', {
        brand: vehicle.brand,
        model: vehicle.model,
      }),
      size: 'large',
      inputs: { vehicleId: vehicle.id },
    });
  }

  getVehicleImage(vehicle: Vehicle): string {
    if (vehicle.imageUrl) {
      return vehicle.imageUrl;
    }

    if (vehicle.imageBase64) {
      return vehicle.imageBase64;
    }

    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9Ijc1IiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxyZWN0IHdpZHRoPSIxMDAiIGhlaWdodD0iNzUiIGZpbGw9IiMyNTI1MjUiLz48dGV4dCB4PSI1MCUiIHk9IjUwJSIgZm9udC1mYW1pbHk9IkFyaWFsIiBmb250LXNpemU9IjEyIiBmaWxsPSIjNjY2NjY2IiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBkeT0iLjNlbSI+QXJhw6c8L3RleHQ+PC9zdmc+';
  }

  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    img.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9Ijc1IiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxyZWN0IHdpZHRoPSIxMDAiIGhlaWdodD0iNzUiIGZpbGw9IiMyNTI1MjUiLz48dGV4dCB4PSI1MCUiIHk9IjUwJSIgZm9udC1mYW1pbHk9IkFyaWFsIiBmb250LXNpemU9IjEyIiBmaWxsPSIjNjY2NjY2IiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBkeT0iLjNlbSI+QXJhw6c8L3RleHQ+PC9zdmc+';
  }

  deleteVehicle(vehicle: Vehicle) {
    this.swalService
      .confirm(
        this.languageService.translate('messages.deleteConfirm.vehicle.title'),
        this.languageService.translateWithParams('messages.deleteConfirm.vehicle.message', {
          brand: vehicle.brand,
          model: vehicle.model,
        }),
        this.languageService.translate('messages.deleteConfirm.vehicle.confirm'),
        this.languageService.translate('messages.deleteConfirm.vehicle.cancel')
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.vehicleService.deleteVehicle(vehicle.id).subscribe({
            next: () => {
              this.toastService.success(this.languageService.translate('messages.success.vehicle.deleted'));
              this.loadVehicles();
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

}
