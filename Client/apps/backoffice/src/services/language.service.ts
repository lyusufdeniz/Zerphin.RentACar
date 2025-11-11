import { Injectable, signal, computed, effect, inject } from '@angular/core';
import { StorageService } from './storage.service';
import trTranslations from '../i18n/tr.json';
import enTranslations from '../i18n/en.json';

export type Language = 'tr' | 'en';

const LANGUAGE_KEY = 'app_language';
const DEFAULT_LANGUAGE: Language = 'tr';

const translations: Record<Language, any> = {
  tr: trTranslations,
  en: enTranslations,
};

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  private storageService = inject(StorageService);

  private _currentLanguage = signal<Language>(this.loadLanguage());

  currentLanguage = computed(() => this._currentLanguage());

  constructor() {

    effect(() => {
      const language = this._currentLanguage();
      this.storageService.setItem(LANGUAGE_KEY, language);
    });
  }

  private loadLanguage(): Language {
    const savedLanguage = this.storageService.getItem<Language>(LANGUAGE_KEY);
    return savedLanguage || DEFAULT_LANGUAGE;
  }

  setLanguage(language: Language): void {
    this._currentLanguage.set(language);
  }

  translate(key: string): string {
    const language = this._currentLanguage();
    const translationObj = translations[language];
    if (!translationObj) return key;

    const keys = key.split('.');
    let value: any = translationObj;

    for (const k of keys) {
      if (value && typeof value === 'object' && k in value) {
        value = value[k];
      } else {
        return key; 
      }
    }

    return typeof value === 'string' ? value : key;
  }

  translateWithParams(key: string, params: Record<string, string | number>): string {
    let translation = this.translate(key);

    Object.keys(params).forEach((paramKey) => {
      translation = translation.replace(`{{${paramKey}}}`, String(params[paramKey]));
    });

    return translation;
  }

  isTurkish(): boolean {
    return this._currentLanguage() === 'tr';
  }

  isEnglish(): boolean {
    return this._currentLanguage() === 'en';
  }

  getAvailableLanguages(): { value: Language; label: string; flag: string }[] {
    return [
      { value: 'tr', label: 'Türkçe', flag: '🇹🇷' },
      { value: 'en', label: 'English', flag: '🇬🇧' },
    ];
  }
}
