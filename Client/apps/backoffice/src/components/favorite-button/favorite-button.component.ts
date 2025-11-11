import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  inject,
  ChangeDetectorRef,
  effect,
  computed,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FavoritePagesService } from '../../services/favorite-pages.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-favorite-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './favorite-button.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FavoriteButtonComponent implements OnInit {
  private favoritePagesService = inject(FavoritePagesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private languageService = inject(LanguageService);
  private cdr = inject(ChangeDetectorRef);

  @Input() title?: string;
  @Input() icon?: string;
  @Input() path?: string;

  isFavorite = false;
  isLoading = false;

  translations = computed(() => {
    const currentLang = this.languageService.currentLanguage();
    return {
      add: this.languageService.translate('settings.favorites.add'),
      remove: this.languageService.translate('settings.favorites.remove'),
      processing: this.languageService.translate('settings.favorites.processing'),
    };
  });

  constructor() {
    effect(() => {
      this.favoritePagesService.favoritePages();
      const path = this.path || this.router.url;
      this.isFavorite = this.favoritePagesService.isFavorite(path);
      this.cdr.markForCheck();
    });

    effect(() => {
      const _ = this.languageService.currentLanguage();
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    const currentPath = this.path || this.router.url;
    this.isFavorite = this.favoritePagesService.isFavorite(currentPath);
  }

  toggleFavorite(event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (this.isLoading) return;

    this.isLoading = true;
    this.cdr.markForCheck();

    const path = this.path || this.router.url;
    const pageTitle = this.title || this.getPageTitle();

    const result = this.favoritePagesService.toggleFavorite({
      path,
      title: pageTitle,
      icon: this.icon,
    });

    if (result.success) {
      this.isFavorite = result.isFavorite;
    }

    this.isLoading = false;
    this.cdr.markForCheck();
  }

  private getPageTitle(): string {
    const routeData = this.route.snapshot.data;
    if (routeData['title']) {
      return routeData['title'];
    }

    const routeConfig = this.route.snapshot.routeConfig;
    if (routeConfig?.data?.['title']) {
      return routeConfig.data['title'];
    }

    const segments = this.router.url.split('/').filter(Boolean);
    const lastSegment = segments[segments.length - 1];
    return lastSegment
      ? lastSegment.charAt(0).toUpperCase() + lastSegment.slice(1)
      : 'Dashboard';
  }
}
