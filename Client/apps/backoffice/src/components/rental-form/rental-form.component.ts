import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  Output,
  EventEmitter,
  inject,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { DatepickerComponent } from '../datepicker/datepicker.component';
import {
  Rental,
  RentalStatus,
  RentalStatusNames,
  CreateRentalCommand,
  UpdateRentalCommand,
} from '../../models/rental';
import { RentalService } from '../../services/rental.service';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { ModalService } from '../../services/modal.service';
import { CustomerSelectComponent } from '../customer-select/customer-select.component';
import { VehicleSelectComponent } from '../vehicle-select/vehicle-select.component';
import { Vehicle } from '../../models/vehicle';
import { Customer } from '../../models/customer';

@Component({
  selector: 'app-rental-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CustomerSelectComponent, VehicleSelectComponent, DatepickerComponent],
  templateUrl: './rental-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RentalFormComponent implements OnInit {
  @Input() rental?: Rental; // Input for edit mode
  @Output() saved = new EventEmitter<Rental>();
  @Output() cancelled = new EventEmitter<void>();

  rentalForm!: FormGroup;
  isEditMode = false;
  isLoading = false;

  rentalStatuses = Object.values(RentalStatus).filter(
    (value) => typeof value === 'number'
  ) as RentalStatus[];
  rentalStatusNames = RentalStatusNames;

  private fb = inject(FormBuilder);
  private rentalService = inject(RentalService);
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private modalService = inject(ModalService);
  private cdr = inject(ChangeDetectorRef);

  selectedCustomer: Customer | null = null;
  selectedVehicle: Vehicle | null = null;

  ngOnInit() {
    this.isEditMode = !!this.rental;
    this.initForm();
    if (this.isEditMode && this.rental?.vehicleId) {
      this.loadSelectedVehicle();
    }
  }

  /**
   * Load selected vehicle for edit mode
   */
  loadSelectedVehicle() {
    if (!this.rental?.vehicleId) return;
    
    this.vehicleService.getVehicleById(this.rental.vehicleId).subscribe({
      next: (vehicle) => {
        this.selectedVehicle = vehicle;
        this.cdr.markForCheck();
      },
      error: () => {
        // Error is already handled by exception interceptor
      },
    });
  }

  /**
   * Initialize form
   */
  initForm() {
    const rental = this.rental;

    // Default dates
    const today = new Date();
    const nextWeek = new Date();
    nextWeek.setDate(today.getDate() + 7);

    this.rentalForm = this.fb.group({
      customerId: [
        rental?.customerId || '',
        [Validators.required],
      ],
      vehicleId: [
        rental?.vehicleId || '',
        [Validators.required],
      ],
      startDate: [
        rental?.startDate
          ? this.formatDateForInput(rental.startDate)
          : this.formatDateForInput(today.toISOString()),
        Validators.required,
      ],
      endDate: [
        rental?.endDate
          ? this.formatDateForInput(rental.endDate)
          : this.formatDateForInput(nextWeek.toISOString()),
        Validators.required,
      ],
      dailyRate: [
        rental?.dailyRate || 0,
        [Validators.required, Validators.min(0)],
      ],
      totalAmount: [
        rental?.totalAmount || 0,
        [Validators.required, Validators.min(0)],
      ],
      // Status is set automatically to Active when creating, no need for form control
      notes: [
        rental?.notes || '',
        Validators.maxLength(1000),
      ],
      pickupLocation: [
        rental?.pickupLocation || '',
        Validators.maxLength(200),
      ],
      returnLocation: [
        rental?.returnLocation || '',
        Validators.maxLength(200),
      ],
      kmAtStart: [
        rental?.kmAtStart || null,
        [Validators.min(0)],
      ],
      actualReturnDate: [
        rental?.actualReturnDate
          ? this.formatDateTimeForInput(rental.actualReturnDate)
          : null,
      ],
      lateFee: [
        rental?.lateFee || null,
        [Validators.min(0)],
      ],
      damageFee: [
        rental?.damageFee || null,
        [Validators.min(0)],
      ],
      kmAtReturn: [
        rental?.kmAtReturn || null,
        [Validators.min(0)],
      ],
    });

    // Calculate total amount when dates or daily rate change
    this.rentalForm.get('startDate')?.valueChanges.subscribe(() => {
      this.calculateTotalAmount();
      this.validateDateRange();
    });
    this.rentalForm.get('endDate')?.valueChanges.subscribe(() => {
      this.calculateTotalAmount();
      this.validateDateRange();
    });
    this.rentalForm.get('dailyRate')?.valueChanges.subscribe(() => {
      this.calculateTotalAmount();
    });

    this.cdr.markForCheck();
  }

  /**
   * Format date for input (YYYY-MM-DD)
   */
  formatDateForInput(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  /**
   * Format date-time for input (YYYY-MM-DDTHH:mm)
   */
  formatDateTimeForInput(dateString: string | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  /**
   * Calculate total amount based on dates and daily rate
   */
  calculateTotalAmount() {
    const startDate = this.rentalForm.get('startDate')?.value;
    const endDate = this.rentalForm.get('endDate')?.value;
    const dailyRate = this.rentalForm.get('dailyRate')?.value;

    if (startDate && endDate && dailyRate && Number(dailyRate) > 0) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      // Calculate days (end date inclusive, so add 1 day)
      const diffTime = Math.abs(end.getTime() - start.getTime());
      const diffDays = Math.max(1, Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1);
      const total = diffDays * Number(dailyRate);
      this.rentalForm.patchValue({ totalAmount: total }, { emitEvent: false });
      this.cdr.markForCheck();
    } else {
      // Reset to 0 if missing required fields
      this.rentalForm.patchValue({ totalAmount: 0 }, { emitEvent: false });
      this.cdr.markForCheck();
    }
  }

  /**
   * Validate date range
   */
  validateDateRange() {
    const startDate = this.rentalForm.get('startDate')?.value;
    const endDate = this.rentalForm.get('endDate')?.value;

    if (startDate && endDate && new Date(endDate) < new Date(startDate)) {
      this.rentalForm.get('endDate')?.setErrors({ dateRange: true });
      return false;
    } else {
      this.rentalForm.get('endDate')?.setErrors(null);
      return true;
    }
  }

  /**
   * Get form control
   */
  get f() {
    return this.rentalForm.controls;
  }

  /**
   * Check if field has error
   */
  hasError(field: string, errorType: string): boolean {
    const control = this.rentalForm.get(field);
    return !!(
      control &&
      control.hasError(errorType) &&
      (control.touched || control.dirty)
    );
  }

  /**
   * Get error message
   */
  getErrorMessage(field: string): string {
    const control = this.rentalForm.get(field);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) return 'Bu alan zorunludur';
    if (control.hasError('min'))
      return `Minimum değer ${control.errors['min'].min} olmalıdır`;
    if (control.hasError('max'))
      return `Maksimum değer ${control.errors['max'].max} olmalıdır`;
    if (control.hasError('maxlength'))
      return `Maksimum ${control.errors['maxlength'].requiredLength} karakter olmalıdır`;
    if (control.hasError('dateRange'))
      return 'Bitiş tarihi başlangıç tarihinden önce olamaz';

    return 'Geçersiz değer';
  }

  /**
   * Submit form
   */
  onSubmit() {
    if (!this.validateDateRange()) {
      this.toastService.error('Bitiş tarihi başlangıç tarihinden önce olamaz');
      return;
    }

    if (this.rentalForm.invalid) {
      Object.keys(this.rentalForm.controls).forEach((key) => {
        this.rentalForm.get(key)?.markAsTouched();
      });
      this.toastService.error('Lütfen tüm zorunlu alanları doldurun');
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.rentalForm.value;

    if (this.isEditMode && this.rental) {
      // Update existing rental
      const updateCommand: UpdateRentalCommand = {
        id: this.rental.id,
        startDate: new Date(formValue.startDate).toISOString(),
        endDate: new Date(formValue.endDate).toISOString(),
        actualReturnDate: formValue.actualReturnDate
          ? new Date(formValue.actualReturnDate).toISOString()
          : undefined,
        dailyRate: Number(formValue.dailyRate),
        totalAmount: Number(formValue.totalAmount),
        lateFee: formValue.lateFee ? Number(formValue.lateFee) : undefined,
        damageFee: formValue.damageFee ? Number(formValue.damageFee) : undefined,
        notes: formValue.notes?.trim() || undefined,
        pickupLocation: formValue.pickupLocation?.trim() || undefined,
        returnLocation: formValue.returnLocation?.trim() || undefined,
        kmAtStart: formValue.kmAtStart ? Number(formValue.kmAtStart) : undefined,
        kmAtReturn: formValue.kmAtReturn ? Number(formValue.kmAtReturn) : undefined,
      };

      this.rentalService.updateRental(updateCommand).subscribe({
        next: (rental) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success('Kiralama başarıyla güncellendi');
          this.saved.emit(rental);
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
          // Error is already handled by exception interceptor
        },
      });
    } else {
      // Create new rental
      const createCommand: CreateRentalCommand = {
        customerId: formValue.customerId,
        vehicleId: formValue.vehicleId,
        startDate: new Date(formValue.startDate).toISOString(),
        endDate: new Date(formValue.endDate).toISOString(),
        dailyRate: Number(formValue.dailyRate),
        totalAmount: Number(formValue.totalAmount),
        status: RentalStatus.Active, // Status otomatik olarak Active olarak ayarlanır
        notes: formValue.notes?.trim() || undefined,
        pickupLocation: formValue.pickupLocation?.trim() || undefined,
        returnLocation: formValue.returnLocation?.trim() || undefined,
        kmAtStart: formValue.kmAtStart ? Number(formValue.kmAtStart) : undefined,
      };

      this.rentalService.createRental(createCommand).subscribe({
        next: (rental) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success('Kiralama başarıyla oluşturuldu');
          this.saved.emit(rental);
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
          // Error is already handled by exception interceptor
        },
      });
    }
  }

  /**
   * Cancel form
   */
  onCancel() {
    this.cancelled.emit();
  }

  /**
   * Get vehicle display text
   */
  getVehicleDisplayText(vehicle: Vehicle): string {
    if (vehicle.licensePlate) {
      return `${vehicle.licensePlate} - ${vehicle.brand} ${vehicle.model}`;
    }
    return `${vehicle.brand} ${vehicle.model} (${vehicle.id})`;
  }

  /**
   * Open customer select modal
   */
  openCustomerSelectModal() {
    const { close, contentRef } = this.modalService.open(CustomerSelectComponent, {
      title: 'Müşteri Seç',
      size: 'large',
    });

    // Listen for customer selected event
    if (contentRef && contentRef.instance) {
      const customerSelect = contentRef.instance as CustomerSelectComponent;

      const selectedSub = customerSelect.customerSelected.subscribe(
        (customer: Customer) => {
          this.selectedCustomer = customer;
          this.rentalForm.patchValue({ customerId: customer.id });
          this.cdr.markForCheck();
          selectedSub.unsubscribe();
          close();
        }
      );
    }
  }

  /**
   * Get selected customer display text
   */
  getSelectedCustomerText(): string {
    if (!this.selectedCustomer) {
      return 'Müşteri Seç';
    }

    if (
      this.selectedCustomer.userFirstName &&
      this.selectedCustomer.userLastName
    ) {
      return `${this.selectedCustomer.userFirstName} ${this.selectedCustomer.userLastName}`;
    }
    if (this.selectedCustomer.userName) {
      return this.selectedCustomer.userName;
    }
    if (this.selectedCustomer.userEmail) {
      return this.selectedCustomer.userEmail;
    }
    return this.selectedCustomer.id;
  }

  /**
   * Clear selected customer
   */
  clearSelectedCustomer() {
    this.selectedCustomer = null;
    this.rentalForm.patchValue({ customerId: '' });
    this.cdr.markForCheck();
  }

  /**
   * Open vehicle select modal
   */
  openVehicleSelectModal() {
    const { close, contentRef } = this.modalService.open(VehicleSelectComponent, {
      title: 'Araç Seç',
      size: 'large',
    });

    // Listen for vehicle selected event
    if (contentRef && contentRef.instance) {
      const vehicleSelect = contentRef.instance as VehicleSelectComponent;

      const selectedSub = vehicleSelect.vehicleSelected.subscribe(
        (vehicle: Vehicle) => {
          this.selectedVehicle = vehicle;
          this.rentalForm.patchValue({ vehicleId: vehicle.id });
          // Auto-fill daily rate if vehicle has one and calculate total
          if (vehicle.dailyRentalPrice) {
            this.rentalForm.patchValue({ 
              dailyRate: vehicle.dailyRentalPrice 
            }, { emitEvent: false });
            // Force recalculation after patchValue
            setTimeout(() => {
              this.calculateTotalAmount();
            }, 0);
          }
          this.cdr.markForCheck();
          selectedSub.unsubscribe();
          close();
        }
      );
    }
  }

  /**
   * Get selected vehicle display text
   */
  getSelectedVehicleText(): string {
    if (!this.selectedVehicle) {
      return 'Araç Seç';
    }
    return this.getVehicleDisplayText(this.selectedVehicle);
  }

  /**
   * Clear selected vehicle
   */
  clearSelectedVehicle() {
    this.selectedVehicle = null;
    this.rentalForm.patchValue({ vehicleId: '' });
    this.cdr.markForCheck();
  }
}

