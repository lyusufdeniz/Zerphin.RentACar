import {
  Injectable,
  inject,
  ComponentRef,
  ApplicationRef,
  createComponent,
  EnvironmentInjector,
  Type,
} from '@angular/core';
import { ModalComponent, ModalConfig } from '../components/modal/modal.component';

export interface ModalRef {
  close: () => void;
  componentRef?: ComponentRef<any>;
}

@Injectable({
  providedIn: 'root',
})
export class ModalService {
  private appRef = inject(ApplicationRef);
  private injector = inject(EnvironmentInjector);
  private modalContainer?: HTMLElement;
  activeModals: ComponentRef<ModalComponent>[] = [];
  private contentRefs: Map<ComponentRef<ModalComponent>, ComponentRef<any>> = new Map();

  constructor() {
    this.createContainer();
  }

  /**
   * Create modal container
   */
  private createContainer() {
    this.modalContainer = document.createElement('div');
    this.modalContainer.className = 'modal-service-container';
    document.body.appendChild(this.modalContainer);
  }

  /**
   * Open modal with content component
   */
  open<T>(
    component: Type<T>,
    config?: ModalConfig & { inputs?: { [key: string]: any } }
  ): { modalRef: ComponentRef<ModalComponent>; contentRef: ComponentRef<T>; close: () => void } {
    if (!this.modalContainer) {
      this.createContainer();
    }

    // Create modal component
    const modalRef = createComponent(ModalComponent, {
      environmentInjector: this.injector,
    });

    // Configure modal
    if (config) {
      if (config.title) modalRef.setInput('title', config.title);
      if (config.size) modalRef.setInput('size', config.size);
      if (config.showCloseButton !== undefined)
        modalRef.setInput('showCloseButton', config.showCloseButton);
      if (config.closeOnBackdropClick !== undefined)
        modalRef.setInput('closeOnBackdropClick', config.closeOnBackdropClick);
      if (config.closeOnEscape !== undefined)
        modalRef.setInput('closeOnEscape', config.closeOnEscape);
    }

    // Create content component
    const contentRef = createComponent(component, {
      environmentInjector: this.injector,
    });

    // Set inputs if provided
    if (config?.inputs) {
      Object.keys(config.inputs).forEach((key) => {
        contentRef.setInput(key, config.inputs![key]);
      });
    }

    // Handle close event
    const closeFn = () => this.closeModal(modalRef);
    modalRef.instance.close.subscribe(closeFn);

    // Attach to view
    this.appRef.attachView(modalRef.hostView);
    this.appRef.attachView(contentRef.hostView);

    // Append modal to container
    this.modalContainer!.appendChild(modalRef.location.nativeElement);
    
    // Project content into modal body
    setTimeout(() => {
      const modalBody = modalRef.location.nativeElement.querySelector('.modal-body');
      if (modalBody) {
        modalBody.appendChild(contentRef.location.nativeElement);
      }
    }, 0);

    this.activeModals.push(modalRef);
    this.contentRefs.set(modalRef, contentRef);

    return { modalRef, contentRef, close: closeFn };
  }

  /**
   * Open simple modal with HTML content
   */
  openSimple(
    title: string,
    content: string,
    config?: Omit<ModalConfig, 'title'>
  ): { modalRef: ComponentRef<ModalComponent>; close: () => void } {
    if (!this.modalContainer) {
      this.createContainer();
    }

    const modalRef = createComponent(ModalComponent, {
      environmentInjector: this.injector,
    });

    modalRef.setInput('title', title);

    if (config) {
      if (config.size) modalRef.setInput('size', config.size);
      if (config.showCloseButton !== undefined)
        modalRef.setInput('showCloseButton', config.showCloseButton);
      if (config.closeOnBackdropClick !== undefined)
        modalRef.setInput('closeOnBackdropClick', config.closeOnBackdropClick);
      if (config.closeOnEscape !== undefined)
        modalRef.setInput('closeOnEscape', config.closeOnEscape);
    }

    // Set content
    const modalBody = modalRef.location.nativeElement.querySelector('.modal-body');
    if (modalBody) {
      modalBody.innerHTML = content;
    }

    modalRef.instance.close.subscribe(() => {
      this.closeModal(modalRef);
    });

    this.appRef.attachView(modalRef.hostView);
    this.modalContainer!.appendChild(modalRef.location.nativeElement);

    this.activeModals.push(modalRef);

    const closeFn = () => this.closeModal(modalRef);
    return { modalRef, close: closeFn };
  }

  /**
   * Close modal
   */
  closeModal(modalRef: ComponentRef<ModalComponent>) {
    const index = this.activeModals.indexOf(modalRef);
    if (index > -1) {
      this.activeModals.splice(index, 1);
    }

    // Get and destroy content ref if exists
    const contentRef = this.contentRefs.get(modalRef);
    if (contentRef) {
      this.contentRefs.delete(modalRef);
    }

    // Animate out
    const element = modalRef.location.nativeElement;
    element.classList.add('modal-closing');

    setTimeout(() => {
      if (contentRef) {
        this.appRef.detachView(contentRef.hostView);
        contentRef.destroy();
      }
      this.appRef.detachView(modalRef.hostView);
      modalRef.destroy();
    }, 300);
  }

  /**
   * Close all modals
   */
  closeAll() {
    this.activeModals.forEach((modal) => this.closeModal(modal));
  }

  /**
   * Check if any modal is open
   */
  hasOpenModals(): boolean {
    return this.activeModals.length > 0;
  }
}

