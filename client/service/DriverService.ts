import { httpClient } from "@/lib/http-client";
import type { DriverLocation } from "./contract/DriverContract";
import { TripContract } from "./contract/TripContract";

class DriverService {
  // Additional
  async getDriverLocation(driverUuid: string): Promise<DriverLocation> {
    const response = await httpClient.get<DriverLocation>(
      `/mp/driver/${driverUuid}/location`,
    );
    return response.data;
  }

  async updateIsOnline(driverUuid: string, isOnline: boolean): Promise<void> {
    await httpClient.patch<void>(`/ts/driver/${driverUuid}/online`, {
      isOnline,
    });
  }

  // Product workflow
  async acceptTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/driver/trip/${tripUuid}/accept`,
    );
    return response.data;
  }

  async rejectTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/driver/trip/${tripUuid}/reject`,
    );
    return response.data;
  }

  // Trip span
  async pickupTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/driver/trip/${tripUuid}/pickup`,
    );
    return response.data;
  }

  async dropoffTrip(tripUuid: string): Promise<TripContract> {
    const response = await httpClient.post<TripContract>(
      `/ts/driver/trip/${tripUuid}/dropoff`,
    );
    return response.data;
  }
}

export const driverService = new DriverService();
