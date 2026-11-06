import { httpClient } from "@/lib/http-client";
import {
  CreateTripRequestContract,
  CreateTripResponseContract,
  MatchingResponseContract,
  TripContract,
  TripHistoryContract,
} from "./contract/TripContract";

class TripService {
  async createTrip(
    contract: CreateTripRequestContract,
  ): Promise<CreateTripResponseContract> {
    const response = await httpClient.post<CreateTripResponseContract>(
      "/ts/trips",
      contract,
    );

    return response.data;
  }

  async confirmTrip(tripUuid: string): Promise<MatchingResponseContract> {
    const response = await httpClient.post<MatchingResponseContract>(
      `/ts/trips/${tripUuid}/confirm`,
    );
    return response.data;
  }

  async getTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.get<TripContract>(`/ts/trips/${tripUuid}`);
    return response.data;
  }

  async getActiveTrip(): Promise<TripContract | null> {
    const response = await httpClient.get<TripContract>("/ts/trips/active");
    return response.status === 204 ? null : response.data;
  }

  async getTripHistory(): Promise<TripHistoryContract[]> {
    const response = await httpClient.get<TripHistoryContract[]>("/ts/trips/history");
    return response.data;
  }

  async acceptTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/trips/${tripUuid}/accept`,
    );
    return response.data;
  }

  async rejectTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/trips/${tripUuid}/reject`,
    );
    return response.data;
  }

  async updateDriverLocation(
    tripUuid: string,
    latitude: number,
    longitude: number,
  ): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/trips/${tripUuid}/driver-location`,
      { latitude, longitude },
    );
    return response.data;
  }

  async pickupTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/trips/${tripUuid}/pickup`,
    );
    return response.data;
  }

  async dropoffTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/trips/${tripUuid}/dropoff`,
    );
    return response.data;
  }
}

export const tripService = new TripService();
