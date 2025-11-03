import {
  Injectable,
  inject,
  ComponentRef,
  ApplicationRef,
  createComponent,
  EnvironmentInjector,
} from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { SwalComponent, SwalConfig, SwalType } from '../components/swal/swal.component';

export interface SwalOptions {
  title?: string;
  text?: string;
  type?: SwalType;
  showCancelButton?: boolean;
  confirmButtonText?: string;
  cancelButtonText?: string;
  confirmButtonColor?: string;
  cancelButtonColor?: string;
  icon?: SwalType;
  showLoaderOnConfirm?: boolean;
}

export interface SwalResult {
  isConfirmed: boolean;
  isDenied?: boolean;
  isDismissed?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class SwalService {
  private appRef = inject(ApplicationRef);
  private injector = inject(EnvironmentInjector);
  private swalContainer?: HTMLElement;

  constructor() {
    this.createContainer();
  }

  private createContainer() {
    this.swalContainer = document.createElement('div');
    this.swalContainer.className = 'swal-container';
    document.body.appendChild(this.swalContainer);
  }

  /**
   * Show confirmation alert
   */
  fire(options: SwalOptions): Observable<SwalResult> {
    return new Observable((observer) => {
      if (!this.swalContainer) {
        this.createContainer();
      }

      const componentRef = createComponent(SwalComponent, {
        environmentInjector: this.injector,
      });

      // Configure swal
      componentRef.setInput('title', options.title || 'Emin misiniz?');
      componentRef.setInput('text', options.text || '');
      componentRef.setInput('type', options.type || options.icon || 'warning');
      componentRef.setInput('showCancelButton', options.showCancelButton !== false);
      componentRef.setInput('confirmButtonText', options.confirmButtonText || 'Evet');
      componentRef.setInput('cancelButtonText', options.cancelButtonText || 'İptal');
      componentRef.setInput('confirmButtonColor', options.confirmButtonColor || '#00d084');
      componentRef.setInput('cancelButtonColor', options.cancelButtonColor || '#666666');

      // Handle result
      const confirmSub = componentRef.instance.confirmed.subscribe(() => {
        confirmSub.unsubscribe();
        cancelSub.unsubscribe();
        dismissSub.unsubscribe();
        this.removeSwal(componentRef);
        observer.next({ isConfirmed: true });
        observer.complete();
      });

      const cancelSub = componentRef.instance.cancelled.subscribe(() => {
        confirmSub.unsubscribe();
        cancelSub.unsubscribe();
        dismissSub.unsubscribe();
        this.removeSwal(componentRef);
        observer.next({ isConfirmed: false, isDismissed: true });
        observer.complete();
      });

      const dismissSub = componentRef.instance.dismissed.subscribe(() => {
        confirmSub.unsubscribe();
        cancelSub.unsubscribe();
        dismissSub.unsubscribe();
        this.removeSwal(componentRef);
        observer.next({ isConfirmed: false, isDismissed: true });
        observer.complete();
      });

      // Attach to view
      this.appRef.attachView(componentRef.hostView);
      this.swalContainer!.appendChild(componentRef.location.nativeElement);
    });
  }

  /**
   * Show success alert
   */
  success(title: string, text?: string): Observable<SwalResult> {
    return this.fire({
      title,
      text,
      type: 'success',
      showCancelButton: false,
      confirmButtonText: 'Tamam',
    });
  }

  /**
   * Show error alert
   */
  error(title: string, text?: string): Observable<SwalResult> {
    return this.fire({
      title,
      text,
      type: 'error',
      showCancelButton: false,
      confirmButtonText: 'Tamam',
    });
  }

  /**
   * Show warning alert
   */
  warning(title: string, text?: string): Observable<SwalResult> {
    return this.fire({
      title,
      text,
      type: 'warning',
      showCancelButton: false,
      confirmButtonText: 'Tamam',
    });
  }

  /**
   * Show info alert
   */
  info(title: string, text?: string): Observable<SwalResult> {
    return this.fire({
      title,
      text,
      type: 'info',
      showCancelButton: false,
      confirmButtonText: 'Tamam',
    });
  }

  /**
   * Show confirmation dialog
   */
  confirm(title: string, text?: string, confirmText?: string, cancelText?: string): Observable<SwalResult> {
    return this.fire({
      title,
      text,
      type: 'warning',
      showCancelButton: true,
      confirmButtonText: confirmText || 'Evet',
      cancelButtonText: cancelText || 'İptal',
    });
  }

  private removeSwal(componentRef: ComponentRef<SwalComponent>) {
    const element = componentRef.location.nativeElement;
    if (element && element.parentNode) {
      element.classList.add('swal-closing');
      setTimeout(() => {
        this.appRef.detachView(componentRef.hostView);
        componentRef.destroy();
        if (element.parentNode) {
          element.parentNode.removeChild(element);
        }
      }, 300);
    }
  }
}

