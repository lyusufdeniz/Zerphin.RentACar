import { Injectable, inject, ComponentRef, ViewContainerRef, ApplicationRef, createComponent, EnvironmentInjector } from '@angular/core';
import { ToastComponent, ToastConfig, ToastType } from '../components/toast/toast.component';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private appRef = inject(ApplicationRef);
  private injector = inject(EnvironmentInjector);
  private toastContainer?: HTMLElement;

  constructor() {
    this.createContainer();
  }

  private createContainer() {
    this.toastContainer = document.createElement('div');
    this.toastContainer.className = 'toast-container';
    document.body.appendChild(this.toastContainer);
  }

  private showToast(message: string, type: ToastType, duration: number = 3000) {
    if (!this.toastContainer) {
      this.createContainer();
    }

    const componentRef = createComponent(ToastComponent, {
      environmentInjector: this.injector,
    });

    componentRef.setInput('message', message);
    componentRef.setInput('type', type);
    componentRef.setInput('duration', duration);

    this.appRef.attachView(componentRef.hostView);
    this.toastContainer!.appendChild(componentRef.location.nativeElement);

    // Auto remove after duration
    if (duration > 0) {
      setTimeout(() => {
        this.removeToast(componentRef);
      }, duration);
    }
  }

  private removeToast(componentRef: ComponentRef<ToastComponent>) {
    const element = componentRef.location.nativeElement;
    element.classList.add('toast-closing');
    
    setTimeout(() => {
      this.appRef.detachView(componentRef.hostView);
      componentRef.destroy();
      element.remove();
    }, 300);
  }

  success(message: string, duration: number = 3000) {
    this.showToast(message, 'success', duration);
  }

  error(message: string, duration: number = 5000) {
    this.showToast(message, 'error', duration);
  }

  warning(message: string, duration: number = 4000) {
    this.showToast(message, 'warning', duration);
  }

  info(message: string, duration: number = 3000) {
    this.showToast(message, 'info', duration);
  }

  showErrorMessages(errorMessages: string[] | null | undefined) {
    if (!errorMessages || errorMessages.length === 0) {
      return;
    }

    errorMessages.forEach((error, index) => {
      setTimeout(() => {
        this.error(error);
      }, index * 100); // Stagger toasts slightly
    });
  }
}


