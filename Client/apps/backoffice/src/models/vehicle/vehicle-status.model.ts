export enum VehicleStatus {
  Available = 1,
  Rented = 2,
}

export const VehicleStatusNames: Record<VehicleStatus, string> = {
  [VehicleStatus.Available]: 'Müsait',
  [VehicleStatus.Rented]: 'Kiralanmış',
};

