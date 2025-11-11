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
  Insurance,
  CreateInsuranceCommand,
  UpdateInsuranceCommand,
} from '../../models/insurance';
import { InsuranceService } from '../../services/insurance.service';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { LanguageService } from '../../services/language.service';
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
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  insuranceForm!: FormGroup;
  isEditMode = false;
  isLoading = false;
  isLoadingVehicles = false;
  vehicles: Vehicle[] = [];

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      vehicle: this.languageService.translate('modals.insurance.vehicle'),
      vehicleSelect: this.languageService.translate('modals.insurance.vehicleSelect'),
      loadingVehicles: this.languageService.translate('modals.insurance.loadingVehicles'),
      policyNumber: this.languageService.translate('modals.insurance.policyNumber'),
      policyNumberPlaceholder: this.languageService.translate('modals.insurance.policyNumberPlaceholder'),
      insuranceCompany: this.languageService.translate('modals.insurance.insuranceCompany'),
      insuranceCompanyPlaceholder: this.languageService.translate('modals.insurance.insuranceCompanyPlaceholder'),
      startDate: this.languageService.translate('modals.insurance.startDate'),
      endDate: this.languageService.translate('modals.insurance.endDate'),
      endDateError: this.languageService.translate('modals.insurance.endDateError'),
      premiumAmount: this.languageService.translate('modals.insurance.premiumAmount'),
      premiumAmountPlaceholder: this.languageService.translate('modals.insurance.premiumAmountPlaceholder'),
      coverageType: this.languageService.translate('modals.insurance.coverageType'),
      coverageTypePlaceholder: this.languageService.translate('modals.insurance.coverageTypePlaceholder'),
      coverageLimit: this.languageService.translate('modals.insurance.coverageLimit'),
      coverageLimitPlaceholder: this.languageService.translate('modals.insurance.coverageLimitPlaceholder'),
      deductible: this.languageService.translate('modals.insurance.deductible'),
      deductiblePlaceholder: this.languageService.translate('modals.insurance.deductiblePlaceholder'),
      coverageDetails: this.languageService.translate('modals.insurance.coverageDetails'),
      coverageDetailsPlaceholder: this.languageService.translate('modals.insurance.coverageDetailsPlaceholder'),
      contactInfo: this.languageService.translate('modals.insurance.contactInfo'),
      contactInfoPlaceholder: this.languageService.translate('modals.insurance.contactInfoPlaceholder'),
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
    this.isEditMode = !!this.insurance;
    this.loadVehicles();
    this.initForm();
  }

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
        },
      });
  }

  initForm() {
    const insurance = this.insurance;

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

  formatDateForInput(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  get f() {
    return this.insuranceForm.controls;
  }

  hasError(field: string, errorType: string): boolean {
    const control = this.insuranceForm.get(field);
    return !!(
      control &&
      control.hasError(errorType) &&
      (control.touched || control.dirty)
    );
  }

  getErrorMessage(field: string): string {
    const control = this.insuranceForm.get(field);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) return this.languageService.translate('messages.errors.validation.required');
    if (control.hasError('min')) return this.languageService.translateWithParams('messages.errors.validation.min', { min: control.errors['min'].min.toString() });
    if (control.hasError('maxlength'))
      return this.languageService.translateWithParams('messages.errors.validation.maxlength', { length: control.errors['maxlength'].requiredLength.toString() });

    return this.languageService.translate('messages.errors.validation.invalid');
  }

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

  onSubmit() {
    if (this.insuranceForm.invalid) {
      Object.keys(this.insuranceForm.controls).forEach((key) => {
        this.insuranceForm.get(key)?.markAsTouched();
      });
      this.toastService.error(this.languageService.translate('messages.errors.validation.fillAllRequired'));
      return;
    }

    if (!this.validateDateRange()) {
      this.toastService.error(this.languageService.translate('messages.errors.validation.dateRange'));
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.insuranceForm.value;

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
      const { vehicleId, ...updateData } = commandData;
      const updateCommand: UpdateInsuranceCommand = {
        id: this.insurance.id,
        ...updateData,
      };

      this.insuranceService.updateInsurance(updateCommand).subscribe({
        next: (insurance) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success(this.languageService.translate('messages.success.insurance.updated'));
          this.saved.emit(insurance);
        },
        error: (error) => {
          this.isLoading = false;
          this.cdr.markForCheck();
        },
      });
    } else {
      const createCommand: CreateInsuranceCommand = commandData;

      this.insuranceService.createInsurance(createCommand).subscribe({
        next: (insurance) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success(this.languageService.translate('messages.success.insurance.created'));
          this.saved.emit(insurance);
        },
        error: (error) => {
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
}
