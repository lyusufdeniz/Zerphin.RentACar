import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class DatepickerService {
  private openDatepickerId$ = new Subject<string | null>();
  onDatepickerOpen() {
    return this.openDatepickerId$.asObservable();
  }
  notifyDatepickerOpened(id: string) {
    this.openDatepickerId$.next(id);
  }
  notifyDatepickerClosed() {
    this.openDatepickerId$.next(null);
  }
}