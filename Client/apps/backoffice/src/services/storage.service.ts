import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class StorageService {
  setItem(key: string, value: any): void {
    try {
      const stringValue = typeof value === 'string' ? value : JSON.stringify(value);
      localStorage.setItem(key, stringValue);
    } catch (error) {
      console.error(`Error setting localStorage key "${key}":`, error);
    }
  }
  getItem<T>(key: string): T | null {
    try {
      const item = localStorage.getItem(key);
      if (item === null) {
        return null;
      }
      try {
        return JSON.parse(item) as T;
      } catch {
        return item as T;
      }
    } catch (error) {
      console.error(`Error getting localStorage key "${key}":`, error);
      return null;
    }
  }
  getItemString(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch (error) {
      console.error(`Error getting localStorage key "${key}":`, error);
      return null;
    }
  }
  removeItem(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error(`Error removing localStorage key "${key}":`, error);
    }
  }
  clear(): void {
    try {
      localStorage.clear();
    } catch (error) {
      console.error('Error clearing localStorage:', error);
    }
  }
  hasItem(key: string): boolean {
    return localStorage.getItem(key) !== null;
  }
  getAllKeys(): string[] {
    const keys: string[] = [];
    try {
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key) {
          keys.push(key);
        }
      }
    } catch (error) {
      console.error('Error getting all localStorage keys:', error);
    }
    return keys;
  }
  setItemSession(key: string, value: any): void {
    try {
      const stringValue = typeof value === 'string' ? value : JSON.stringify(value);
      sessionStorage.setItem(key, stringValue);
    } catch (error) {
      console.error(`Error setting sessionStorage key "${key}":`, error);
    }
  }
  getItemSession<T>(key: string): T | null {
    try {
      const item = sessionStorage.getItem(key);
      if (item === null) {
        return null;
      }
      try {
        return JSON.parse(item) as T;
      } catch {
        return item as T;
      }
    } catch (error) {
      console.error(`Error getting sessionStorage key "${key}":`, error);
      return null;
    }
  }
  getItemStringSession(key: string): string | null {
    try {
      return sessionStorage.getItem(key);
    } catch (error) {
      console.error(`Error getting sessionStorage key "${key}":`, error);
      return null;
    }
  }
  removeItemSession(key: string): void {
    try {
      sessionStorage.removeItem(key);
    } catch (error) {
      console.error(`Error removing sessionStorage key "${key}":`, error);
    }
  }
  clearSession(): void {
    try {
      sessionStorage.clear();
    } catch (error) {
      console.error('Error clearing sessionStorage:', error);
    }
  }
}