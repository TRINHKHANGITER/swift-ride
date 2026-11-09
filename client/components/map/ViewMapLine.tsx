"use client";

import { GeoJSONSource, LngLatBounds, Map, Marker, setWorkerUrl } from "maplibre-gl";
import "maplibre-gl/dist/maplibre-gl.css";
import { useEffect, useMemo, useRef } from "react";
import type { TripGeometry } from "@/service/contract/TripContract";

interface ViewMapLineProps {
  pickup?: { name: string; latitude: number; longitude: number };
  destination?: { name: string; latitude: number; longitude: number };
  geometry?: TripGeometry;
  driverGeometry?: TripGeometry;
  driverLocation?: { latitude: number; longitude: number } | null;
  driverAnimation?: { key: number; coordinates: number[][]; durationMs?: number } | null;
  onDriverLocationChange?: (location: { latitude: number; longitude: number }) => void | Promise<void>;
  onDriverAnimationComplete?: () => void | Promise<void>;
}

setWorkerUrl("https://unpkg.com/maplibre-gl@6.16.0/dist/maplibre-gl-worker.mjs");

const emptyFeature = (coordinates: number[][]) => ({
  type: "Feature" as const,
  properties: {},
  geometry: { type: "LineString" as const, coordinates },
});

const createDriverMarkerElement = (label: string) => {
  const element = document.createElement("div");
  element.className = "relative flex h-11 w-10 items-start justify-center";
  element.setAttribute("role", "img");
  element.setAttribute("aria-label", label);
  element.title = label;

  const foot = document.createElement("span");
  foot.className = "absolute bottom-1.5 size-3 rotate-45 rounded-[2px] bg-[#126b4f] shadow-sm";

  const badge = document.createElement("span");
  badge.className = "relative z-10 grid size-9 place-items-center rounded-full border-[3px] border-white bg-[#126b4f] text-white shadow-md";
  badge.innerHTML = '<svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><circle cx="5.5" cy="17.5" r="3.5"/><circle cx="18.5" cy="17.5" r="3.5"/><path d="M15 6h2.5l2 4M5.5 17.5 9 10h5l4.5 7.5M9 10l3 7.5M7 7h4"/></svg>';

  element.append(foot, badge);
  return element;
};

const createLocationMarkerElement = (type: "pickup" | "destination") => {
  const isPickup = type === "pickup";
  const label = isPickup ? "Điểm đón" : "Điểm trả";
  const element = document.createElement("div");
  element.className = "h-10 w-8 drop-shadow-md";
  element.setAttribute("role", "img");
  element.setAttribute("aria-label", label);
  element.title = label;
  const color = isPickup ? "#126b4f" : "#b05d43";
  const centerShape = isPickup
    ? '<circle cx="16" cy="15" r="5" fill="white"/>'
    : '<rect x="11" y="10" width="10" height="10" rx="2" fill="white"/>';
  element.innerHTML = `<svg width="32" height="40" viewBox="0 0 32 40" aria-hidden="true"><path fill="${color}" stroke="white" stroke-width="1.5" d="M16 1.5C8.5 1.5 2.5 7.5 2.5 15c0 9.7 13.5 23.2 13.5 23.2S29.5 24.7 29.5 15C29.5 7.5 23.5 1.5 16 1.5Z"/>${centerShape}</svg>`;
  return element;
};

export default function ViewMapLine({
  pickup,
  destination,
  geometry,
  driverGeometry,
  driverLocation,
  driverAnimation,
  onDriverLocationChange,
  onDriverAnimationComplete,
}: ViewMapLineProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const mapRef = useRef<Map | null>(null);

  const driverMarkerRef = useRef<Marker | null>(null);
  const tripCoordinatesRef = useRef<number[][]>([]);
  const driverCoordinatesRef = useRef<number[][]>([]);
  const driverLocationRef = useRef(driverLocation);
  const fittedRoutesRef = useRef(false);
  const animationFrameRef = useRef<number | null>(null);
  const animationKeyRef = useRef<number | null>(null);
  const isAnimatingRef = useRef(false);
  const lastLocationEmissionRef = useRef(0);
  const locationChangeRef = useRef(onDriverLocationChange);
  const animationCompleteRef = useRef(onDriverAnimationComplete);

  const pickupLatitude = pickup?.latitude;
  const pickupLongitude = pickup?.longitude;
  const destinationLatitude = destination?.latitude;
  const destinationLongitude = destination?.longitude;
  
  const tripCoordinates = useMemo(() => geometry?.coordinates ?? [], [geometry]);
  const driverCoordinates = useMemo(() => driverGeometry?.coordinates ?? [], [driverGeometry]);

  useEffect(() => {
    tripCoordinatesRef.current = tripCoordinates;
    driverCoordinatesRef.current = driverCoordinates;
    driverLocationRef.current = driverLocation;
  }, [tripCoordinates, driverCoordinates, driverLocation]);

  useEffect(() => {
    locationChangeRef.current = onDriverLocationChange;
    animationCompleteRef.current = onDriverAnimationComplete;
  }, [onDriverAnimationComplete, onDriverLocationChange]);

  // ! Pickip and destination
  useEffect(() => {
    if (!containerRef.current || pickupLatitude === undefined || pickupLongitude === undefined || destinationLatitude === undefined || destinationLongitude === undefined) return;

    const initialBounds = new LngLatBounds()
      .extend([pickupLongitude, pickupLatitude])
      .extend([destinationLongitude, destinationLatitude]);

    const map = new Map({
      container: containerRef.current,
      style: "https://tiles.openfreemap.org/styles/positron",
      center: initialBounds.getCenter(),
      maplibreLogo: false,
      attributionControl: false,
      dragRotate: false,
      pitchWithRotate: false,
    });
    mapRef.current = map;

    const pickupMarker = new Marker({
      element: createLocationMarkerElement("pickup"),
      anchor: "bottom",
    }).setLngLat([pickupLongitude, pickupLatitude]).addTo(map);

    const destinationMarker = new Marker({
      element: createLocationMarkerElement("destination"),
      anchor: "bottom",
    }).setLngLat([destinationLongitude, destinationLatitude]).addTo(map);

    map.on("load", () => {
      const addRoute = (id: string, coordinates: number[][], color: string, dashed = false) => {
        map.addSource(id, { type: "geojson", data: emptyFeature(coordinates) });
        map.addLayer({
          id: `${id}-outline`,
          type: "line",
          source: id,
          layout: { "line-cap": "round", "line-join": "round" },
          paint: { "line-color": "#ffffff", "line-width": 9 },
        });
        map.addLayer({
          id: `${id}-line`,
          type: "line",
          source: id,
          layout: { "line-cap": "round", "line-join": "round" },
          paint: { "line-color": color, "line-width": 5, ...(dashed ? { "line-dasharray": [1.4, 1.4] } : {}) },
        });
      };

      addRoute("trip-route", tripCoordinatesRef.current, "#126b4f");
      // addRoute("driver-route", driverCoordinatesRef.current, "#126b4f");

      const bounds = new LngLatBounds(initialBounds.getSouthWest(), initialBounds.getNorthEast());
      tripCoordinatesRef.current.forEach((coordinate) => bounds.extend(coordinate as [number, number]));
      driverCoordinatesRef.current.forEach((coordinate) => bounds.extend(coordinate as [number, number]));
      const currentDriver = driverLocationRef.current;
      if (currentDriver) bounds.extend([currentDriver.longitude, currentDriver.latitude]);
      map.fitBounds(bounds, { padding: 44, maxZoom: 15, animate: false });
      fittedRoutesRef.current = tripCoordinatesRef.current.length > 1 || driverCoordinatesRef.current.length > 1;
    });

    return () => {
      if (animationFrameRef.current !== null) cancelAnimationFrame(animationFrameRef.current);
      animationFrameRef.current = null;
      isAnimatingRef.current = false;
      driverMarkerRef.current?.remove();
      driverMarkerRef.current = null;
      pickupMarker.remove();
      destinationMarker.remove();
      mapRef.current = null;
      fittedRoutesRef.current = false;
      map.remove();
    };
  }, [pickupLatitude, pickupLongitude, destinationLatitude, destinationLongitude]);

  // ! Draw lines for routing
  useEffect(() => {
    const map = mapRef.current;
    if (!map?.loaded()) return;
    (map.getSource("trip-route") as GeoJSONSource | undefined)?.setData(emptyFeature(tripCoordinates));
    (map.getSource("driver-route") as GeoJSONSource | undefined)?.setData(emptyFeature(driverCoordinates));

    if (!fittedRoutesRef.current && (tripCoordinates.length > 1 || driverCoordinates.length > 1)) {
      const bounds = new LngLatBounds();
      tripCoordinates.forEach((coordinate) => bounds.extend(coordinate as [number, number]));
      driverCoordinates.forEach((coordinate) => bounds.extend(coordinate as [number, number]));
      if (!bounds.isEmpty()) map.fitBounds(bounds, { padding: 44, maxZoom: 15, animate: true });
      fittedRoutesRef.current = true;
    }
  }, [tripCoordinates, driverCoordinates]);

  // ! Driver location marker
  useEffect(() => {
    const map = mapRef.current;
    if (!map || !driverLocation) {
      driverMarkerRef.current?.remove();
      driverMarkerRef.current = null;
      return;
    }

    if (!driverMarkerRef.current) {
      driverMarkerRef.current = new Marker({
        element: createDriverMarkerElement("Vị trí tài xế"),
        anchor: "bottom",
      })
        .setLngLat([driverLocation.longitude, driverLocation.latitude])
        .addTo(map);
    }
    if (!isAnimatingRef.current) {
      driverMarkerRef.current.setLngLat([driverLocation.longitude, driverLocation.latitude]);
    }
  }, [driverLocation?.latitude, driverLocation?.longitude, driverLocation]);

  useEffect(() => {
    const marker = driverMarkerRef.current;
    const coordinates = (driverAnimation?.coordinates ?? []).filter(
      (coordinate): coordinate is [number, number] =>
        Array.isArray(coordinate) &&
        coordinate.length >= 2 &&
        Number.isFinite(coordinate[0]) &&
        Number.isFinite(coordinate[1]),
    );
    const animationKey = driverAnimation?.key;
    if (!marker || animationKey === undefined || coordinates.length < 2 || animationKeyRef.current === animationKey) return;

    if (animationFrameRef.current !== null) cancelAnimationFrame(animationFrameRef.current);
    animationKeyRef.current = animationKey;
    isAnimatingRef.current = true;
    const startedAt = performance.now();
    const duration = driverAnimation?.durationMs ?? 5_000;

    const animate = (now: number) => {
      const elapsed = Math.max(0, now - startedAt);
      const progress = Math.min(1, elapsed / Math.max(1, duration));
      const position = progress * (coordinates.length - 1);
      const index = Math.max(0, Math.min(Math.floor(position), coordinates.length - 2));
      const segmentProgress = position - index;
      const from = coordinates[index];
      const to = coordinates[index + 1];
      if (!from || !to) {
        animationFrameRef.current = null;
        Promise.resolve(animationCompleteRef.current?.()).finally(() => {
          isAnimatingRef.current = false;
        });
        return;
      }
      const [fromLongitude, fromLatitude] = from;
      const [toLongitude, toLatitude] = to;
      const longitude = fromLongitude + (toLongitude - fromLongitude) * segmentProgress;
      const latitude = fromLatitude + (toLatitude - fromLatitude) * segmentProgress;

      marker.setLngLat([longitude, latitude]);

      let locationUpdate: void | Promise<void> = undefined;
      if (now - lastLocationEmissionRef.current >= 500 || progress === 1) {
        lastLocationEmissionRef.current = now;
        locationUpdate = locationChangeRef.current?.({ latitude, longitude });
      }

      if (progress < 1) {
        animationFrameRef.current = requestAnimationFrame(animate);
        return;
      }

      animationFrameRef.current = null;
      Promise.resolve(locationUpdate)
        .then(() => animationCompleteRef.current?.())
        .finally(() => {
          isAnimatingRef.current = false;
        });
    };

    animationFrameRef.current = requestAnimationFrame(animate);
    return () => {
      if (animationFrameRef.current !== null) cancelAnimationFrame(animationFrameRef.current);
      animationFrameRef.current = null;
      isAnimatingRef.current = false;
    };
  }, [driverAnimation]);

  // ! UI
  if (!pickup || !destination) {
    return <div className="grid h-80 w-full place-items-center bg-[#f3f6f5] px-6 text-center"><p className="text-sm text-[#71807a]">Chưa chọn điểm đón hoặc điểm đến</p></div>;
  }

  return (
    <div className="relative">
      <div ref={containerRef} className="h-80 w-full bg-[#eef2f0] sm:h-120" aria-label="Bản đồ lộ trình chuyến đi" />
      {driverCoordinates.length > 1 && <div className="absolute bottom-3 left-3 z-10 rounded-lg bg-white/95 px-3 py-2 text-[11px] shadow"><p className="flex items-center gap-2"><span className="h-0.5 w-5 bg-[#126b4f]" /> Tuyến đường chuyến đi</p></div>}
    </div>
  );
}
