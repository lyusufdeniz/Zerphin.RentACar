import { Injectable } from '@angular/core';
import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
  FormGroup,
  FormControl,
} from '@angular/forms';

export interface ValidationError {
  field: string;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class ValidationService {
  /**
   * Email validator
   */
  emailValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null; // Don't validate empty values (use required for that)
      }
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return emailRegex.test(control.value) ? null : { email: true };
    };
  }

  /**
   * Phone number validator (Turkish format)
   */
  phoneValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const phoneRegex = /^(\+90|0)?[5][0-9]{9}$/;
      return phoneRegex.test(control.value.replace(/\s/g, ''))
        ? null
        : { phone: true };
    };
  }

  /**
   * Turkish identity number validator
   */
  identityNumberValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const identityRegex = /^[1-9][0-9]{10}$/;
      if (!identityRegex.test(control.value)) {
        return { identityNumber: true };
      }
      // TC Kimlik algoritması kontrolü
      const digits = control.value.split('').map(Number);
      const sum1 = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
      const sum2 = digits[1] + digits[3] + digits[5] + digits[7];
      if ((sum1 * 7 - sum2) % 10 !== digits[9]) {
        return { identityNumber: true };
      }
      return null;
    };
  }

  /**
   * Password strength validator
   */
  passwordStrengthValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const value = control.value;
      const errors: ValidationErrors = {};

      if (value.length < 8) {
        errors['minlength'] = { requiredLength: 8, actualLength: value.length };
      }
      if (!/[A-Z]/.test(value)) {
        errors['noUppercase'] = true;
      }
      if (!/[a-z]/.test(value)) {
        errors['noLowercase'] = true;
      }
      if (!/[0-9]/.test(value)) {
        errors['noNumber'] = true;
      }
      if (!/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
        errors['noSpecialChar'] = true;
      }

      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  /**
   * Password match validator (for confirm password fields)
   */
  passwordMatchValidator(passwordControlName: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.parent) {
        return null;
      }
      const password = control.parent.get(passwordControlName);
      if (!password) {
        return null;
      }
      return password.value === control.value
        ? null
        : { passwordMismatch: true };
    };
  }

  /**
   * Date validator (not in future)
   */
  dateNotFutureValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const date = new Date(control.value);
      const today = new Date();
      today.setHours(23, 59, 59, 999);
      return date <= today ? null : { dateFuture: true };
    };
  }

  /**
   * Date range validator
   */
  dateRangeValidator(minDate?: Date, maxDate?: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const date = new Date(control.value);
      const errors: ValidationErrors = {};

      if (minDate && date < minDate) {
        errors['dateBeforeMin'] = { minDate };
      }
      if (maxDate && date > maxDate) {
        errors['dateAfterMax'] = { maxDate };
      }

      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  /**
   * Credit card number validator (Luhn algorithm)
   */
  creditCardValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const value = control.value.replace(/\s/g, '');
      if (!/^\d{13,19}$/.test(value)) {
        return { creditCard: true };
      }

      // Luhn algorithm
      let sum = 0;
      let isEven = false;
      for (let i = value.length - 1; i >= 0; i--) {
        let digit = parseInt(value[i]);
        if (isEven) {
          digit *= 2;
          if (digit > 9) {
            digit -= 9;
          }
        }
        sum += digit;
        isEven = !isEven;
      }
      return sum % 10 === 0 ? null : { creditCard: true };
    };
  }

  /**
   * URL validator
   */
  urlValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      try {
        new URL(control.value);
        return null;
      } catch {
        return { url: true };
      }
    };
  }

  /**
   * Number range validator
   */
  numberRangeValidator(min: number, max: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value === null || control.value === undefined) {
        return null;
      }
      const value = Number(control.value);
      if (isNaN(value)) {
        return { notANumber: true };
      }
      if (value < min || value > max) {
        return { numberRange: { min, max, actual: value } };
      }
      return null;
    };
  }

  /**
   * License plate validator (Turkish format)
   */
  licensePlateValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const plateRegex = /^[0-9]{2}\s?[A-Z]{1,3}\s?[0-9]{2,4}$/;
      return plateRegex.test(control.value.toUpperCase())
        ? null
        : { licensePlate: true };
    };
  }

  /**
   * Get error message for a control
   */
  getErrorMessage(
    control: AbstractControl | null,
    fieldName: string = ''
  ): string {
    if (!control || !control.errors || !control.touched) {
      return '';
    }

    const errors = control.errors;
    const fieldLabel = fieldName || 'Bu alan';

    if (errors['required']) {
      return `${fieldLabel} gereklidir`;
    }
    if (errors['email']) {
      return 'Geçerli bir email adresi giriniz';
    }
    if (errors['phone']) {
      return 'Geçerli bir telefon numarası giriniz (05XX XXX XX XX)';
    }
    if (errors['identityNumber']) {
      return 'Geçerli bir TC Kimlik numarası giriniz';
    }
    if (errors['minlength']) {
      return `${fieldLabel} en az ${errors['minlength'].requiredLength} karakter olmalıdır`;
    }
    if (errors['maxlength']) {
      return `${fieldLabel} en fazla ${errors['maxlength'].requiredLength} karakter olabilir`;
    }
    if (errors['min']) {
      return `${fieldLabel} en az ${errors['min'].min} olmalıdır`;
    }
    if (errors['max']) {
      return `${fieldLabel} en fazla ${errors['max'].max} olabilir`;
    }
    if (errors['passwordMismatch']) {
      return 'Şifreler eşleşmiyor';
    }
    if (errors['noUppercase']) {
      return 'Şifre en az bir büyük harf içermelidir';
    }
    if (errors['noLowercase']) {
      return 'Şifre en az bir küçük harf içermelidir';
    }
    if (errors['noNumber']) {
      return 'Şifre en az bir rakam içermelidir';
    }
    if (errors['noSpecialChar']) {
      return 'Şifre en az bir özel karakter içermelidir';
    }
    if (errors['dateFuture']) {
      return 'Gelecek bir tarih seçilemez';
    }
    if (errors['dateBeforeMin']) {
      return 'Tarih minimum tarihten önce olamaz';
    }
    if (errors['dateAfterMax']) {
      return 'Tarih maksimum tarihten sonra olamaz';
    }
    if (errors['creditCard']) {
      return 'Geçerli bir kredi kartı numarası giriniz';
    }
    if (errors['url']) {
      return 'Geçerli bir URL giriniz';
    }
    if (errors['numberRange']) {
      return `${fieldLabel} ${errors['numberRange'].min} ile ${errors['numberRange'].max} arasında olmalıdır`;
    }
    if (errors['licensePlate']) {
      return 'Geçerli bir plaka numarası giriniz (34 ABC 123)';
    }
    if (errors['notANumber']) {
      return 'Geçerli bir sayı giriniz';
    }

    return `${fieldLabel} geçersiz`;
  }

  /**
   * Get all validation errors from a form
   */
  getFormErrors(form: FormGroup): ValidationError[] {
    const errors: ValidationError[] = [];
    Object.keys(form.controls).forEach((key) => {
      const control = form.get(key);
      if (control && control.errors && (control.touched || control.dirty)) {
        const errorMessage = this.getErrorMessage(control, key);
        if (errorMessage) {
          errors.push({ field: key, message: errorMessage });
        }
      }
    });
    return errors;
  }

  /**
   * Mark all form fields as touched
   */
  markFormGroupTouched(form: FormGroup): void {
    Object.keys(form.controls).forEach((key) => {
      const control = form.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  /**
   * Check if form has any errors
   */
  hasFormErrors(form: FormGroup): boolean {
    return this.getFormErrors(form).length > 0;
  }

  /**
   * Get first error message from form
   */
  getFirstFormError(form: FormGroup): string | null {
    const errors = this.getFormErrors(form);
    return errors.length > 0 ? errors[0].message : null;
  }

  /**
   * Check if control has specific error
   */
  hasError(
    control: AbstractControl | null,
    errorType: string
  ): boolean {
    return (
      !!control &&
      control.hasError(errorType) &&
      (control.dirty || control.touched)
    );
  }

  /**
   * Common field labels
   */
  getFieldLabel(fieldName: string): string {
    const labels: Record<string, string> = {
      email: 'Email',
      password: 'Şifre',
      confirmPassword: 'Şifre Tekrar',
      firstName: 'Ad',
      lastName: 'Soyad',
      phoneNumber: 'Telefon',
      identityNumber: 'TC Kimlik No',
      address: 'Adres',
      birthDate: 'Doğum Tarihi',
      licensePlate: 'Plaka',
      licenseNumber: 'Ehliyet No',
    };
    return labels[fieldName] || fieldName;
  }
}


