import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  inject,
  ChangeDetectorRef,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FavoritePagesService } from '../../services/favorite-pages.service';
import { ActivatedRoute, Router } from '@angular/router';

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
  private cdr = inject(ChangeDetectorRef);

  @Input() title?: string; // Page title (if not provided, will try to get from route)
  @Input() icon?: string; // Page icon
  @Input() path?: string; // Page path (if not provided, will use current route)

  isFavorite = false;
  isLoading = false;

  constructor() {
    // Track changes to favoritePages signal
    effect(() => {
      // Access favoritePages signal to trigger effect when it changes
      this.favoritePagesService.favoritePages();
      // Update isFavorite status
      const path = this.path || this.router.url;
      this.isFavorite = this.favoritePagesService.isFavorite(path);
      this.cdr.markForCheck();
    });
  }

  ngOnInit() {
    const currentPath = this.path || this.router.url;
    this.isFavorite = this.favoritePagesService.isFavorite(currentPath);
  }

  /**
   * Toggle favorite status
   */
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

  /**
   * Get page title from route or default
   */
  private getPageTitle(): string {
    // Try to get title from route data
    const routeData = this.route.snapshot.data;
    if (routeData['title']) {
      return routeData['title'];
    }

    // Try to get from route config
    const routeConfig = this.route.snapshot.routeConfig;
    if (routeConfig?.data?.['title']) {
      return routeConfig.data['title'];
    }

    // Default: use path segments
    const segments = this.router.url.split('/').filter(Boolean);
    const lastSegment = segments[segments.length - 1];
    return lastSegment
      ? lastSegment.charAt(0).toUpperCase() + lastSegment.slice(1)
      : 'Dashboard';
  }
}

