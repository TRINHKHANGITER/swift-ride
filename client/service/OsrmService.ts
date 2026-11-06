import type { TripGeometry } from "@/service/contract/TripContract";

type Coordinate = { latitude: number; longitude: number };

type OsrmResponse = {
  code: string;
  routes: Array<{ geometry: TripGeometry }>;
};

class OsrmService {
  async getRoute(from: Coordinate, to: Coordinate): Promise<TripGeometry> {
    const coordinates = `${from.longitude},${from.latitude};${to.longitude},${to.latitude}`;
    const response = await fetch(
      `https://router.project-osrm.org/route/v1/driving/${coordinates}?overview=full&geometries=geojson`,
      { cache: "no-store" },
    );
    if (!response.ok) throw new Error("Không thể tải tuyến đường từ OSRM.");

    const result = await response.json() as OsrmResponse;
    const geometry = result.routes[0]?.geometry;
    if (result.code !== "Ok" || !geometry) throw new Error("OSRM không tìm thấy tuyến đường.");
    return geometry;
  }
}

export const osrmService = new OsrmService();
