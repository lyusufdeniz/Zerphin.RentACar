import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DatepickerService {
  private openDatepickerId$ = new Subject<string | null>();

  /**
   * Subscribe to datepicker open/close events
   */
  onDatepickerOpen() {
    return this.openDatepickerId$.asObservable();
  }

  /**
   * Notify that a datepicker is opened
   */
  notifyDatepickerOpened(id: string) {
    this.openDatepickerId$.next(id);
  }

  /**
   * Notify that all datepickers should close
   */
  notifyDatepickerClosed() {
    this.openDatepickerId$.next(null);
  }
}

