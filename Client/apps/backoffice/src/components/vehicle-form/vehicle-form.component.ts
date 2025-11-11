import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  ChangeDetectorRef,
  inject,
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
import {
  Vehicle,
  VehicleCategory,
  VehicleStatus,
  VehicleCategoryNames,
  VehicleStatusNames,
  CreateVehicleCommand,
  UpdateVehicleCommand,
} from '../../models/vehicle';
import { VehicleService } from '../../services/vehicle.service';
import { ToastService } from '../../services/toast.service';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-vehicle-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './vehicle-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VehicleFormComponent implements OnInit {
  @Input() vehicle?: Vehicle;
  @Output() saved = new EventEmitter<Vehicle>();
  @Output() cancelled = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private vehicleService = inject(VehicleService);
  private toastService = inject(ToastService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  vehicleForm!: FormGroup;
  isEditMode = false;
  isLoading = false;
  imagePreviewUrl: string | null = null;

  categories = Object.values(VehicleCategory).filter(
    (v) => typeof v === 'number'
  ) as VehicleCategory[];
  statuses = Object.values(VehicleStatus).filter(
    (v) => typeof v === 'number'
  ) as VehicleStatus[];
  categoryNames = VehicleCategoryNames;
  statusNames = VehicleStatusNames;

  translations = computed(() => {
    const _ = this.languageService.currentLanguage();
    return {
      brand: this.languageService.translate('modals.vehicle.brand'),
      brandPlaceholder: this.languageService.translate('modals.vehicle.brandPlaceholder'),
      model: this.languageService.translate('modals.vehicle.model'),
      modelPlaceholder: this.languageService.translate('modals.vehicle.modelPlaceholder'),
      licensePlate: this.languageService.translate('modals.vehicle.licensePlate'),
      licensePlatePlaceholder: this.languageService.translate('modals.vehicle.licensePlatePlaceholder'),
      year: this.languageService.translate('modals.vehicle.year'),
      yearPlaceholder: this.languageService.translate('modals.vehicle.yearPlaceholder'),
      color: this.languageService.translate('modals.vehicle.color'),
      colorPlaceholder: this.languageService.translate('modals.vehicle.colorPlaceholder'),
      category: this.languageService.translate('modals.vehicle.category'),
      dailyRentalPrice: this.languageService.translate('modals.vehicle.dailyRentalPrice'),
      dailyRentalPricePlaceholder: this.languageService.translate('modals.vehicle.dailyRentalPricePlaceholder'),
      seatingCapacity: this.languageService.translate('modals.vehicle.seatingCapacity'),
      seatingCapacityPlaceholder: this.languageService.translate('modals.vehicle.seatingCapacityPlaceholder'),
      fuelType: this.languageService.translate('modals.vehicle.fuelType'),
      fuelTypePlaceholder: this.languageService.translate('modals.vehicle.fuelTypePlaceholder'),
      transmission: this.languageService.translate('modals.vehicle.transmission'),
      transmissionPlaceholder: this.languageService.translate('modals.vehicle.transmissionPlaceholder'),
      km: this.languageService.translate('modals.vehicle.km'),
      kmPlaceholder: this.languageService.translate('modals.vehicle.kmPlaceholder'),
      description: this.languageService.translate('modals.vehicle.description'),
      descriptionPlaceholder: this.languageService.translate('modals.vehicle.descriptionPlaceholder'),
      image: this.languageService.translate('modals.vehicle.image'),
      imageHint: this.languageService.translate('modals.vehicle.imageHint'),
      hasAirConditioning: this.languageService.translate('modals.vehicle.hasAirConditioning'),
      hasGPS: this.languageService.translate('modals.vehicle.hasGPS'),
      select: this.languageService.translate('modals.vehicle.select'),
      features: this.languageService.translate('modals.vehicle.features'),
      bluetooth: this.languageService.translate('modals.vehicle.bluetooth'),
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

  fuelTypes = ['gasoline', 'diesel', 'electric', 'hybrid', 'lpg'];
  transmissionTypes = ['manual', 'automatic', 'semiAutomatic'];

  ngOnInit() {
    this.isEditMode = !!this.vehicle;
    this.initForm();
  }

  initForm() {
    const vehicle = this.vehicle;

    this.vehicleForm = this.fb.group({
      brand: [vehicle?.brand || '', [Validators.required, Validators.maxLength(50)]],
      model: [vehicle?.model || '', [Validators.required, Validators.maxLength(50)]],
      licensePlate: [
        vehicle?.licensePlate || '',
        [Validators.required, Validators.maxLength(20)],
      ],
      year: [
        vehicle?.year || new Date().getFullYear(),
        [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)],
      ],
      color: [vehicle?.color || '', Validators.maxLength(30)],
      category: [
        vehicle?.category || VehicleCategory.Hatchback,
        Validators.required,
      ],
      dailyRentalPrice: [
        vehicle?.dailyRentalPrice || 0,
        [Validators.required, Validators.min(0)],
      ],
      seatingCapacity: [
        vehicle?.seatingCapacity || 5,
        [Validators.required, Validators.min(1), Validators.max(50)],
      ],
      fuelType: [vehicle?.fuelType || '', Validators.maxLength(30)],
      transmission: [vehicle?.transmission || '', Validators.maxLength(30)],
      km: [vehicle?.km || 0, [Validators.min(0)]],
      description: [vehicle?.description || '', Validators.maxLength(1000)],
      imageBase64: [vehicle?.imageBase64 || ''],
      hasAirConditioning: [vehicle?.hasAirConditioning ?? true],
      hasGPS: [vehicle?.hasGPS ?? false],
      hasBluetooth: [vehicle?.hasBluetooth ?? false],
    });

    this.cdr.markForCheck();
  }

  get f() {
    return this.vehicleForm.controls;
  }

  hasError(field: string, errorType: string): boolean {
    const control = this.vehicleForm.get(field);
    return !!(
      control &&
      control.hasError(errorType) &&
      (control.touched || control.dirty)
    );
  }

  getErrorMessage(field: string): string {
    const control = this.vehicleForm.get(field);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) return this.languageService.translate('messages.errors.validation.required');
    if (control.hasError('min')) return this.languageService.translateWithParams('messages.errors.validation.min', { min: control.errors['min'].min.toString() });
    if (control.hasError('max')) return this.languageService.translateWithParams('messages.errors.validation.max', { max: control.errors['max'].max.toString() });
    if (control.hasError('maxlength'))
      return this.languageService.translateWithParams('messages.errors.validation.maxlength', { length: control.errors['maxlength'].requiredLength.toString() });

    return this.languageService.translate('messages.errors.validation.invalid');
  }

  getImagePreview(): string | null {
    if (this.imagePreviewUrl) {
      return this.imagePreviewUrl;
    }

    if (this.vehicle?.imageUrl) {
      return this.vehicle.imageUrl;
    }

    const imageBase64 = this.vehicleForm.get('imageBase64')?.value;
    if (imageBase64 && imageBase64.trim()) {
      if (imageBase64.startsWith('data:')) {
        return imageBase64;
      }
      return `data:image/jpeg;base64,${imageBase64}`;
    }

    return null;
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];

      if (!file.type.startsWith('image/')) {
        this.toastService.error(this.languageService.translate('messages.errors.validation.imageRequired'));
        input.value = '';
        return;
      }

      const maxSize = 5 * 1024 * 1024;
      if (file.size > maxSize) {
        this.toastService.error(this.languageService.translate('messages.errors.validation.imageSize'));
        input.value = '';
        return;
      }

      const reader = new FileReader();
      reader.onload = (e: any) => {
        const dataUrl = e.target.result as string;
        this.imagePreviewUrl = dataUrl;
        const base64 = dataUrl.includes(',') ? dataUrl.split(',')[1] : dataUrl;
        this.vehicleForm.patchValue({ imageBase64: base64 });
        this.cdr.markForCheck();
      };
      reader.onerror = () => {
        this.toastService.error(this.languageService.translate('messages.errors.validation.imageUpload'));
        input.value = '';
        this.imagePreviewUrl = null;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    if (this.vehicleForm.invalid) {
      Object.keys(this.vehicleForm.controls).forEach((key) => {
        this.vehicleForm.get(key)?.markAsTouched();
      });
      this.toastService.error(this.languageService.translate('messages.errors.validation.fillAllRequired'));
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.vehicleForm.value;

    const commandData: any = {
      brand: formValue.brand,
      model: formValue.model,
      licensePlate: formValue.licensePlate,
      year: Number(formValue.year),
      color: formValue.color || undefined,
      category: Number(formValue.category),
      dailyRentalPrice: Number(formValue.dailyRentalPrice),
      seatingCapacity: Number(formValue.seatingCapacity),
      fuelType: formValue.fuelType || undefined,
      transmission: formValue.transmission || undefined,
      km: formValue.km ? Number(formValue.km) : undefined,
      description: formValue.description || undefined,
      hasAirConditioning: Boolean(formValue.hasAirConditioning),
      hasGPS: Boolean(formValue.hasGPS),
      hasBluetooth: Boolean(formValue.hasBluetooth),
    };

    if (formValue.imageBase64 && formValue.imageBase64.trim()) {
      commandData.imageBase64 = formValue.imageBase64;
    }

    if (this.isEditMode && this.vehicle) {
      const updateCommand: UpdateVehicleCommand = {
        id: this.vehicle.id,
        ...commandData,
      };

      this.vehicleService.updateVehicle(updateCommand).subscribe({
        next: (vehicle) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success(this.languageService.translate('messages.success.vehicle.updated'));
          this.saved.emit(vehicle);
        },
        error: (error) => {
          this.isLoading = false;
          this.cdr.markForCheck();
        },
      });
    } else {
      const createCommand: CreateVehicleCommand = commandData;

      this.vehicleService.createVehicle(createCommand).subscribe({
        next: (vehicle) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success(this.languageService.translate('messages.success.vehicle.created'));
          this.saved.emit(vehicle);
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

  getFuelTypeLabel(fuelType: string): string {
    return this.languageService.translate(`messages.fuelTypes.${fuelType}`);
  }

  getTransmissionTypeLabel(transmissionType: string): string {
    return this.languageService.translate(`messages.transmissionTypes.${transmissionType}`);
  }

}
