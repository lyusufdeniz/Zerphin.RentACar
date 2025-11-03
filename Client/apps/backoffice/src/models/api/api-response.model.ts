/**
 * Generic API Response Model
 * All API responses follow this ServiceResult<T> format
 */
export interface ServiceResult<T> {
  data: T | null;
  errorMessage: string[] | null;
}

/**
 * Non-generic ServiceResult (for DELETE operations and operations without data)
 */
export interface ServiceResultWithoutData {
  errorMessage: string[] | null;
}


