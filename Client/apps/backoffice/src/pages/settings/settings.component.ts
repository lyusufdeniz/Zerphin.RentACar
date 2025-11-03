import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FavoriteButtonComponent],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Ayarlar</h1>
        <app-favorite-button
          title="Ayarlar"
          icon="⚙️"
          path="/settings"
        ></app-favorite-button>
      </div>
      <p>Ayarlar sayfası içeriği buraya gelecek</p>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsComponent {}

