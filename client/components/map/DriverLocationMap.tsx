"use client";

import { Map, setWorkerUrl } from "maplibre-gl";
import "maplibre-gl/dist/maplibre-gl.css";
import { useEffect, useRef } from "react";
import { drawClientLocation } from "./util/driver-point";

type DriverLocationMapProps = {
  latitude: number;
  longitude: number;
};

setWorkerUrl("https://unpkg.com/maplibre-gl@6.16.0/dist/maplibre-gl-worker.mjs");

export default function DriverLocationMap({ latitude, longitude }: DriverLocationMapProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const mapRef = useRef<Map | null>(null);
  const initialLocationRef = useRef({ latitude, longitude });

  useEffect(() => {
    if (!containerRef.current) return;

    const initialLocation = initialLocationRef.current;
    const map = new Map({
      container: containerRef.current,
      style: "https://tiles.openfreemap.org/styles/positron",
      center: [initialLocation.longitude, initialLocation.latitude],
      zoom: 14,
      maplibreLogo: false,
      attributionControl: false,
      dragRotate: false,
      pitchWithRotate: false,
    });

    mapRef.current = map;
    drawClientLocation(map, initialLocation);

    return () => {
      mapRef.current = null;
      map.remove();
    };
  }, []);

  useEffect(() => {
    const map = mapRef.current;
    if (!map) return;

    const location = { latitude, longitude };
    drawClientLocation(map, location);
    map.easeTo({ center: [longitude, latitude], duration: 500 });
  }, [latitude, longitude]);

  return (
    <div className="relative">
      <div ref={containerRef} className="h-72 w-full bg-[#eef2f0]" aria-label="Bản đồ vị trí tài xế" />
      {/* <div className="absolute bottom-3 left-3 z-10 rounded-lg bg-white/95 px-3 py-2 text-xs font-medium text-[#25322e] shadow">
        Vị trí hiện tại của bạn
      </div> */}
    </div>
  );
}
