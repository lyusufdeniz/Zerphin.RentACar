import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { provideRouter } from '@angular/router';

@Component({
  imports: [RouterModule],
  selector: 'app-root',
  template: '<router-outlet></router-outlet>',
 changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  protected title = 'backoffice';
}
