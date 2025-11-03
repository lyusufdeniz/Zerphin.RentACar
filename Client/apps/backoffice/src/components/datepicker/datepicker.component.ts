import {
  ChangeDetectionStrategy,
  Component,
  forwardRef,
  Input,
  OnInit,
  OnDestroy,
  inject,
  ChangeDetectorRef,
  ElementRef,
  ViewChild,
  HostListener,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  FormsModule,
} from '@angular/forms';
import { DatepickerService } from '../../services/datepicker.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-datepicker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './datepicker.component.html',
  styleUrls: ['./datepicker.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatepickerComponent),
      multi: true,
    },
  ],
})
export class DatepickerComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() placeholder = 'Tarih seçin';
  @Input() label = '';
  @Input() required = false;
  @Input() disabled = false;
  @Input() min?: string;
  @Input() max?: string;

  @ViewChild('dateInput', { static: false }) dateInput!: ElementRef<HTMLInputElement>;
  @ViewChild('dropdown', { static: false }) dropdown!: ElementRef<HTMLDivElement>;
  @ViewChild('yearGrid', { static: false }) yearGrid!: ElementRef<HTMLDivElement>;

  value: string = '';
  displayValue: string = '';
  isOpen = false;
  showYearPicker = false;
  currentMonth = 0;
  currentYear = 0;
  calendarDays: (number | null)[] = [];
  selectedDate: Date | null = null;
  yearList: number[] = [];
  private datepickerId!: string;
  private datepickerSubscription?: Subscription;

  private cdr = inject(ChangeDetectorRef);
  private datepickerService = inject(DatepickerService);
  private onChange = (value: string) => {};
  
  // Public method for template access
  onTouched = () => {};

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent) {
    if (this.isOpen && this.dropdown && !this.dropdown.nativeElement.contains(event.target as Node)) {
      const target = event.target as HTMLElement;
      if (!target.closest('.datepicker-display') && !target.closest('.datepicker-dropdown')) {
        this.closeCalendar();
      }
    }
  }

  months = [
    'Ocak',
    'Şubat',
    'Mart',
    'Nisan',
    'Mayıs',
    'Haziran',
    'Temmuz',
    'Ağustos',
    'Eylül',
    'Ekim',
    'Kasım',
    'Aralık',
  ];

  weekDays = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];

  ngOnInit() {
    const today = new Date();
    this.currentMonth = today.getMonth();
    this.currentYear = today.getFullYear();
    
    // Generate year list (current year ± 50 years)
    this.generateYearList(this.currentYear);
    
    // Generate unique ID for this datepicker instance
    this.datepickerId = `datepicker-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
    
    // Subscribe to datepicker open events
    this.datepickerSubscription = this.datepickerService.onDatepickerOpen().subscribe((openedId) => {
      if (openedId !== null && openedId !== this.datepickerId && this.isOpen) {
        // Another datepicker was opened, close this one
        this.closeCalendar();
      }
    });
  }

  /**
   * Generate year list for year picker
   */
  generateYearList(centerYear: number) {
    this.yearList = [];
    for (let year = centerYear - 50; year <= centerYear + 50; year++) {
      this.yearList.push(year);
    }
  }

  writeValue(value: string): void {
    this.value = value || '';
    if (this.value) {
      const date = new Date(this.value);
      if (!isNaN(date.getTime())) {
        this.selectedDate = date;
        this.displayValue = this.formatDisplayDate(date);
        this.currentMonth = date.getMonth();
        this.currentYear = date.getFullYear();
      }
    } else {
      this.selectedDate = null;
      this.displayValue = '';
    }
    this.generateCalendar();
    this.cdr.markForCheck();
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
    this.cdr.markForCheck();
  }

  formatDisplayDate(date: Date): string {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  }

  formatValueForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  toggleCalendar() {
    if (this.disabled) return;
    
    if (!this.isOpen) {
      // Opening calendar - notify service to close other datepickers
      this.datepickerService.notifyDatepickerOpened(this.datepickerId);
      this.isOpen = true;
      if (!this.currentYear) {
        const today = new Date();
        this.currentMonth = today.getMonth();
        this.currentYear = today.getFullYear();
        this.generateCalendar();
      }
    } else {
      // Closing calendar
      this.isOpen = false;
      this.datepickerService.notifyDatepickerClosed();
    }
    this.cdr.markForCheck();
  }

  closeCalendar() {
    if (this.isOpen) {
      this.isOpen = false;
      this.showYearPicker = false;
      this.datepickerService.notifyDatepickerClosed();
      this.cdr.markForCheck();
    }
  }

  selectDate(day: number) {
    if (day === null) return;

    const date = new Date(this.currentYear, this.currentMonth, day);
    
    // Check min/max constraints
    if (this.min) {
      const minDate = new Date(this.min);
      if (date < minDate) return;
    }
    if (this.max) {
      const maxDate = new Date(this.max);
      if (date > maxDate) return;
    }

    this.selectedDate = date;
    this.value = this.formatValueForInput(date);
    this.displayValue = this.formatDisplayDate(date);
    this.onChange(this.value);
    this.onTouched();
    this.closeCalendar();
    this.cdr.markForCheck();
  }

  previousMonth() {
    if (this.showYearPicker) {
      // Scroll years if in year picker mode
      const firstYear = this.yearList[0];
      this.generateYearList(firstYear - 20);
      this.cdr.markForCheck();
    } else {
      if (this.currentMonth === 0) {
        this.currentMonth = 11;
        this.currentYear--;
      } else {
        this.currentMonth--;
      }
      this.generateCalendar();
      this.cdr.markForCheck();
    }
  }

  nextMonth() {
    if (this.showYearPicker) {
      // Scroll years if in year picker mode
      const lastYear = this.yearList[this.yearList.length - 1];
      this.generateYearList(lastYear + 20);
      this.cdr.markForCheck();
    } else {
      if (this.currentMonth === 11) {
        this.currentMonth = 0;
        this.currentYear++;
      } else {
        this.currentMonth++;
      }
      this.generateCalendar();
      this.cdr.markForCheck();
    }
  }

  /**
   * Toggle year picker view
   */
  toggleYearPicker() {
    this.showYearPicker = !this.showYearPicker;
    if (this.showYearPicker) {
      // Regenerate year list centered on current year
      this.generateYearList(this.currentYear);
      
      // Scroll to current year after view updates
      setTimeout(() => {
        this.scrollToCurrentYear();
      }, 0);
    }
    this.cdr.markForCheck();
  }

  /**
   * Scroll to current year in year picker
   */
  scrollToCurrentYear() {
    if (this.yearGrid && this.yearGrid.nativeElement) {
      const yearIndex = this.yearList.indexOf(this.currentYear);
      if (yearIndex !== -1) {
        // Calculate which row the current year is in (4 columns)
        const rowIndex = Math.floor(yearIndex / 4);
        const yearItemHeight = 48; // Approximate height including gap
        const scrollPosition = rowIndex * yearItemHeight;
        
        this.yearGrid.nativeElement.scrollTop = Math.max(0, scrollPosition - 60); // Offset for better visibility
      }
    }
  }

  /**
   * Select year from year picker
   */
  selectYear(year: number) {
    this.currentYear = year;
    this.showYearPicker = false;
    this.generateCalendar();
    this.cdr.markForCheck();
  }

  /**
   * Check if year is current year
   */
  isCurrentYear(year: number): boolean {
    const today = new Date();
    return year === today.getFullYear();
  }

  /**
   * Check if year is selected
   */
  isSelectedYear(year: number): boolean {
    return year === this.currentYear;
  }

  generateCalendar() {
    const firstDay = new Date(this.currentYear, this.currentMonth, 1);
    const lastDay = new Date(this.currentYear, this.currentMonth + 1, 0);
    const daysInMonth = lastDay.getDate();
    const startingDayOfWeek = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1; // Monday = 0

    this.calendarDays = [];

    // Add empty cells for days before the first day of the month
    for (let i = 0; i < startingDayOfWeek; i++) {
      this.calendarDays.push(null);
    }

    // Add days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      this.calendarDays.push(day);
    }
  }

  isToday(day: number | null): boolean {
    if (day === null) return false;
    const today = new Date();
    return (
      day === today.getDate() &&
      this.currentMonth === today.getMonth() &&
      this.currentYear === today.getFullYear()
    );
  }

  isSelected(day: number | null): boolean {
    if (day === null || !this.selectedDate) return false;
    return (
      day === this.selectedDate.getDate() &&
      this.currentMonth === this.selectedDate.getMonth() &&
      this.currentYear === this.selectedDate.getFullYear()
    );
  }

  isDisabled(day: number | null): boolean {
    if (day === null) return true;

    const date = new Date(this.currentYear, this.currentMonth, day);

    if (this.min) {
      const minDate = new Date(this.min);
      if (date < minDate) return true;
    }

    if (this.max) {
      const maxDate = new Date(this.max);
      if (date > maxDate) return true;
    }

    return false;
  }

  onInputChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    
    if (value) {
      const date = new Date(value);
      if (!isNaN(date.getTime())) {
        this.selectedDate = date;
        this.displayValue = this.formatDisplayDate(date);
        this.currentMonth = date.getMonth();
        this.currentYear = date.getFullYear();
        this.generateCalendar();
        this.value = value;
        this.onChange(value);
        this.onTouched();
      }
    } else {
      this.selectedDate = null;
      this.displayValue = '';
      this.value = '';
      this.onChange('');
      this.onTouched();
    }
    this.cdr.markForCheck();
  }

  ngOnDestroy() {
    // Unsubscribe from datepicker service
    if (this.datepickerSubscription) {
      this.datepickerSubscription.unsubscribe();
    }
  }
}

