import { LocationOption, locations } from "./out/resource-locations";

class _SearchLocation {
    private _fakeLocations: LocationOption[] = [];
    constructor() {
        this._fakeLocations = locations;
    }

    async searchLocation(query: string): Promise<LocationOption[]> {
        await new Promise((resolve) => setTimeout(resolve, 500));

        const filteredLocations = this._fakeLocations.filter((location) =>
            location.name.toLowerCase().includes(query.toLowerCase()) ||
            location.address.toLowerCase().includes(query.toLowerCase()) ||
            location.keywords.some((keyword) =>
                keyword.toLowerCase().includes(query.toLowerCase())
            )
        );

        return filteredLocations;
    }
}

export const _searchLocation = new _SearchLocation();