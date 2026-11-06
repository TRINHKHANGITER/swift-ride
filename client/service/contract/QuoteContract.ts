export type GeoLocationContract = {
    latitude: number;
    longitude: number;
}

export type Geometry = {
    type: string;
    coordinates: number[][];
}

export type QuoteRequestContract = {
    pickup: GeoLocationContract;
    destination: GeoLocationContract;
}

export type QuoteResponseContract = {
    geometry: Geometry;
    weight: number;
    distanceKM: number;
    durationMin: number;
    price: number;
    currency: string;
}