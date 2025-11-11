import { Injectable, signal, computed, effect, inject } from '@angular/core';
import { StorageService } from './storage.service';
export type Theme = 'light' | 'dark';
const THEME_KEY = 'app_theme';
const DEFAULT_THEME: Theme = 'dark';
@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private storageService = inject(StorageService);
  private _currentTheme = signal<Theme>(this.loadTheme());
  currentTheme = computed(() => this._currentTheme());
  constructor() {
    this.applyTheme(this._currentTheme());
    effect(() => {
      const theme = this._currentTheme();
      this.applyTheme(theme);
      this.storageService.setItem(THEME_KEY, theme);
    });
  }
  private loadTheme(): Theme {
    const savedTheme = this.storageService.getItem<Theme>(THEME_KEY);
    return savedTheme || DEFAULT_THEME;
  }
  setTheme(theme: Theme): void {
    this._currentTheme.set(theme);
  }
  toggleTheme(): void {
    const newTheme: Theme = this._currentTheme() === 'light' ? 'dark' : 'light';
    this.setTheme(newTheme);
  }
  private applyTheme(theme: Theme): void {
    const htmlElement = document.documentElement;
    const bodyElement = document.body;
    htmlElement.classList.remove('light-theme', 'dark-theme');
    bodyElement.classList.remove('light-theme', 'dark-theme');
    htmlElement.classList.add(`${theme}-theme`);
    bodyElement.classList.add(`${theme}-theme`);
    htmlElement.setAttribute('data-theme', theme);
  }
  isDarkTheme(): boolean {
    return this._currentTheme() === 'dark';
  }
  isLightTheme(): boolean {
    return this._currentTheme() === 'light';
  }
}