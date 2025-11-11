import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  OnDestroy,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';

export type SwalType = 'success' | 'error' | 'warning' | 'info' | 'question';

export interface SwalConfig {
  title?: string;
  text?: string;
  type?: SwalType;
  showCancelButton?: boolean;
}

@Component({
  selector: 'app-swal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './swal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SwalComponent implements OnInit, OnDestroy {
  @Input() title: string = 'Emin misiniz?';
  @Input() text: string = '';
  @Input() type: SwalType = 'warning';
  @Input() showCancelButton: boolean = true;
  @Input() confirmButtonText: string = 'Evet';
  @Input() cancelButtonText: string = 'İptal';
  @Input() confirmButtonColor: string = '#00d084';
  @Input() cancelButtonColor: string = '#666666';

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
  @Output() dismissed = new EventEmitter<void>();

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {

    document.addEventListener('keydown', this.handleEscape);
  }

  ngOnDestroy() {
    document.removeEventListener('keydown', this.handleEscape);
  }

  handleEscape = (event: KeyboardEvent) => {
    if (event.key === 'Escape') {
      this.onDismiss();
    }
  };

  getIconPath(): string {
    switch (this.type) {
      case 'success':
        return 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'error':
        return 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'warning':
        return 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z';
      case 'info':
        return 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'question':
        return 'M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
      default:
        return 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z';
    }
  }

  getIconColor(): string {
    switch (this.type) {
      case 'success':
        return '#00d084';
      case 'error':
        return '#ff4444';
      case 'warning':
        return '#ffa500';
      case 'info':
        return '#00d084';
      case 'question':
        return '#00d084';
      default:
        return '#00d084';
    }
  }

  onConfirm() {
    this.confirmed.emit();
  }

  onCancel() {
    this.cancelled.emit();
  }

  onDismiss() {
    this.dismissed.emit();
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('swal-backdrop')) {
      this.onDismiss();
    }
  }
}
