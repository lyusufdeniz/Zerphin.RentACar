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
  Insurance,
  CreateInsuranceCommand,
  UpdateInsuranceCommand,
} from '../../models/insurance';
import { InsuranceService } from '../../services/insurance.service';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { Vehicle } from '../../models/vehicle';

@Component({
  selector: 'app-insurance-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatepickerComponent],
  templateUrl: './insurance-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InsuranceFormComponent implements OnInit {
  @Input() insurance?: Insurance;
  @Output() saved = new EventEmitter<Insurance>();
  @Output() cancelled = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private insuranceService = inject(InsuranceService);
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  insuranceForm!: FormGroup;
  isEditMode = false;
  isLoading = false;
  isLoadingVehicles = false;
  vehicles: Vehicle[] = [];

  ngOnInit() {
    this.isEditMode = !!this.insurance;
    this.loadVehicles();
    this.initForm();
  }

  /**
   * Load vehicles for dropdown
   */
  loadVehicles() {
    this.isLoadingVehicles = true;
    this.cdr.markForCheck();

    this.vehicleService
      .searchVehicles({ PageNumber: 1, PageSize: 1000 })
      .subscribe({
        next: (response) => {
          this.vehicles = response.vehicles || [];
          this.isLoadingVehicles = false;
          this.cdr.markForCheck();
        },
        error: (error) => {
          this.isLoadingVehicles = false;
          this.cdr.markForCheck();
          // Error is already handled by exception interceptor
        },
      });
  }

  /**
   * Initialize form
   */
  initForm() {
    const insurance = this.insurance;

    // Default dates
    const today = new Date();
    const nextYear = new Date();
    nextYear.setFullYear(today.getFullYear() + 1);

    this.insuranceForm = this.fb.group({
      vehicleId: [insurance?.vehicleId || '', Validators.required],
      policyNumber: [
        insurance?.policyNumber || '',
        [Validators.required, Validators.maxLength(100)],
      ],
      insuranceCompany: [
        insurance?.insuranceCompany || '',
        [Validators.required, Validators.maxLength(100)],
      ],
      startDate: [
        insurance?.startDate
          ? this.formatDateForInput(insurance.startDate)
          : this.formatDateForInput(today.toISOString()),
        Validators.required,
      ],
      endDate: [
        insurance?.endDate
          ? this.formatDateForInput(insurance.endDate)
          : this.formatDateForInput(nextYear.toISOString()),
        Validators.required,
      ],
      premiumAmount: [
        insurance?.premiumAmount || 0,
        [Validators.required, Validators.min(0)],
      ],
      coverageType: [insurance?.coverageType || '', Validators.maxLength(100)],
      coverageLimit: [
        insurance?.coverageLimit || null,
        [Validators.min(0)],
      ],
      deductible: [
        insurance?.deductible || null,
        [Validators.min(0)],
      ],
      coverageDetails: [
        insurance?.coverageDetails || '',
        Validators.maxLength(1000),
      ],
      contactInfo: [
        insurance?.contactInfo || '',
        Validators.maxLength(200),
      ],
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
   * Get form control
   */
  get f() {
    return this.insuranceForm.controls;
  }

  /**
   * Check if field has error
   */
  hasError(field: string, errorType: string): boolean {
    const control = this.insuranceForm.get(field);
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
    const control = this.insuranceForm.get(field);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) return 'Bu alan zorunludur';
    if (control.hasError('min')) return `Minimum değer ${control.errors['min'].min} olmalıdır`;
    if (control.hasError('maxlength'))
      return `Maksimum ${control.errors['maxlength'].requiredLength} karakter olmalıdır`;

    return 'Geçersiz değer';
  }

  /**
   * Validate date range
   */
  validateDateRange() {
    const startDate = this.insuranceForm.get('startDate')?.value;
    const endDate = this.insuranceForm.get('endDate')?.value;

    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);

      if (end < start) {
        this.insuranceForm.get('endDate')?.setErrors({ dateRange: true });
        return false;
      } else {
        this.insuranceForm.get('endDate')?.setErrors(null);
      }
    }

    return true;
  }

  /**
   * Submit form
   */
  onSubmit() {
    if (this.insuranceForm.invalid) {
      Object.keys(this.insuranceForm.controls).forEach((key) => {
        this.insuranceForm.get(key)?.markAsTouched();
      });
      this.toastService.error('Lütfen tüm zorunlu alanları doldurun');
      return;
    }

    if (!this.validateDateRange()) {
      this.toastService.error('Bitiş tarihi başlangıç tarihinden önce olamaz');
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.insuranceForm.value;

    // Prepare command data
    const commandData: any = {
      vehicleId: formValue.vehicleId,
      insuranceCompany: formValue.insuranceCompany?.trim() || undefined,
      policyNumber: formValue.policyNumber?.trim() || undefined,
      startDate: new Date(formValue.startDate).toISOString(),
      endDate: new Date(formValue.endDate).toISOString(),
      premiumAmount: Number(formValue.premiumAmount),
      coverageType: formValue.coverageType?.trim() || undefined,
      coverageLimit: formValue.coverageLimit ? Number(formValue.coverageLimit) : undefined,
      deductible: formValue.deductible ? Number(formValue.deductible) : undefined,
      coverageDetails: formValue.coverageDetails?.trim() || undefined,
      contactInfo: formValue.contactInfo?.trim() || undefined,
    };

    if (this.isEditMode && this.insurance) {
      // Update existing insurance (vehicleId is not included in UpdateInsuranceCommand)
      const { vehicleId, ...updateData } = commandData;
      const updateCommand: UpdateInsuranceCommand = {
        id: this.insurance.id,
        ...updateData,
      };

      this.insuranceService.updateInsurance(updateCommand).subscribe({
        next: (insurance) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success('Sigorta başarıyla güncellendi');
          this.saved.emit(insurance);
        },
        error: (error) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          // Error is already handled by exception interceptor
        },
      });
    } else {
      // Create new insurance
      const createCommand: CreateInsuranceCommand = commandData;

      this.insuranceService.createInsurance(createCommand).subscribe({
        next: (insurance) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success('Sigorta başarıyla oluşturuldu');
          this.saved.emit(insurance);
        },
        error: (error) => {
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
}

