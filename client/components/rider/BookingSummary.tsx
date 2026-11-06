"use client";

import {
  ArrowLeft,
  Bike,
  ChevronRight,
  Clock3,
  LoaderCircle,
  MapPin,
  Route,
} from "lucide-react";
import { useRouter, useSearchParams } from "next/navigation";
import { useState } from "react";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import { locations } from "@/service/out/resource-locations";
import ViewMapLine from "../map/ViewMapLine";
import { Button } from "../ui/button";
import { tripService } from "@/service/TripService";
import { CreateTripResponseContract } from "@/service/contract/TripContract";

const formatPrice = (price: number, currency: string = "VND") =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency,
    maximumFractionDigits: 0,
  }).format(price);

export function BookingSummary() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pickup = locations.find(
    (location) => location.id === searchParams.get("pickup"),
  );
  const destination = locations.find(
    (location) => location.id === searchParams.get("destination"),
  );
  const [trip, setTrip] = useState<CreateTripResponseContract | null>(null);
  const [tripUuid, setTripUuid] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isConfirming, setIsConfirming] = useState(false);
  const [error, setError] = useState("");

  const createTrip = async () => {
    if (!pickup || !destination || isLoading) return;

    setIsLoading(true);
    setError("");

    try {
      const result = await tripService.createTrip({
        pickup: {
          name: pickup.name,
          latitude: pickup.latitude,
          longitude: pickup.longitude,
        },
        destination: {
          name: destination.name,
          latitude: destination.latitude,
          longitude: destination.longitude,
        },
      });
      setTrip(result);
      setTripUuid(result.uuid);
    } catch {
      setError("Chưa thể tạo chuyến đi. Vui lòng kiểm tra kết nối và thử lại.");
    } finally {
      setIsLoading(false);
    }
  };

  const confirmTrip = async () => {
    if (!tripUuid || isConfirming) return;

    setIsConfirming(true);
    setError("");
    try {
      const result = await tripService.confirmTrip(tripUuid);
      if (!result.isSuccess) {
        setError("Hiện chưa có tài xế phù hợp. Vui lòng thử lại sau.");
        return;
      }
      router.push(`/trip/${result.tripUuid}`);
    } catch (requestError) {
      const message =
        typeof requestError === "object" &&
        requestError !== null &&
        "response" in requestError
          ? (requestError as { response?: { data?: { message?: string } } }).response
              ?.data?.message
          : undefined;
      setError(message ?? "Chưa thể đặt xe. Vui lòng thử lại.");
    } finally {
      setIsConfirming(false);
    }
  };

  const handleBack = () => {
    if (tripUuid) {
      setTrip(null);
      setTripUuid(null);
      setError("");
      return;
    }
    router.back();
  };

  return (
    <AuthenticatedScreen>
      <main className="mx-auto flex min-h-dvh w-full max-w-140 flex-col bg-white shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="flex items-center gap-3 border-b border-[#edf0ee] px-5 pb-4 pt-[max(16px,env(safe-area-inset-top))]">
          <button
            type="button"
            onClick={handleBack}
            className="grid size-10 place-items-center rounded-full text-[#17211e] transition-colors hover:bg-[#f0f3f2]"
            aria-label="Quay lại"
          >
            <ArrowLeft className="size-5" />
          </button>
          <h1 className="text-lg font-bold text-[#17211e]">
            {tripUuid ? "Báo giá chuyến đi" : "Chuyến đi đã chọn"}
          </h1>
        </header>

        <div className="flex-1 space-y-6 px-5 pb-28 pt-6">
          <section>
            <div className="relative space-y-6">
              <div className="absolute left-2.25 top-5 h-[calc(100%-40px)] w-px border-l border-dashed border-[#b7c7c1]" />

              <div className="relative flex gap-4">
                <span className="mt-1 grid size-4.75 shrink-0 place-items-center rounded-full ring-4 ring-white">
                  <span className="size-2 rounded-full bg-[#126b4f]" />
                </span>
                <div className="min-w-0">
                  <p className="text-xs font-medium uppercase tracking-wide text-[#71807a]">
                    Điểm đón
                  </p>
                  <p className="mt-1 font-bold leading-snug text-[#17211e]">
                    {pickup?.name ?? "Chưa chọn điểm đón"}
                  </p>
                  {pickup && (
                    <p className="mt-1 text-sm leading-relaxed text-[#71807a]">
                      {pickup.address}
                    </p>
                  )}
                </div>
              </div>

              <div className="relative flex gap-4">
                <span className="mt-1 grid size-4.75 shrink-0 place-items-center rounded-full ring-4 ring-white">
                  <MapPin className="size-3 text-[#b05d43]" fill="currentColor" />
                </span>
                <div className="min-w-0">
                  <p className="text-xs font-medium uppercase tracking-wide text-[#71807a]">
                    Điểm đến
                  </p>
                  <p className="mt-1 font-bold leading-snug text-[#17211e]">
                    {destination?.name ?? "Chưa chọn điểm đến"}
                  </p>
                  {destination && (
                    <p className="mt-1 text-sm leading-relaxed text-[#71807a]">
                      {destination.address}
                    </p>
                  )}
                </div>
              </div>
            </div>
          </section>

          <section className="overflow-hidden rounded-xl border border-[#e3e8e6] bg-white">
            <ViewMapLine
              pickup={pickup}
              destination={destination}
              geometry={trip?.geometry}
            />
          </section>

          {trip && (
            <section className="border-t border-[#edf0ee] pt-5">
              <div className="flex items-start justify-between gap-4 border-b border-[#edf1ef] pb-5">
                <div>
                  <div className="flex items-center gap-2 text-sm text-[#71807a]">
                    <Bike className="size-4 text-[#126b4f]" />
                    Giá chuyến dự kiến
                  </div>
                  <p className="mt-1 text-2xl font-bold tracking-tight text-[#17211e]">
                    {formatPrice(trip.price, trip.currency)}
                  </p>
                </div>
              </div>

              <div className="grid grid-cols-2 divide-x divide-[#edf0ee] py-5">
                <div className="flex items-center gap-3 pr-4">
                  <Route className="size-5 shrink-0 text-[#66736e]" />
                  <div>
                  <p className="font-bold text-[#17211e]">
                    {trip.distanceKM.toLocaleString("vi-VN", {
                      maximumFractionDigits: 1,
                    })} km
                  </p>
                  <p className="text-xs text-[#71807a]">Quãng đường</p>
                  </div>
                </div>
                <div className="flex items-center gap-3 pl-4">
                  <Clock3 className="size-5 shrink-0 text-[#66736e]" />
                  <div>
                  <p className="font-bold text-[#17211e]">
                    Khoảng {Math.max(1, Math.ceil(trip.durationMin))} phút
                  </p>
                  <p className="text-xs text-[#71807a]">{trip.durationMin} phút</p>
                  <p className="text-xs text-[#71807a]">Thời gian đi</p>
                  </div>
                </div>
              </div>
            </section>
          )}

          {error && (
            <div role="alert" className="rounded-2xl bg-[#fff0ec] px-4 py-3 text-sm text-[#9a422d]">
              {error}
            </div>
          )}
        </div>

        <section className="fixed inset-x-0 bottom-0 z-20 mx-auto w-full max-w-140 border-t border-[#edf0ee] bg-white px-5 pb-[max(16px,env(safe-area-inset-bottom))] pt-3">
          <div className="flex items-center gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={handleBack}
              className="h-13 rounded-xl border-[#d8e1dd] bg-white px-4 text-sm font-bold text-[#40504a] shadow-none hover:bg-[#f5f8f7] hover:text-[#17211e]"
            >
              <ArrowLeft className="size-4" />
              Quay lại
            </Button>
            <Button
              type="button"
              disabled={!pickup || !destination || isLoading || isConfirming}
              onClick={tripUuid ? confirmTrip : createTrip}
              className="h-13 flex-1 rounded-xl bg-[#126b4f] px-5 text-base font-bold text-white shadow-none hover:bg-[#0e5942]"
            >
              {isLoading || isConfirming ? (
                <>
                  <LoaderCircle className="size-5 animate-spin" />
                  {isConfirming ? "Đang tìm tài xế..." : "Đang tính giá..."}
                </>
              ) : tripUuid ? (
                <>
                  <Bike className="size-5" />
                  Đặt xe
                </>
              ) : (
                <>
                  Xem báo giá
                  <ChevronRight className="size-5" />
                </>
              )}
            </Button>
          </div>
          {tripUuid && (
            <p className="mt-2 text-center text-xs text-[#71807a]">
              Giá có thể thay đổi theo tình hình thực tế
            </p>
          )}
        </section>
      </main>
    </AuthenticatedScreen>
  );
}
