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
  Rental,
  RentalStatus,
  RentalStatusNames,
} from '../../models/rental';
import { RentalService } from '../../services/rental.service';
import { ToastService } from '../../services/toast.service';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle, VehicleCategoryNames, VehicleCategory } from '../../models/vehicle';
import { LanguageService } from '../../services/language.service';

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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  rental: Rental | null = null;
  vehicle: Vehicle | null = null;
  isLoading = true;

  statusNames = RentalStatusNames;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      loading: this.languageService.translate('messages.details.rental.loading'),
      notFound: this.languageService.translate('messages.details.notFound.rental'),
      rentalInfo: this.languageService.translate('messages.details.rental.rentalInfo'),
      status: this.languageService.translate('messages.details.rental.status'),
      customerName: this.languageService.translate('messages.details.rental.customerName'),
      customerEmail: this.languageService.translate('messages.details.rental.customerEmail'),
      customerPhone: this.languageService.translate('messages.details.rental.customerPhone'),
      vehicleInfo: this.languageService.translate('messages.details.rental.vehicleInfo'),
      licensePlate: this.languageService.translate('messages.details.rental.licensePlate'),
      brandModel: this.languageService.translate('messages.details.rental.brandModel'),
      year: this.languageService.translate('messages.details.rental.year'),
      color: this.languageService.translate('messages.details.rental.color'),
      category: this.languageService.translate('messages.details.rental.category'),
      fuelType: this.languageService.translate('messages.details.rental.fuelType'),
      transmission: this.languageService.translate('messages.details.rental.transmission'),
      seatingCapacity: this.languageService.translate('messages.details.rental.seatingCapacity'),
      person: this.languageService.translate('messages.details.rental.person'),
      km: this.languageService.translate('messages.details.rental.km'),
      kmUnit: this.languageService.translate('messages.details.rental.kmUnit'),
      dateInfo: this.languageService.translate('messages.details.rental.dateInfo'),
      startDate: this.languageService.translate('messages.details.rental.startDate'),
      endDate: this.languageService.translate('messages.details.rental.endDate'),
      actualReturnDate: this.languageService.translate('messages.details.rental.actualReturnDate'),
      createdAt: this.languageService.translate('messages.details.rental.createdAt'),
      updatedAt: this.languageService.translate('messages.details.rental.updatedAt'),
      financialInfo: this.languageService.translate('messages.details.rental.financialInfo'),
      dailyPrice: this.languageService.translate('messages.details.rental.dailyPrice'),
      totalAmount: this.languageService.translate('messages.details.rental.totalAmount'),
      lateFee: this.languageService.translate('messages.details.rental.lateFee'),
      damageFee: this.languageService.translate('messages.details.rental.damageFee'),
      vatIncluded: this.languageService.translate('messages.details.rental.vatIncluded'),
      locationInfo: this.languageService.translate('messages.details.rental.locationInfo'),
      pickupLocation: this.languageService.translate('messages.details.rental.pickupLocation'),
      returnLocation: this.languageService.translate('messages.details.rental.returnLocation'),
      kmInfo: this.languageService.translate('messages.details.rental.kmInfo'),
      kmAtStart: this.languageService.translate('messages.details.rental.kmAtStart'),
      kmAtReturn: this.languageService.translate('messages.details.rental.kmAtReturn'),
      kmUsed: this.languageService.translate('messages.details.rental.kmUsed'),
      notes: this.languageService.translate('messages.details.rental.notes'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    if (this.rentalId) {
      this.loadRental();
    }
  }

  loadRental() {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.rentalService.getRentalById(this.rentalId).subscribe({
      next: (rental) => {
        this.rental = rental;
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
      },
    });
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return date.toLocaleDateString(locale, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  formatDateTime(dateString: string | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return date.toLocaleString(locale, {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  formatCurrency(amount: number | undefined): string {
    if (amount === undefined || amount === null) return '-';
    const locale = this.languageService.isTurkish() ? 'tr-TR' : 'en-US';
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency: 'TRY',
    }).format(amount);
  }

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
      },
    });
  }

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

  getVehicleDisplayName(): string {
    if (!this.vehicle) return '';
    return `${this.vehicle.brand} ${this.vehicle.model}`;
  }

  getCategoryName(category: number): string {
    return VehicleCategoryNames[category as VehicleCategory] || '';
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
}