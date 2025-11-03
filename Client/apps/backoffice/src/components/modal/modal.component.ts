import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  EventEmitter,
  HostListener,
  ElementRef,
  OnInit,
  OnDestroy,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';

export type ModalSize = 'small' | 'medium' | 'large' | 'fullscreen';

export interface ModalConfig {
  title?: string;
  size?: ModalSize;
  showCloseButton?: boolean;
  closeOnBackdropClick?: boolean;
  closeOnEscape?: boolean;
}

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ModalComponent implements OnInit, OnDestroy {
  @Input() title?: string;
  @Input() size: ModalSize = 'medium';
  @Input() showCloseButton: boolean = true;
  @Input() closeOnBackdropClick: boolean = false;
  @Input() closeOnEscape: boolean = true;

  @Output() close = new EventEmitter<void>();
  @Output() backdropClick = new EventEmitter<void>();

  private elementRef = inject(ElementRef);

  ngOnInit() {
    // Prevent body scroll when modal is open
    document.body.style.overflow = 'hidden';
    
    // Focus trap - focus first focusable element
    setTimeout(() => {
      const firstFocusable = this.elementRef.nativeElement.querySelector(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
      );
      if (firstFocusable) {
        firstFocusable.focus();
      }
    }, 100);
  }

  ngOnDestroy() {
    // Restore body scroll
    document.body.style.overflow = '';
  }

  /**
   * Handle escape key press
   */
  @HostListener('document:keydown.escape', ['$event'])
  handleEscape(event: Event) {
    if (this.closeOnEscape && event instanceof KeyboardEvent) {
      this.onClose();
    }
  }

  /**
   * Handle backdrop click
   */
  onBackdropClick(event: MouseEvent) {
    if (this.closeOnBackdropClick && event.target === event.currentTarget) {
      this.backdropClick.emit();
      this.onClose();
    }
  }

  /**
   * Close modal
   */
  onClose() {
    this.close.emit();
  }

  /**
   * Get size class
   */
  getSizeClass(): string {
    return `modal-${this.size}`;
  }
}

