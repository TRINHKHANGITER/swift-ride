"use client";

import { useEffect, useState } from "react";
import type { TripContract, TripGeometry } from "@/service/contract/TripContract";
import { osrmService } from "@/service/OsrmService";
import { tripStatusName } from "@/service/trip-status";

export function useTripRoutes(trip: TripContract | null) {
  const storedGeometry = trip?.geometry;
  const [tripRoute, setTripRoute] = useState<TripGeometry | undefined>(
    storedGeometry && storedGeometry.coordinates.length > 2 ? storedGeometry : undefined,
  );
  const [driverRoute, setDriverRoute] = useState<TripGeometry | undefined>();
  const status = trip ? tripStatusName(trip.status) : "";
  const isPickedUp = status === "PickedUp";
  const pickupLatitude = trip?.pickupLatitude;
  const pickupLongitude = trip?.pickupLongitude;
  const destinationLatitude = trip?.destinationLatitude;
  const destinationLongitude = trip?.destinationLongitude;
  const driverLatitude = trip?.driverLatitude;
  const driverLongitude = trip?.driverLongitude;

  useEffect(() => {
    if (pickupLatitude === undefined || pickupLongitude === undefined || destinationLatitude === undefined || destinationLongitude === undefined) return;
    if (storedGeometry && storedGeometry.coordinates.length > 2) {
      const timer = window.setTimeout(() => setTripRoute(storedGeometry), 0);
      return () => window.clearTimeout(timer);
    }

    let active = true;
    osrmService.getRoute(
      { latitude: pickupLatitude, longitude: pickupLongitude },
      { latitude: destinationLatitude, longitude: destinationLongitude },
    ).then((geometry) => { if (active) setTripRoute(geometry); }).catch(() => undefined);
    return () => { active = false; };
  }, [storedGeometry, pickupLatitude, pickupLongitude, destinationLatitude, destinationLongitude]);

  useEffect(() => {
    if (driverLatitude === undefined || driverLatitude === null || driverLongitude === undefined || driverLongitude === null ||
        pickupLatitude === undefined || pickupLongitude === undefined || destinationLatitude === undefined || destinationLongitude === undefined) return;
    if (status === "DroppedOff") {
      const timer = window.setTimeout(() => setDriverRoute(undefined), 0);
      return () => window.clearTimeout(timer);
    }
    let active = true;
    const target = isPickedUp
      ? { latitude: destinationLatitude, longitude: destinationLongitude }
      : { latitude: pickupLatitude, longitude: pickupLongitude };

    osrmService.getRoute(
      { latitude: driverLatitude, longitude: driverLongitude },
      target,
    ).then((geometry) => { if (active) setDriverRoute(geometry); }).catch(() => undefined);
    return () => { active = false; };
  }, [driverLatitude, driverLongitude, pickupLatitude, pickupLongitude, destinationLatitude, destinationLongitude, status, isPickedUp]);

  return { tripRoute, driverRoute };
}
