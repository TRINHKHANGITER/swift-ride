"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { ArrowLeft, ChevronDown, Clock3, MapPin, Search } from "lucide-react";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import { useEffect, useRef, useState } from "react";
import {
  defaultPickupLocation,
  locations,
  LocationOption,
} from "@/service/out/resource-locations";
import { _searchLocation } from "@/service/_SearchLocation";
import { useQuery } from "@/lib/use-query";
import { tripService } from "@/service/TripService";

// Place data will be fetched from the server and sent by props
// const places = [
//   {
//     name: "488/9 Cộng Hòa",
//     detail: "8,27 km · Phường 13, Tân Bình, TP. Hồ Chí Minh",
//     recent: true,
//   },
//   {
//     name: "Bến xe Miền Đông mới",
//     detail: "12,4 km · 501 Hoàng Hữu Nam, TP. Thủ Đức",
//   },
//   {
//     name: "492 Nguyễn Thị Minh Khai",
//     detail: "2,56 km · Phường Bàn Cờ, TP. Hồ Chí Minh",
//   },
// ];

type Place = {
  name: string;
  detail: string;
  recent?: boolean;
};

type RecentSearch = string;

export function SearchScreen() {
  const router = useRouter();
  const searchParams = useSearchParams();

  // Recieved from props
  const places: Place[] = [];
  const recentSearches: RecentSearch[] = [];

  const { execute, inProcess } = useQuery();

  const pickupLocation =
    locations.find((location) => location.id === searchParams.get("pickup")) ??
    defaultPickupLocation;

  const isPickupMode = searchParams.get("mode") === "pickup";

  // Location search state
  const searchTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>("");
  const [searchResults, setSearchResults] = useState<LocationOption[]>([]);

  useEffect(() => {
    tripService.getActiveTrip().then((activeTrip) => {
      if (activeTrip) router.replace(`/trip/${activeTrip.uuid}`);
    }).catch(() => undefined);
  }, [router]);
  const visibleLocations =
    isPickupMode && searchQuery.trim() === "" ? locations.slice(0, 5) : searchResults && locations.slice(0, 5);

  const startPickupSearch = () => {
    setSearchQuery("");
    setSearchResults([]);
    router.push(`/search?mode=pickup&pickup=${pickupLocation.id}`);
  };

  const selectSearchResult = (result: LocationOption) => {
    setSearchQuery("");
    setSearchResults([]);

    if (isPickupMode) {
      router.replace(`/search?pickup=${result.id}`);
      return;
    }

    router.push(
      `/booking?pickup=${pickupLocation.id}&destination=${result.id}`,
    );
  };

  useEffect(() => {
    if (searchTimer.current) {
      clearTimeout(searchTimer.current);
    }

    searchTimer.current = setTimeout(async () => {
      if (searchQuery.trim() === "") {
        setSearchResults([]);
        return;
      }

      await execute(
        async () => {
          const results = await _searchLocation.searchLocation(searchQuery);
          setSearchResults(results);
        },
        {
          onError: (error) => {
            alert(`Lỗi khi tìm kiếm địa điểm: ${error.message}`);
          },
        },
      );
    }, 300);

    return () => {
      if (searchTimer.current) {
        clearTimeout(searchTimer.current);
      }
    };
  }, [execute, searchQuery]);

  return (
    <AuthenticatedScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-white pb-10 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="border-b border-[#edf0ee] px-5 pb-5 pt-[max(20px,env(safe-area-inset-top))]">
          {/* Change pickup location */}
          {isPickupMode ? (
            <div className="flex items-center gap-3">
              <button
                type="button"
                onClick={() => router.back()}
                className="grid size-10 shrink-0 place-items-center rounded-full hover:bg-[#f0f3f2]"
                aria-label="Quay lại màn đặt chuyến"
              >
                <ArrowLeft className="size-6" />
              </button>

              <label className="flex h-12 min-w-0 flex-1 items-center gap-3 rounded-xl border border-[#126b4f] bg-white px-3 shadow-[0_0_0_3px_rgba(18,107,79,0.1)]">
                <Search className="size-5 shrink-0 text-[#66736e]" />
                <input
                  autoFocus
                  type="search"
                  placeholder="Tìm điểm đón"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="h-full min-w-0 flex-1 bg-transparent text-base outline-none placeholder:text-[#929c98]"
                />
              </label>
            </div>
          ) : (
            <>
              <div className="flex items-center gap-3 mb-2">
                <button
                  type="button"
                  onClick={() => router.back()}
                  className="grid size-10 place-items-center rounded-full hover:bg-[#f0f3f2]"
                  aria-label="Quay lại"
                >
                  <ArrowLeft className="size-6" />
                </button>
                {/* Default section to render pickup location */}
                <button
                  type="button"
                  onClick={startPickupSearch}
                  className="min-w-0 flex-1 text-left"
                  aria-label={`Thay đổi điểm đón hiện tại: ${pickupLocation.name}`}
                >
                  <span className="block text-xs text-[#71807a]">Điểm đón</span>
                  <span className="mt-0.5 flex items-center gap-1 font-bold text-[#17211e]">
                    <span className="truncate">{pickupLocation.name}</span>
                    <ChevronDown className="size-4 shrink-0" />
                  </span>
                  <span className="mt-0.5 block truncate text-xs text-[#71807a]">
                    {pickupLocation.address}
                  </span>
                </button>
              </div>
              {/* Default section to render and search destination location */}
              <label className="flex h-12 min-w-0 flex-1 items-center gap-3 rounded-xl border border-[#126b4f] bg-white px-3 shadow-[0_0_0_3px_rgba(18,107,79,0.1)]">
                <Search className="size-5 text-[#66736e]" />
                <input
                  autoFocus
                  type="search"
                  placeholder="Tìm địa điểm đến"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="h-full min-w-0 flex-1 bg-transparent text-base outline-none placeholder:text-[#929c98]"
                />
              </label>
            </>
          )}
        </header>

        {/* Search results */}
        {inProcess && searchQuery.trim() !== "" && (
          <section
            className="px-5 py-7"
            aria-live="polite"
            aria-busy="true"
            aria-label="Đang tìm kiếm địa điểm"
          >
            <div className="flex items-center gap-2 text-base font-medium text-[#66736e]">
              <span className="size-5 animate-spin rounded-full border-3 border-[#cbd5d1] border-t-[#126b4f]" />
              <h2
                id="search-results-title"
                className="text-lg font-bold text-[#17211e]"
              >
                Đang tìm kiếm địa điểm
              </h2>
            </div>

            <div className="mt-5 space-y-5" aria-hidden="true">
              {[1, 2, 3].map((item) => (
                <div key={item} className="flex animate-pulse gap-3">
                  <div className="size-10 shrink-0 rounded-full bg-[#e8eeeb]" />
                  <div className="flex-1 space-y-2 py-1">
                    <div className="h-4 w-2/3 rounded-full bg-[#e8eeeb]" />
                    <div className="h-3 w-full rounded-full bg-[#f0f3f2]" />
                  </div>
                </div>
              ))}
            </div>
          </section>
        )}

        {!inProcess && visibleLocations.length > 0 && (
          <section className="px-5 py-7" aria-labelledby="search-results-title">
            <h2
              id="search-results-title"
              className="text-xl font-bold text-[#17211e]"
            >
              {isPickupMode && searchQuery.trim() === ""
                ? "Địa điểm gợi ý"
                : "Kết quả tìm kiếm"}
            </h2>
            <div className="mt-4">
              {visibleLocations.map((result) => (
                <button
                  key={result.id}
                  type="button"
                  onClick={() => selectSearchResult(result)}
                  className="flex w-full gap-3 border-b border-[#edf0ee] py-4 text-left last:border-0"
                >
                  <span className="mt-0.5 grid size-10 shrink-0 place-items-center rounded-full bg-[#e8f5ef] text-[#126b4f]">
                    <MapPin className="size-5" />
                  </span>
                  <span className="min-w-0">
                    <span className="block font-bold leading-snug text-[#17211e]">
                      {result.name}
                    </span>
                    <span className="mt-1 block text-sm leading-relaxed text-[#75817d]">
                      {result.address}
                    </span>
                  </span>
                </button>
              ))}
            </div>
          </section>
        )}

        {/* Destination options */}
        {!isPickupMode && (
          <>
            <section className="px-5 py-7" aria-labelledby="book-title">
              <h1 id="book-title" className="text-xl font-bold text-[#17211e]">
                Đặt chuyến
              </h1>
              <div className="mt-4">
                {places.map((place) => (
                  <button
                    key={place.name}
                    type="button"
                    className="flex w-full gap-3 border-b border-[#edf0ee] py-4 text-left last:border-0"
                  >
                    <span className="mt-0.5 grid size-10 shrink-0 place-items-center rounded-full bg-[#e8f5ef] text-[#126b4f]">
                      {place.recent ? (
                        <Clock3 className="size-5" />
                      ) : (
                        <MapPin className="size-5" />
                      )}
                    </span>
                    <span className="min-w-0">
                      <span className="block font-bold leading-snug text-[#17211e]">
                        {place.name}
                      </span>
                      <span className="mt-1 block text-sm leading-relaxed text-[#75817d]">
                        {place.detail}
                      </span>
                    </span>
                  </button>
                ))}
              </div>
            </section>

            <section
              className="border-t-8 border-[#f4f6f5] px-5 py-7"
              aria-labelledby="recent-title"
            >
              <h2
                id="recent-title"
                className="text-xl font-bold text-[#17211e]"
              >
                Tìm kiếm gần đây
              </h2>
              <div className="mt-4 flex flex-wrap gap-2">
                {recentSearches.map((search) => (
                  <button
                    key={search}
                    type="button"
                    className="flex items-center gap-1.5 rounded-full bg-[#f3f5f4] px-3.5 py-2.5 text-sm text-[#46534e]"
                  >
                    <Clock3 className="size-4" /> {search}
                  </button>
                ))}
              </div>
            </section>
          </>
        )}
      </main>
    </AuthenticatedScreen>
  );
}
