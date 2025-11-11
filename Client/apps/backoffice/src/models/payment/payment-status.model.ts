export enum PaymentStatus {
  Pending = 1,
  Processing = 2,
  Completed = 3,
  Failed = 4,
  Refunded = 5,
  Cancelled = 6,
}
export const PaymentStatusNames: Record<PaymentStatus, string> = {
  [PaymentStatus.Pending]: 'Beklemede',
  [PaymentStatus.Processing]: 'İşleniyor',
  [PaymentStatus.Completed]: 'Tamamlandı',
  [PaymentStatus.Failed]: 'Başarısız',
  [PaymentStatus.Refunded]: 'İade Edildi',
  [PaymentStatus.Cancelled]: 'İptal Edildi',
};