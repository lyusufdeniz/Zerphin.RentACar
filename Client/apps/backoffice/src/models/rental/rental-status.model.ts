export enum RentalStatus {
  Active = 1,
  Completed = 2,
  Cancelled = 3,
}

export const RentalStatusNames: Record<RentalStatus, string> = {
  [RentalStatus.Active]: 'Aktif',
  [RentalStatus.Completed]: 'Tamamlandı',
  [RentalStatus.Cancelled]: 'İptal Edildi',
};

