import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';

@Component({
  imports: [RouterModule, LayoutComponent],
  selector: 'app-root',
  template: `
    <router-outlet></router-outlet>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  protected title = 'backoffice';
}
