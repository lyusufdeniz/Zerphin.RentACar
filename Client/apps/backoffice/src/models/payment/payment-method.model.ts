export enum PaymentMethod {
  Cash = 1,
  CreditCard = 2,
  DebitCard = 3,
  BankTransfer = 4,
  Other = 5,
}
export const PaymentMethodNames: Record<PaymentMethod, string> = {
  [PaymentMethod.Cash]: 'Nakit',
  [PaymentMethod.CreditCard]: 'Kredi Kartı',
  [PaymentMethod.DebitCard]: 'Banka Kartı',
  [PaymentMethod.BankTransfer]: 'Banka Transferi',
  [PaymentMethod.Other]: 'Diğer',
};