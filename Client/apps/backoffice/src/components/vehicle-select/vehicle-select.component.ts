import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  Output,
  EventEmitter,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle, VehicleSearchParams } from '../../models/vehicle';

@Component({
  selector: 'app-vehicle-select',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VehicleSelectComponent implements OnInit {
  @Output() vehicleSelected = new EventEmitter<Vehicle>();

  private vehicleService = inject(VehicleService);
  private cdr = inject(ChangeDetectorRef);

  vehicles: Vehicle[] = [];
  isLoading = false;
  searchQuery = '';
  currentPage = 1;
  pageSize = 20;
  totalPages = 0;
  totalCount = 0;

  ngOnInit() {
    this.loadVehicles();
  }

  /**
   * Load vehicles with search
   */
  loadVehicles() {
    this.isLoading = true;
    this.cdr.markForCheck();

    const params: VehicleSearchParams = {
      PageNumber: this.currentPage,
      PageSize: this.pageSize,
      OrderBy: 'LicensePlate',
      IsDescending: false,
    };

    if (this.searchQuery.trim()) {
      const query = this.searchQuery.trim();
      // Try to detect if it's a license plate (contains numbers and letters)
      // Otherwise search by brand/model
      if (/^[0-9]{2}[A-Z]{1,3}[0-9]{2,4}$/.test(query.toUpperCase().replace(/\s/g, ''))) {
        params.LicensePlate = query;
      } else {
        params.Brand = query;
        params.Model = query;
      }
    }

    this.vehicleService.searchVehicles(params).subscribe({
      next: (response) => {
        this.vehicles = response.vehicles || [];
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
   * Search vehicles
   */
  search() {
    this.currentPage = 1;
    this.loadVehicles();
  }

  /**
   * Clear search
   */
  clearSearch() {
    this.searchQuery = '';
    this.currentPage = 1;
    this.loadVehicles();
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
   * Select vehicle
   */
  selectVehicle(vehicle: Vehicle) {
    this.vehicleSelected.emit(vehicle);
  }

  /**
   * Get vehicle display name
   */
  getVehicleDisplayName(vehicle: Vehicle): string {
    return `${vehicle.licensePlate} - ${vehicle.brand} ${vehicle.model}`;
  }

  /**
   * Get vehicle details text
   */
  getVehicleDetailsText(vehicle: Vehicle): string {
    const details: string[] = [];
    if (vehicle.year) {
      details.push(`${vehicle.year}`);
    }
    if (vehicle.category) {
      const categoryNames: Record<number, string> = {
        1: 'Hatchback',
        2: 'Sedan',
        3: 'SUV',
        4: 'Pickup',
      };
      details.push(categoryNames[vehicle.category] || '');
    }
    if (vehicle.dailyRentalPrice) {
      details.push(`${vehicle.dailyRentalPrice} TL/gün`);
    }
    if (vehicle.status) {
      const statusNames: Record<number, string> = {
        1: 'Müsait',
        2: 'Kiralanmış',
      };
      details.push(statusNames[vehicle.status] || '');
    }
    return details.filter(d => d).join(' • ');
  }

  /**
   * Get vehicle image URL
   */
  getVehicleImage(vehicle: Vehicle): string | null {
    if (vehicle.imageUrl) {
      return vehicle.imageUrl;
    }
    if (vehicle.imageBase64) {
      return `data:image/jpeg;base64,${vehicle.imageBase64}`;
    }
    return null;
  }
}

