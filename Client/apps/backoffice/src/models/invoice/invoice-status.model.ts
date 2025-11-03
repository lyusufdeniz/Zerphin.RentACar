export enum InvoiceStatus {
  Draft = 0,
  Sent = 1,
  Paid = 2,
  Overdue = 3,
  Cancelled = 4,
}

export const InvoiceStatusNames: Record<InvoiceStatus, string> = {
  [InvoiceStatus.Draft]: 'Taslak',
  [InvoiceStatus.Sent]: 'Gönderildi',
  [InvoiceStatus.Paid]: 'Ödendi',
  [InvoiceStatus.Overdue]: 'Vadesi Geçti',
  [InvoiceStatus.Cancelled]: 'İptal Edildi',
};

