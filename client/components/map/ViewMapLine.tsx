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
  onDriverAnimationComplete?: () => void | Promise<void>;
}

setWorkerUrl("https://unpkg.com/maplibre-gl@6.16.0/dist/maplibre-gl-worker.mjs");

const emptyFeature = (coordinates: number[][]) => ({
  type: "Feature" as const,
  properties: {},
  geometry: { type: "LineString" as const, coordinates },
});

const driverImageUrl = "https://media.istockphoto.com/id/1315202286/vi/vec-to/ng%C6%B0%E1%BB%9Di-l%C3%A1i-xe-%C3%B4m-tr%E1%BB%B1c-tuy%E1%BA%BFn-v%E1%BB%9Bi-x%E1%BA%BFp-h%E1%BA%A1ng-h%E1%BB%93-s%C6%A1-v%C3%A0-k%C3%BD-hi%E1%BB%87u-gps-%C4%91%E1%BA%B7t-vector-minh-h%E1%BB%8Da-ho%E1%BA%A1t-h%C3%ACnh.jpg?s=612x612&w=is&k=20&c=ZBGzvg7ScQukMBLzbkamefEsPokWPal_2ZOoN-88CCA=";

const createDriverMarkerElement = (label: string) => {
  const element = document.createElement("div");
  element.className = "relative flex h-17 w-14 items-start justify-center";
  element.setAttribute("role", "img");
  element.setAttribute("aria-label", label);
  element.title = label;

  const pulse = document.createElement("span");
  pulse.className = "absolute bottom-0 size-5 animate-ping rounded-full bg-[#126b4f]/35";

  const pointer = document.createElement("span");
  pointer.className = "absolute bottom-1 size-4 rotate-45 rounded-br-sm bg-[#126b4f] shadow-md";

  const avatar = document.createElement("span");
  avatar.className = "relative z-10 grid size-13 overflow-hidden rounded-full border-3 border-white bg-[#126b4f] shadow-[0_5px_15px_rgba(15,45,35,0.35)] ring-2 ring-[#126b4f]";

  const fallback = document.createElement("span");
  fallback.className = "absolute inset-0 grid place-items-center text-xl";
  fallback.textContent = "🏍️";

  const image = document.createElement("img");
  image.src = driverImageUrl;
  image.alt = "";
  image.className = "relative z-10 size-full object-cover";
  image.style.objectPosition = "50% 42%";
  image.addEventListener("error", () => image.remove(), { once: true });

  avatar.append(fallback, image);
  element.append(pulse, pointer, avatar);
  return element;
};

const createLocationMarkerElement = (type: "pickup" | "destination") => {
  const isPickup = type === "pickup";
  const label = isPickup ? "Điểm đón" : "Điểm trả";
  const element = document.createElement("div");
  element.className = "relative flex h-14 items-start justify-center";
  element.setAttribute("role", "img");
  element.setAttribute("aria-label", label);
  element.title = label;

  const pointer = document.createElement("span");
  pointer.className = isPickup
    ? "absolute bottom-1 left-1/2 size-4 -translate-x-1/2 rotate-45 rounded-br-sm bg-[#126b4f] shadow"
    : "absolute bottom-1 left-1/2 size-4 -translate-x-1/2 rotate-45 rounded-br-sm bg-[#b05d43] shadow";

  const card = document.createElement("span");
  card.className = isPickup
    ? "relative z-10 flex items-center gap-1.5 rounded-xl border-2 border-[#126b4f] bg-white p-1 pr-2.5 shadow-[0_5px_14px_rgba(15,45,35,0.22)]"
    : "relative z-10 flex items-center gap-1.5 rounded-xl border-2 border-[#b05d43] bg-white p-1 pr-2.5 shadow-[0_5px_14px_rgba(100,45,30,0.22)]";

  const icon = document.createElement("span");
  icon.className = isPickup
    ? "grid size-8 place-items-center rounded-lg bg-[#126b4f] text-white"
    : "grid size-8 place-items-center rounded-lg bg-[#b05d43] text-white";
  icon.innerHTML = isPickup
    ? '<svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><polygon points="3 11 22 2 13 21 11 13 3 11"></polygon></svg>'
    : '<svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M5 22V4"></path><path d="M5 4h11l-1 4 1 4H5"></path></svg>';

  const text = document.createElement("span");
  text.className = isPickup
    ? "whitespace-nowrap text-[11px] font-bold text-[#126b4f]"
    : "whitespace-nowrap text-[11px] font-bold text-[#9a4934]";
  text.textContent = label;

  card.append(icon, text);
  element.append(pointer, card);
  return element;
};

export default function ViewMapLine({
  pickup,
  destination,
  geometry,
  driverGeometry,
  driverLocation,
  driverAnimation,
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
    animationCompleteRef.current = onDriverAnimationComplete;
  }, [onDriverAnimationComplete]);

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
      addRoute("driver-route", driverCoordinatesRef.current, "#126b4f");

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

      marker.setLngLat([
        fromLongitude + (toLongitude - fromLongitude) * segmentProgress,
        fromLatitude + (toLatitude - fromLatitude) * segmentProgress,
      ]);

      if (progress < 1) {
        animationFrameRef.current = requestAnimationFrame(animate);
        return;
      }

      animationFrameRef.current = null;
      Promise.resolve(animationCompleteRef.current?.()).finally(() => {
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
