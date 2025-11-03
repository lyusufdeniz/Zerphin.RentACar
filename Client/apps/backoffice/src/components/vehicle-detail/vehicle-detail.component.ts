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
  Vehicle,
  VehicleCategoryNames,
  VehicleStatusNames,
  VehicleStatus,
} from '../../models/vehicle';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-vehicle-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicle-detail.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VehicleDetailComponent implements OnInit {
  @Input() vehicleId!: string;

  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  vehicle: Vehicle | null = null;
  isLoading = true;

  categoryNames = VehicleCategoryNames;
  statusNames = VehicleStatusNames;

  ngOnInit() {
    if (this.vehicleId) {
      this.loadVehicle();
    }
  }

  /**
   * Load vehicle details
   */
  loadVehicle() {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.vehicleService.getVehicleById(this.vehicleId).subscribe({
      next: (vehicle) => {
        this.vehicle = vehicle;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.cdr.markForCheck();
        console.error('Araç detayları yüklenirken hata oluştu', error);
        if (error.errorMessage && Array.isArray(error.errorMessage)) {
          this.toastService.showErrorMessages(error.errorMessage);
        } else {
          this.toastService.error('Araç detayları yüklenirken hata oluştu');
        }
      },
    });
  }

  /**
   * Get image source
   */
  getImageSource(): string {
    if (!this.vehicle) return '';
    
    if (this.vehicle.imageUrl) {
      return this.vehicle.imageUrl;
    }
    
    if (this.vehicle.imageBase64) {
      return this.vehicle.imageBase64;
    }
    
    // Placeholder image
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjE1MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMjAwIiBoZWlnaHQ9IjE1MCIgZmlsbD0iIzI1MjUyNSIvPjx0ZXh0IHg9IjUwJSIgeT0iNTAlIiBmb250LWZhbWlseT0iQXJpYWwiIGZvbnQtc2l6ZT0iMTQiIGZpbGw9IiM2NjY2NjYiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIj5BcmHDpyBSZXNtaTwvdGV4dD48L3N2Zz4=';
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
}

