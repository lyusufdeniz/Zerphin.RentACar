import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
  effect,
  ChangeDetectorRef,
  computed,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FavoritePagesService } from '../../services/favorite-pages.service';
import { FavoritePage } from '../../models/favorite-page.model';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-favorite-pages',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './favorite-pages.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FavoritePagesComponent implements OnInit {
  private favoritePagesService = inject(FavoritePagesService);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  favoritePages: FavoritePage[] = [];

  translations = computed(() => {
    const currentLang = this.languageService.currentLanguage();
    return {
      title: this.languageService.translate('settings.favorites.title'),
      remove: this.languageService.translate('settings.favorites.remove'),
      empty: this.languageService.translate('settings.favorites.empty'),
      hint: this.languageService.translate('settings.favorites.hint'),
    };
  });

  constructor() {
    effect(() => {
      this.favoritePages = this.favoritePagesService.favoritePages();
      this.cdr.markForCheck();
    });

    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.favoritePages = this.favoritePagesService.getAllFavorites();
  }

  navigateTo(path: string): void {
    this.favoritePagesService.navigateToFavorite(path);
  }

  removeFavorite(path: string, event: Event): void {
    event.stopPropagation();
    this.favoritePagesService.removeFavorite(path);
  }

  getIcon(page: FavoritePage): string {
    return page.icon || '📄';
  }

  getIconSVGPath(page: FavoritePage): string {
    const path = page.path.toLowerCase();

    if (path.includes('dashboard')) {
      return 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6';
    } else if (path.includes('cars') || path.includes('araç')) {
      return 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z';
    } else if (path.includes('rental') || path.includes('kiralama')) {
      return 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z';
    } else if (path.includes('customer') || path.includes('kullanıcı') || path.includes('müşteri')) {
      return 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z';
    } else if (path.includes('insurance') || path.includes('sigorta')) {
      return 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z';
    } else if (path.includes('invoice') || path.includes('fatura')) {
      return 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
    } else if (path.includes('setting') || path.includes('ayar')) {
      return 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z';
    }

    return 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
  }
}
