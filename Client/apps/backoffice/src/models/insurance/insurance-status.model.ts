export enum InsuranceStatus {
  Active = 1,
  Expired = 2,
  ExpiringSoon = 3,
  Cancelled = 4,
}
export const InsuranceStatusNames: Record<InsuranceStatus, string> = {
  [InsuranceStatus.Active]: 'Aktif',
  [InsuranceStatus.Expired]: 'Süresi Dolmuş',
  [InsuranceStatus.ExpiringSoon]: 'Yakında Bitecek',
  [InsuranceStatus.Cancelled]: 'İptal Edilmiş',
};