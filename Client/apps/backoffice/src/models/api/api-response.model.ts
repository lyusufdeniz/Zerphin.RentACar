
export interface ServiceResult<T> {
  data: T | null;
  errorMessage: string[] | null;
}
export interface ServiceResultWithoutData {
  errorMessage: string[] | null;
}