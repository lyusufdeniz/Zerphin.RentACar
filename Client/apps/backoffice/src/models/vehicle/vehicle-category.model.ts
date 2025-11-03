export enum VehicleCategory {
  Hatchback = 1,
  Sedan = 2,
  SUV = 3,
  Pickup = 4,
}

export const VehicleCategoryNames: Record<VehicleCategory, string> = {
  [VehicleCategory.Hatchback]: 'Hatchback',
  [VehicleCategory.Sedan]: 'Sedan',
  [VehicleCategory.SUV]: 'SUV',
  [VehicleCategory.Pickup]: 'Pickup',
};

