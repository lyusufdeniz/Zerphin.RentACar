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
  Rental,
  RentalStatus,
  RentalStatusNames,
} from '../../models/rental';
import { RentalService } from '../../services/rental.service';
import { ToastService } from '../../services/toast.service';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle, VehicleCategoryNames, VehicleCategory } from '../../models/vehicle';

@Component({
  selector: 'app-rental-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rental-detail.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RentalDetailComponent implements OnInit {
  @Input() rentalId!: string;

  private rentalService = inject(RentalService);
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  rental: Rental | null = null;
  vehicle: Vehicle | null = null;
  isLoading = true;

  statusNames = RentalStatusNames;

  ngOnInit() {
    if (this.rentalId) {
      this.loadRental();
    }
  }

  /**
   * Load rental details
   */
  loadRental() {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.rentalService.getRentalById(this.rentalId).subscribe({
      next: (rental) => {
        this.rental = rental;
        // Load vehicle details if vehicleId exists
        if (rental.vehicleId) {
          this.loadVehicle(rental.vehicleId);
        } else {
          this.isLoading = false;
          this.cdr.markForCheck();
        }
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
   * Load vehicle details
   */
  loadVehicle(vehicleId: string) {
    this.vehicleService.getVehicleById(vehicleId).subscribe({
      next: (vehicle) => {
        this.vehicle = vehicle;
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
   * Get vehicle image
   */
  getVehicleImage(): string | null {
    if (!this.vehicle) return null;
    if (this.vehicle.imageUrl) {
      return this.vehicle.imageUrl;
    }
    if (this.vehicle.imageBase64) {
      return `data:image/jpeg;base64,${this.vehicle.imageBase64}`;
    }
    return null;
  }

  /**
   * Get vehicle display name
   */
  getVehicleDisplayName(): string {
    if (!this.vehicle) return '';
    return `${this.vehicle.brand} ${this.vehicle.model}`;
  }

  /**
   * Get category name
   */
  getCategoryName(category: number): string {
    return VehicleCategoryNames[category as VehicleCategory] || '';
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
}

