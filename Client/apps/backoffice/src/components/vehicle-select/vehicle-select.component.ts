import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ChangeDetectorRef,
  inject,
  Output,
  EventEmitter,
  computed,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle, VehicleSearchParams } from '../../models/vehicle';
import { LanguageService } from '../../services/language.service';

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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  vehicles: Vehicle[] = [];
  isLoading = false;
  searchQuery = '';
  currentPage = 1;
  pageSize = 20;
  totalPages = 0;
  totalCount = 0;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      searchPlaceholder: this.languageService.translate('messages.select.vehicle.searchPlaceholder'),
      searchButton: this.languageService.translate('messages.select.vehicle.searchButton'),
      clearButton: this.languageService.translate('messages.select.vehicle.clearButton'),
      loading: this.languageService.translate('messages.select.vehicle.loading'),
      notFound: this.languageService.translate('messages.select.vehicle.notFound'),
      previous: this.languageService.translate('messages.select.vehicle.previous'),
      next: this.languageService.translate('messages.select.vehicle.next'),
      pageInfo: this.languageService.translateWithParams('messages.select.vehicle.pageInfo', {
        currentPage: this.currentPage,
        totalPages: this.totalPages,
        totalCount: this.totalCount,
      }),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    this.loadVehicles();
  }

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

      },
    });
  }

  search() {
    this.currentPage = 1;
    this.loadVehicles();
  }

  clearSearch() {
    this.searchQuery = '';
    this.currentPage = 1;
    this.loadVehicles();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadVehicles();
    }
  }

  selectVehicle(vehicle: Vehicle) {
    this.vehicleSelected.emit(vehicle);
  }

  getVehicleDisplayName(vehicle: Vehicle): string {
    return `${vehicle.licensePlate} - ${vehicle.brand} ${vehicle.model}`;
  }

  getVehicleDetailsText(vehicle: Vehicle): string {
    const details: string[] = [];
    if (vehicle.year) {
      details.push(`${vehicle.year}`);
    }
    if (vehicle.category) {
      const categoryMap: Record<number, string> = {
        1: this.languageService.translate('messages.select.vehicle.categories.hatchback'),
        2: this.languageService.translate('messages.select.vehicle.categories.sedan'),
        3: this.languageService.translate('messages.select.vehicle.categories.suv'),
        4: this.languageService.translate('messages.select.vehicle.categories.pickup'),
      };
      details.push(categoryMap[vehicle.category] || '');
    }
    if (vehicle.dailyRentalPrice) {
      const priceText = this.languageService.translateWithParams('messages.select.vehicle.pricePerDay', {
        price: vehicle.dailyRentalPrice,
      });
      details.push(priceText);
    }
    if (vehicle.status) {
      const statusMap: Record<number, string> = {
        1: this.languageService.translate('messages.select.vehicle.status.available'),
        2: this.languageService.translate('messages.select.vehicle.status.rented'),
      };
      details.push(statusMap[vehicle.status] || '');
    }
    return details.filter(d => d).join(' • ');
  }

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
