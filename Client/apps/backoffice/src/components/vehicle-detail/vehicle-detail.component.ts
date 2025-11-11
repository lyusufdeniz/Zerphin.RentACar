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
  Vehicle,
  VehicleCategoryNames,
  VehicleStatusNames,
  VehicleStatus,
} from '../../models/vehicle';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { LanguageService } from '../../services/language.service';

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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  vehicle: Vehicle | null = null;
  isLoading = true;

  categoryNames = VehicleCategoryNames;
  statusNames = VehicleStatusNames;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      loading: this.languageService.translate('messages.details.loading'),
      generalInfo: this.languageService.translate('messages.details.vehicle.generalInfo'),
      brand: this.languageService.translate('messages.details.vehicle.brand'),
      model: this.languageService.translate('messages.details.vehicle.model'),
      year: this.languageService.translate('messages.details.vehicle.year'),
      color: this.languageService.translate('messages.details.vehicle.color'),
      category: this.languageService.translate('messages.details.vehicle.category'),
      status: this.languageService.translate('messages.details.vehicle.status'),
      rentalInfo: this.languageService.translate('messages.details.vehicle.rentalInfo'),
      dailyPrice: this.languageService.translate('messages.details.vehicle.dailyPrice'),
      seatingCapacity: this.languageService.translate('messages.details.vehicle.seatingCapacity'),
      person: this.languageService.translate('messages.details.vehicle.person'),
      technicalInfo: this.languageService.translate('messages.details.vehicle.technicalInfo'),
      fuelType: this.languageService.translate('messages.details.vehicle.fuelType'),
      transmission: this.languageService.translate('messages.details.vehicle.transmission'),
      km: this.languageService.translate('messages.details.vehicle.km'),
      kmUnit: this.languageService.translate('messages.details.vehicle.kmUnit'),
      features: this.languageService.translate('messages.details.vehicle.features'),
      airConditioning: this.languageService.translate('messages.details.vehicle.airConditioning'),
      gps: this.languageService.translate('messages.details.vehicle.gps'),
      bluetooth: this.languageService.translate('messages.details.vehicle.bluetooth'),
      description: this.languageService.translate('messages.details.vehicle.description'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    if (this.vehicleId) {
      this.loadVehicle();
    }
  }

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
        const errorMsg = this.languageService.translate('messages.details.loadingError.vehicle');
        console.error(errorMsg, error);
        if (error.errorMessage && Array.isArray(error.errorMessage)) {
          this.toastService.showErrorMessages(error.errorMessage);
        } else {
          this.toastService.error(errorMsg);
        }
      },
    });
  }

  getImageSource(): string {
    if (!this.vehicle) return '';

    if (this.vehicle.imageUrl) {
      return this.vehicle.imageUrl;
    }

    if (this.vehicle.imageBase64) {
      return this.vehicle.imageBase64;
    }

    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjE1MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMjAwIiBoZWlnaHQ9IjE1MCIgZmlsbD0iIzI1MjUyNSIvPjx0ZXh0IHg9IjUwJSIgeT0iNTAlIiBmb250LWZhbWlseT0iQXJpYWwiIGZvbnQtc2l6ZT0iMTQiIGZpbGw9IiM2NjY2NjYiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIj5BcmHDpyBSZXNtaTwvdGV4dD48L3N2Zz4=';
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
}
