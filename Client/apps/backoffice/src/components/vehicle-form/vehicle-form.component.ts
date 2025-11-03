import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  ChangeDetectorRef,
  inject,
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
  private cdr = inject(ChangeDetectorRef);

  vehicleForm!: FormGroup;
  isEditMode = false;
  isLoading = false;
  imagePreviewUrl: string | null = null; // For preview purposes

  // Enums
  categories = Object.values(VehicleCategory).filter(
    (v) => typeof v === 'number'
  ) as VehicleCategory[];
  statuses = Object.values(VehicleStatus).filter(
    (v) => typeof v === 'number'
  ) as VehicleStatus[];
  categoryNames = VehicleCategoryNames;
  statusNames = VehicleStatusNames;

  // Fuel types
  fuelTypes = ['Benzin', 'Dizel', 'Elektrik', 'Hibrit', 'LPG'];
  
  // Transmission types
  transmissionTypes = ['Manuel', 'Otomatik', 'Yarı Otomatik'];

  ngOnInit() {
    this.isEditMode = !!this.vehicle;
    this.initForm();
  }

  /**
   * Initialize form
   */
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

  /**
   * Get form control
   */
  get f() {
    return this.vehicleForm.controls;
  }

  /**
   * Check if field has error
   */
  hasError(field: string, errorType: string): boolean {
    const control = this.vehicleForm.get(field);
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
    const control = this.vehicleForm.get(field);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) return 'Bu alan zorunludur';
    if (control.hasError('min')) return `Minimum değer ${control.errors['min'].min} olmalıdır`;
    if (control.hasError('max')) return `Maksimum değer ${control.errors['max'].max} olmalıdır`;
    if (control.hasError('maxlength'))
      return `Maksimum ${control.errors['maxlength'].requiredLength} karakter olmalıdır`;

    return 'Geçersiz değer';
  }

  /**
   * Get image preview URL
   */
  getImagePreview(): string | null {
    // If we have a preview URL from newly selected image, use it
    if (this.imagePreviewUrl) {
      return this.imagePreviewUrl;
    }
    
    // If editing and vehicle has imageUrl, use it
    if (this.vehicle?.imageUrl) {
      return this.vehicle.imageUrl;
    }
    
    // If we have base64, construct data URL
    const imageBase64 = this.vehicleForm.get('imageBase64')?.value;
    if (imageBase64 && imageBase64.trim()) {
      // If it's already a data URL, return as is
      if (imageBase64.startsWith('data:')) {
        return imageBase64;
      }
      // Otherwise, construct data URL (assume JPEG format)
      return `data:image/jpeg;base64,${imageBase64}`;
    }
    
    return null;
  }

  /**
   * Handle file input change
   */
  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file type
      if (!file.type.startsWith('image/')) {
        this.toastService.error('Lütfen bir resim dosyası seçin');
        input.value = '';
        return;
      }
      
      // Validate file size (max 5MB)
      const maxSize = 5 * 1024 * 1024; // 5MB
      if (file.size > maxSize) {
        this.toastService.error('Resim boyutu 5MB\'dan büyük olamaz');
        input.value = '';
        return;
      }
      
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const dataUrl = e.target.result as string;
        // Store data URL for preview
        this.imagePreviewUrl = dataUrl;
        // Extract only base64 part (remove data:image/xxx;base64, prefix)
        const base64 = dataUrl.includes(',') ? dataUrl.split(',')[1] : dataUrl;
        // Store only base64 string in form (without data URL prefix)
        this.vehicleForm.patchValue({ imageBase64: base64 });
        this.cdr.markForCheck();
      };
      reader.onerror = () => {
        this.toastService.error('Resim yüklenirken hata oluştu');
        input.value = '';
        this.imagePreviewUrl = null;
      };
      reader.readAsDataURL(file);
    }
  }

  /**
   * Submit form
   */
  onSubmit() {
    if (this.vehicleForm.invalid) {
      Object.keys(this.vehicleForm.controls).forEach((key) => {
        this.vehicleForm.get(key)?.markAsTouched();
      });
      this.toastService.error('Lütfen tüm zorunlu alanları doldurun');
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    const formValue = this.vehicleForm.value;

    // Prepare command data - only include imageBase64 if it has a value
    const commandData: any = {
      brand: formValue.brand,
      model: formValue.model,
      licensePlate: formValue.licensePlate,
      year: Number(formValue.year),
      color: formValue.color || undefined,
      category: Number(formValue.category), // Ensure category is sent as int
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

    // Only include imageBase64 if it has a value
    if (formValue.imageBase64 && formValue.imageBase64.trim()) {
      commandData.imageBase64 = formValue.imageBase64;
    }

    if (this.isEditMode && this.vehicle) {
      // Update existing vehicle
      const updateCommand: UpdateVehicleCommand = {
        id: this.vehicle.id,
        ...commandData,
      };

      this.vehicleService.updateVehicle(updateCommand).subscribe({
        next: (vehicle) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success('Araç başarıyla güncellendi');
          this.saved.emit(vehicle);
        },
        error: (error) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          // Error is already handled by exception interceptor
        },
      });
    } else {
      // Create new vehicle
      const createCommand: CreateVehicleCommand = commandData;

      this.vehicleService.createVehicle(createCommand).subscribe({
        next: (vehicle) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toastService.success('Araç başarıyla oluşturuldu');
          this.saved.emit(vehicle);
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

}

