"use client";

import { createContext, ReactNode, useCallback, useContext, useEffect, useRef, useState } from "react";
import { LocationPermissionSheet } from "@/components/location/LocationPermissionSheet";

export type LocationStatus =
  | "checking"
  | "prompt"
  | "granted"
  | "denied"
  | "unsupported"
  | "unavailable"
  | "timeout";

export type CurrentLocation = {
  latitude: number;
  longitude: number;
  accuracy: number;
  updatedAt: number;
};

type LocationContextValue = {
  status: LocationStatus;
  location: CurrentLocation | null;
  requestLocation: () => void;
};

const LocationContext = createContext<LocationContextValue | null>(null);

export function LocationProvider({ children }: { children: ReactNode }) {
  const [status, setStatus] = useState<LocationStatus>("checking");
  const [location, setLocation] = useState<CurrentLocation | null>(null);
  const watchId = useRef<number | null>(null);

  const handleSuccess = useCallback((position: GeolocationPosition) => {
    setLocation({
      latitude: position.coords.latitude,
      longitude: position.coords.longitude,
      accuracy: position.coords.accuracy,
      updatedAt: position.timestamp,
    });
    setStatus("granted");
  }, []);

  const handleError = useCallback((error: GeolocationPositionError) => {
    setLocation(null);

    if (error.code === error.PERMISSION_DENIED) {
      setStatus("denied");
      return;
    }

    if (error.code === error.POSITION_UNAVAILABLE) {
      setStatus("unavailable");
      return;
    }

    setStatus("timeout");
  }, []);

  const startTracking = useCallback(() => {
    if (!("geolocation" in navigator)) {
      setStatus("unsupported");
      return;
    }

    if (watchId.current !== null) {
      navigator.geolocation.clearWatch(watchId.current);
    }

    watchId.current = navigator.geolocation.watchPosition(
      handleSuccess,
      handleError,
      {
        enableHighAccuracy: true,
        timeout: 10_000,
        maximumAge: 5_000,
      }
    );
  }, [handleError, handleSuccess]);

  const requestLocation = useCallback(() => {
    setStatus("checking");
    startTracking();
  }, [startTracking]);

  useEffect(() => {
    let active = true;

    if (!("geolocation" in navigator)) {
      const timeout = window.setTimeout(() => {
        if (active) setStatus("unsupported");
      }, 0);

      return () => {
        active = false;
        window.clearTimeout(timeout);
      };
    }

    if ("permissions" in navigator) {
      navigator.permissions.query({ name: "geolocation" }).then((permission) => {
        if (active && permission.state === "prompt") setStatus("prompt");
      }).catch(() => undefined);
    }

    watchId.current = navigator.geolocation.watchPosition(
      handleSuccess,
      handleError,
      {
        enableHighAccuracy: true,
        timeout: 10_000,
        maximumAge: 5_000,
      }
    );

    return () => {
      active = false;
      if (watchId.current !== null) {
        navigator.geolocation.clearWatch(watchId.current);
        watchId.current = null;
      }
    };
  }, [handleError, handleSuccess]);

  const blockedStatus = status !== "checking" && status !== "granted" ? status : null;

  return (
    <LocationContext.Provider value={{ status, location, requestLocation }}>
      {children}
      {blockedStatus && (
        <LocationPermissionSheet status={blockedStatus} onRetry={requestLocation} />
      )}
    </LocationContext.Provider>
  );
}

export function useLocation() {
  const context = useContext(LocationContext);
  if (!context) throw new Error("useLocation phải được sử dụng bên trong LocationProvider");
  return context;
}
