import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  Output,
  EventEmitter,
  inject,
  ChangeDetectorRef,
  computed,
  effect,
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
import { LanguageService } from '../../services/language.service';
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
  @Input() rental?: Rental; 
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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  selectedCustomer: Customer | null = null;
  selectedVehicle: Vehicle | null = null;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      customer: this.languageService.translate('modals.rental.customer'),
      customerSelect: this.languageService.translate('modals.rental.customerSelect'),
      customerId: this.languageService.translate('modals.rental.customerId'),
      clearSelection: this.languageService.translate('modals.common.clearSelection'),
      vehicle: this.languageService.translate('modals.rental.vehicle'),
      vehicleSelect: this.languageService.translate('modals.rental.vehicleSelect'),
      vehicleId: this.languageService.translate('modals.rental.vehicleId'),
      startDate: this.languageService.translate('modals.rental.startDate'),
      endDate: this.languageService.translate('modals.rental.endDate'),
      dailyRate: this.languageService.translate('modals.rental.dailyRate'),
      dailyRatePlaceholder: this.languageService.translate('modals.rental.dailyRatePlaceholder'),
      dailyRateHint: this.languageService.translate('modals.rental.dailyRateHint'),
      totalAmount: this.languageService.translate('modals.rental.totalAmount'),
      totalAmountPlaceholder: this.languageService.translate('modals.rental.totalAmountPlaceholder'),
      totalAmountHint: this.languageService.translate('modals.rental.totalAmountHint'),
      actualReturnDate: this.languageService.translate('modals.rental.actualReturnDate'),
      lateFee: this.languageService.translate('modals.rental.lateFee'),
      lateFeePlaceholder: this.languageService.translate('modals.rental.lateFeePlaceholder'),
      damageFee: this.languageService.translate('modals.rental.damageFee'),
      damageFeePlaceholder: this.languageService.translate('modals.rental.damageFeePlaceholder'),
      kmAtStart: this.languageService.translate('modals.rental.kmAtStart'),
      kmAtStartPlaceholder: this.languageService.translate('modals.rental.kmAtStartPlaceholder'),
      kmAtReturn: this.languageService.translate('modals.rental.kmAtReturn'),
      kmAtReturnPlaceholder: this.languageService.translate('modals.rental.kmAtReturnPlaceholder'),
      pickupLocation: this.languageService.translate('modals.rental.pickupLocation'),
      pickupLocationPlaceholder: this.languageService.translate('modals.rental.pickupLocationPlaceholder'),
      returnLocation: this.languageService.translate('modals.rental.returnLocation'),
      returnLocationPlaceholder: this.languageService.translate('modals.rental.returnLocationPlaceholder'),
      notes: this.languageService.translate('modals.rental.notes'),
      notesPlaceholder: this.languageService.translate('modals.rental.notesPlaceholder'),
      cancel: this.languageService.translate('modals.common.cancel'),
      save: this.languageService.translate('modals.common.save'),
      update: this.languageService.translate('modals.common.update'),
      saving: this.languageService.translate('modals.common.saving'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    this.isEditMode = !!this.rental;
    this.initForm();
    if (this.isEditMode && this.rental?.vehicleId) {
      this.loadSelectedVehicle();
    }
  }

  loadSelectedVehicle() {
    if (!this.rental?.vehicleId) return;

    this.vehicleService.getVehicleById(this.rental.vehicleId).subscribe({
      next: (vehicle) => {
        this.selectedVehicle = vehicle;
        this.cdr.markForCheck();
      },
      error: () => {},
    });
  }

  initForm() {
    const rental = this.rental;

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

  formatDateForInput(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

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

  calculateTotalAmount() {
    const startDate = this.rentalForm.get('startDate')?.value;
    const endDate = this.rentalForm.get('endDate')?.value;
    const dailyRate = this.rentalForm.get('dailyRate')?.value;

    if (startDate && endDate && dailyRate && Number(dailyRate) > 0) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      const diffTime = Math.abs(end.getTime() - start.getTime());
      const diffDays = Math.max(1, Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1);
      const total = diffDays * Number(dailyRate);
      this.rentalForm.patchValue({ totalAmount: total }, { emitEvent: false });
      this.cdr.markForCheck();
    } else {
      this.rentalForm.patchValue({ totalAmount: 0 }, { emitEvent: false });
      this.cdr.markForCheck();
    }
  }

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

  get f() {
    return this.rentalForm.controls;
  }

  hasError(field: string, errorType: string): boolean {
    const control = this.rentalForm.get(field);
    return !!(
      control &&
      control.hasError(errorType) &&
      (control.touched || control.dirty)
    );
  }

  getErrorMessage(field: string): string {
    const control = this.rentalForm.get(field);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) return this.languageService.translate('messages.errors.validation.required');
    if (control.hasError('min'))
      return this.languageService.translateWithParams('messages.errors.validation.min', { min: control.errors['min'].min.toString() });
    if (control.hasError('max'))
      return this.languageService.translateWithParams('messages.errors.validation.max', { max: control.errors['max'].max.toString() });
    if (control.hasError('maxlength'))
      return this.languageService.translateWithParams('messages.errors.validation.maxlength', { length: control.errors['maxlength'].requiredLength.toString() });
    if (control.hasError('dateRange'))
      return this.languageService.translate('messages.errors.validation.dateRange');

    return this.languageService.translate('messages.errors.validation.invalid');
  }

  onSubmit() {
    if (!this.validateDateRange()) {
      this.toastService.error(this.languageService.translate('messages.errors.validation.dateRange'));
      return;
    }

    if (this.rentalForm.invalid) {
      Object.keys(this.rentalForm.controls).forEach((key) => {
        this.rentalForm.get(key)?.markAsTouched();
      });
      this.toastService.error(this.languageService.translate('messages.errors.validation.fillAllRequired'));
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.rentalForm.value;

    if (this.isEditMode && this.rental) {

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
          this.toastService.success(this.languageService.translate('messages.success.rental.updated'));
          this.saved.emit(rental);
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
        },
      });
    } else {

      const createCommand: CreateRentalCommand = {
        customerId: formValue.customerId,
        vehicleId: formValue.vehicleId,
        startDate: new Date(formValue.startDate).toISOString(),
        endDate: new Date(formValue.endDate).toISOString(),
        dailyRate: Number(formValue.dailyRate),
        totalAmount: Number(formValue.totalAmount),
        status: RentalStatus.Active,
        notes: formValue.notes?.trim() || undefined,
        pickupLocation: formValue.pickupLocation?.trim() || undefined,
        returnLocation: formValue.returnLocation?.trim() || undefined,
        kmAtStart: formValue.kmAtStart ? Number(formValue.kmAtStart) : undefined,
      };

      this.rentalService.createRental(createCommand).subscribe({
        next: (rental) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success(this.languageService.translate('messages.success.rental.created'));
          this.saved.emit(rental);
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
        },
      });
    }
  }

  onCancel() {
    this.cancelled.emit();
  }

  getVehicleDisplayText(vehicle: Vehicle): string {
    if (vehicle.licensePlate) {
      return `${vehicle.licensePlate} - ${vehicle.brand} ${vehicle.model}`;
    }
    return `${vehicle.brand} ${vehicle.model} (${vehicle.id})`;
  }

  openCustomerSelectModal() {
    const { close, contentRef } = this.modalService.open(CustomerSelectComponent, {
      title: this.languageService.translate('messages.modal.customerSelect'),
      size: 'large',
    });

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

  getSelectedCustomerText(): string {
    if (!this.selectedCustomer) {
      return this.languageService.translate('messages.modal.customerSelect');
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

  clearSelectedCustomer() {
    this.selectedCustomer = null;
    this.rentalForm.patchValue({ customerId: '' });
    this.cdr.markForCheck();
  }

  openVehicleSelectModal() {
    const { close, contentRef } = this.modalService.open(VehicleSelectComponent, {
      title: this.languageService.translate('messages.modal.vehicleSelect'),
      size: 'large',
    });

    if (contentRef && contentRef.instance) {
      const vehicleSelect = contentRef.instance as VehicleSelectComponent;

      const selectedSub = vehicleSelect.vehicleSelected.subscribe(
        (vehicle: Vehicle) => {
          this.selectedVehicle = vehicle;
          this.rentalForm.patchValue({ vehicleId: vehicle.id });
          if (vehicle.dailyRentalPrice) {
            this.rentalForm.patchValue({ 
              dailyRate: vehicle.dailyRentalPrice 
            }, { emitEvent: false });
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

  getSelectedVehicleText(): string {
    if (!this.selectedVehicle) {
      return this.languageService.translate('messages.modal.vehicleSelect');
    }
    return this.getVehicleDisplayText(this.selectedVehicle);
  }

  clearSelectedVehicle() {
    this.selectedVehicle = null;
    this.rentalForm.patchValue({ vehicleId: '' });
    this.cdr.markForCheck();
  }
}
