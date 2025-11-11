import { ChangeDetectionStrategy, Component, Input, inject, OnInit, ChangeDetectorRef, computed, effect } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './sidebar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent implements OnInit {
  @Input() isOpen: boolean = true;
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  translations = computed(() => {
    const currentLang = this.languageService.currentLanguage();
    return {
      dashboard: this.languageService.translate('sidebar.dashboard'),
      cars: this.languageService.translate('sidebar.cars'),
      rentals: this.languageService.translate('sidebar.rentals'),
      users: this.languageService.translate('sidebar.users'),
      insurances: this.languageService.translate('sidebar.insurances'),
      settings: this.languageService.translate('sidebar.settings'),
    };
  });

  constructor() {
    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    this.cdr.detectChanges();
  }
}
