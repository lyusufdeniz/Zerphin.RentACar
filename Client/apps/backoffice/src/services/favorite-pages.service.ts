import { Injectable, inject, signal } from '@angular/core';
import { FavoritePage } from '../models/favorite-page.model';
import { StorageService } from './storage.service';
import { Router } from '@angular/router';
const FAVORITE_PAGES_KEY = 'favorite_pages';
const MAX_FAVORITES = 10; 

@Injectable({
  providedIn: 'root',
})
export class FavoritePagesService {
  private storageService = inject(StorageService);
  private router = inject(Router);
  favoritePages = signal<FavoritePage[]>([]);
  constructor() {
    this.loadFavorites();
  }
  private loadFavorites(): void {
    const favorites = this.storageService.getItem<FavoritePage[]>(
      FAVORITE_PAGES_KEY
    );
    if (favorites && Array.isArray(favorites)) {
      const sorted = favorites.sort((a, b) => a.order - b.order);
      this.favoritePages.set(sorted);
    } else {
      this.favoritePages.set([]);
    }
  }
  private saveFavorites(favorites: FavoritePage[]): void {
    this.storageService.setItem(FAVORITE_PAGES_KEY, favorites);
    this.favoritePages.set([...favorites]);
  }
  addFavorite(page: Omit<FavoritePage, 'order' | 'addedAt'>): boolean {
    const currentFavorites = this.favoritePages();
    if (currentFavorites.some((fav) => fav.path === page.path)) {
      return false;
    }
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
  removeFavorite(path: string): boolean {
    const currentFavorites = this.favoritePages();
    const filtered = currentFavorites.filter((fav) => fav.path !== path);
    if (filtered.length === currentFavorites.length) {
      return false; 
    }
    const reordered = filtered.map((fav, index) => ({
      ...fav,
      order: index,
    }));
    this.saveFavorites(reordered);
    return true;
  }
  isFavorite(path: string): boolean {
    return this.favoritePages().some((fav) => fav.path === path);
  }
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
  getAllFavorites(): FavoritePage[] {
    return this.favoritePages();
  }
  updateOrder(pages: FavoritePage[]): void {
    const reordered = pages.map((page, index) => ({
      ...page,
      order: index,
    }));
    this.saveFavorites(reordered);
  }
  navigateToFavorite(path: string): void {
    this.router.navigate([path]);
  }
  clearFavorites(): void {
    this.storageService.removeItem(FAVORITE_PAGES_KEY);
    this.favoritePages.set([]);
  }
}