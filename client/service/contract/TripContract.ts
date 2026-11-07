export type TripLocationContract = {
  name: string;
  latitude: number;
  longitude: number;
};

export type CreateTripRequestContract = {
  pickup: TripLocationContract;
  destination: TripLocationContract;
};

export type TripGeometry = {
  type: string;
  coordinates: number[][];
};

export type CreateTripResponseContract = {
  uuid: string;
  riderUuid: string;
  pickup: TripLocationContract;
  destination: TripLocationContract;
  geometry: TripGeometry;
  status: string | number;
  distanceKM: number;
  durationMin: number;
  price: number;
  currency: string;
  createdAt: string;
};

export type MatchingResponseContract = {
  tripUuid: string;
  driverUuid: string | null;
  isSuccess: boolean;
};

export type TripContract = {
  uuid: string;
  riderUuid: string;
  driverUuid: string | null;
  paymentUuid: string | null;
  qrUrl: string | null;
  pickupLatitude: number;
  pickupLongitude: number;
  pickupName: string;
  destinationLatitude: number;
  destinationLongitude: number;
  destinationName: string;
  distanceKM: number | null;
  durationMin: number | null;
  price: number | null;
  status: string;
  driverLatitude: number | null;
  driverLongitude: number | null;
  driver: {
    uuid: string;
    fullname: string;
    phone: string;
    vehicleName: string;
    licensePlate: string;
  } | null;
  geometry: TripGeometry | null;
  createdAt: string;
  updatedAt: string;
};

export type TripHistoryContract = {
  uuid: string;
  pickupName: string;
  destinationName: string;
  distanceKM: number | null;
  durationMin: number | null;
  price: number | null;
  status: string;
  createdAt: string;
  updatedAt: string;
};
