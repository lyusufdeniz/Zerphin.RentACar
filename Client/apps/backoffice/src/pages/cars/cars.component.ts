import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { SwalService } from '../../services/swal.service';
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
export class CarsComponent implements OnInit {
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private swalService = inject(SwalService);
  private cdr = inject(ChangeDetectorRef);

  // Data
  vehicles: Vehicle[] = [];
  totalCount = 0;
  isLoading = false;

  // Pagination
  currentPage = 1;
  pageSize = 100; // Default page size - load more results initially
  totalPages = 0;

  // Filters
  searchParams: VehicleSearchParams = {
    PageNumber: 1,
    PageSize: 100,
  };

  // Filter values
  searchBrand = '';
  searchModel = '';
  searchLicensePlate = '';
  selectedCategory?: VehicleCategory;
  selectedStatus?: VehicleStatus;
  minPrice?: number;
  maxPrice?: number;

  // Enums for template
  categories = Object.values(VehicleCategory).filter(
    (v) => typeof v === 'number'
  ) as VehicleCategory[];
  statuses = Object.values(VehicleStatus).filter(
    (v) => typeof v === 'number'
  ) as VehicleStatus[];
  categoryNames = VehicleCategoryNames;
  statusNames = VehicleStatusNames;

  // Order
  orderBy = 'brand';
  isDescending = false;

  ngOnInit() {
    this.loadVehicles();
  }

  /**
   * Load vehicles with current search params
   */
  loadVehicles() {
    this.isLoading = true;
    this.cdr.markForCheck();

    // Build search params
    const params: VehicleSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: this.orderBy,
      IsDescending: this.isDescending,
    };

    if (this.searchBrand) params.Brand = this.searchBrand;
    if (this.searchModel) params.Model = this.searchModel;
    if (this.searchLicensePlate) params.LicensePlate = this.searchLicensePlate;
    if (this.selectedCategory) params.Category = this.selectedCategory;
    if (this.selectedStatus) params.Status = this.selectedStatus;
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
        // Error is already handled by exception interceptor
      },
    });
  }

  /**
   * Search with filters
   */
  search() {
    this.currentPage = 1;
    this.loadVehicles();
  }

  /**
   * Clear all filters
   */
  clearFilters() {
    this.searchBrand = '';
    this.searchModel = '';
    this.searchLicensePlate = '';
    this.selectedCategory = undefined;
    this.selectedStatus = undefined;
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.orderBy = 'brand';
    this.isDescending = false;
    this.search();
  }

  /**
   * Change page
   */
  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadVehicles();
    }
  }

  /**
   * Change page size
   */
  changePageSize(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadVehicles();
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
    this.loadVehicles();
  }

  /**
   * Get status badge class
   */
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

  /**
   * Open add vehicle modal
   */
  openAddVehicleModal() {
    const { close, contentRef } = this.modalService.open(VehicleFormComponent, {
      title: 'Yeni Araç Ekle',
      size: 'large',
    });

    // Listen for saved event
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

  /**
   * Edit vehicle
   */
  editVehicle(vehicle: Vehicle) {
    const { close, contentRef } = this.modalService.open(VehicleFormComponent, {
      title: 'Araç Düzenle',
      size: 'large',
      inputs: { vehicle },
    });

    // Listen for saved event
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

  /**
   * View vehicle detail
   */
  viewVehicleDetail(vehicle: Vehicle) {
    this.modalService.open(VehicleDetailComponent, {
      title: `${vehicle.brand} ${vehicle.model} - Detaylar`,
      size: 'large',
      inputs: { vehicleId: vehicle.id },
    });
  }

  /**
   * Get vehicle image source
   */
  getVehicleImage(vehicle: Vehicle): string {
    if (vehicle.imageUrl) {
      return vehicle.imageUrl;
    }
    
    if (vehicle.imageBase64) {
      return vehicle.imageBase64;
    }
    
    // Placeholder
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9Ijc1IiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxyZWN0IHdpZHRoPSIxMDAiIGhlaWdodD0iNzUiIGZpbGw9IiMyNTI1MjUiLz48dGV4dCB4PSI1MCUiIHk9IjUwJSIgZm9udC1mYW1pbHk9IkFyaWFsIiBmb250LXNpemU9IjEyIiBmaWxsPSIjNjY2NjY2IiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBkeT0iLjNlbSI+QXJhw6c8L3RleHQ+PC9zdmc+';
  }

  /**
   * Handle image error
   */
  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    img.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9Ijc1IiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxyZWN0IHdpZHRoPSIxMDAiIGhlaWdodD0iNzUiIGZpbGw9IiMyNTI1MjUiLz48dGV4dCB4PSI1MCUiIHk9IjUwJSIgZm9udC1mYW1pbHk9IkFyaWFsIiBmb250LXNpemU9IjEyIiBmaWxsPSIjNjY2NjY2IiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBkeT0iLjNlbSI+QXJhw6c8L3RleHQ+PC9zdmc+';
  }

  /**
   * Delete vehicle
   */
  deleteVehicle(vehicle: Vehicle) {
    this.swalService
      .confirm(
        'Araç Sil',
        `${vehicle.brand} ${vehicle.model} aracını silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`,
        'Sil',
        'İptal'
      )
      .subscribe((result) => {
        if (result.isConfirmed) {
          this.vehicleService.deleteVehicle(vehicle.id).subscribe({
            next: () => {
              this.toastService.success('Araç başarıyla silindi');
              this.loadVehicles();
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

}
