import { Injectable, inject, signal } from '@angular/core';
import { FavoritePage } from '../models/favorite-page.model';
import { StorageService } from './storage.service';
import { Router } from '@angular/router';

const FAVORITE_PAGES_KEY = 'favorite_pages';
const MAX_FAVORITES = 10; // Maximum number of favorite pages

@Injectable({
  providedIn: 'root',
})
export class FavoritePagesService {
  private storageService = inject(StorageService);
  private router = inject(Router);

  // Signal for reactive updates
  favoritePages = signal<FavoritePage[]>([]);

  constructor() {
    // Load favorites from localStorage on initialization
    this.loadFavorites();
  }

  /**
   * Load favorites from localStorage
   */
  private loadFavorites(): void {
    const favorites = this.storageService.getItem<FavoritePage[]>(
      FAVORITE_PAGES_KEY
    );
    if (favorites && Array.isArray(favorites)) {
      // Sort by order
      const sorted = favorites.sort((a, b) => a.order - b.order);
      this.favoritePages.set(sorted);
    } else {
      this.favoritePages.set([]);
    }
  }

  /**
   * Save favorites to localStorage
   */
  private saveFavorites(favorites: FavoritePage[]): void {
    this.storageService.setItem(FAVORITE_PAGES_KEY, favorites);
    this.favoritePages.set([...favorites]);
  }

  /**
   * Add page to favorites
   */
  addFavorite(page: Omit<FavoritePage, 'order' | 'addedAt'>): boolean {
    const currentFavorites = this.favoritePages();

    // Check if already exists
    if (currentFavorites.some((fav) => fav.path === page.path)) {
      return false;
    }

    // Check max limit
    if (currentFavorites.length >= MAX_FAVORITES) {
      return false;
    }

    const newFavorite: FavoritePage = {
      ...page,
      order: currentFavorites.length,
      addedAt: new Date().toISOString(),
    };

    const updatedFavorites = [...currentFavorites, newFavorite];
    this.saveFavorites(updatedFavorites);
    return true;
  }

  /**
   * Remove page from favorites
   */
  removeFavorite(path: string): boolean {
    const currentFavorites = this.favoritePages();
    const filtered = currentFavorites.filter((fav) => fav.path !== path);

    if (filtered.length === currentFavorites.length) {
      return false; // Not found
    }

    // Reorder favorites
    const reordered = filtered.map((fav, index) => ({
      ...fav,
      order: index,
    }));

    this.saveFavorites(reordered);
    return true;
  }

  /**
   * Check if page is favorite
   */
  isFavorite(path: string): boolean {
    return this.favoritePages().some((fav) => fav.path === path);
  }

  /**
   * Toggle favorite status
   */
  toggleFavorite(
    page: Omit<FavoritePage, 'order' | 'addedAt'>
  ): { isFavorite: boolean; success: boolean } {
    if (this.isFavorite(page.path)) {
      const success = this.removeFavorite(page.path);
      return { isFavorite: false, success };
    } else {
      const success = this.addFavorite(page);
      return { isFavorite: true, success };
    }
  }

  /**
   * Get all favorites
   */
  getAllFavorites(): FavoritePage[] {
    return this.favoritePages();
  }

  /**
   * Update favorite order (for drag and drop)
   */
  updateOrder(pages: FavoritePage[]): void {
    const reordered = pages.map((page, index) => ({
      ...page,
      order: index,
    }));
    this.saveFavorites(reordered);
  }

  /**
   * Navigate to favorite page
   */
  navigateToFavorite(path: string): void {
    this.router.navigate([path]);
  }
}


