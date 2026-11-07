"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { AlertCircle, CheckCircle2, ChevronRight, Clock3, LoaderCircle, MapPin, Navigation } from "lucide-react";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import { BottomNavigation } from "@/components/rider/BottomNavigation";
import { tripService } from "@/service/TripService";
import type { TripHistoryContract } from "@/service/contract/TripContract";

const formatPrice = (price: number | null) => new Intl.NumberFormat("vi-VN", {
  style: "currency",
  currency: "VND",
  maximumFractionDigits: 0,
}).format(price ?? 0);

const statusView = (status: string) => {
  if (status === "DroppedOff" || status === "Paid") {
    return { label: "Chuyến đi hoàn thành", className: "text-[#126b4f]", Icon: CheckCircle2 };
  }
  if (status === "Cancelled") {
    return { label: "Chuyến đi đã hủy", className: "text-[#8a5b35]", Icon: AlertCircle };
  }
  return { label: "Không tìm được tài xế", className: "text-[#a34b35]", Icon: AlertCircle };
};

/**
 * * Trip history page
 * @returns 
 */
export function ActivityScreen() {
  const router = useRouter();
  const [trips, setTrips] = useState<TripHistoryContract[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    tripService.getTripHistory()
      .then(setTrips)
      .catch(() => setError("Không thể tải lịch sử chuyến đi."))
      .finally(() => setLoading(false));
  }, []);

  return (
    <AuthenticatedScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="border-b border-[#edf0ee] bg-white px-5 pb-5 pt-[max(24px,env(safe-area-inset-top))]">
          <h1 className="text-2xl font-bold tracking-[-0.02em] text-[#17211e]">Lịch sử chuyến đi</h1>
          <p className="mt-1 text-sm text-[#71807a]">Các chuyến đi trước đây của bạn</p>
        </header>

        {loading && <div className="grid min-h-80 place-items-center"><LoaderCircle className="size-8 animate-spin text-[#126b4f]" aria-label="Đang tải lịch sử" /></div>}
        {error && <p role="alert" className="m-5 rounded-xl bg-[#fff0ec] p-4 text-sm text-[#9a422d]">{error}</p>}
        {!loading && !error && trips.length === 0 && (
          <section className="mt-3 bg-white px-5 py-16 text-center">
            <Clock3 className="mx-auto size-12 text-[#9ba6a2]" />
            <h2 className="mt-4 text-lg font-bold text-[#17211e]">Chưa có lịch sử chuyến đi</h2>
            <p className="mt-2 text-sm text-[#71807a]">Các chuyến đã hoàn thành hoặc hủy sẽ xuất hiện tại đây.</p>
          </section>
        )}

        <section className="space-y-3 px-5 py-5" aria-label="Các chuyến đi">
          {trips.map((trip) => {
            const { label, className, Icon } = statusView(trip.status);
            return (
              <button key={trip.uuid} type="button" onClick={() => router.push(`/trip/${trip.uuid}`)} className="w-full rounded-2xl border border-[#e3e8e6] bg-white p-4 text-left transition-colors hover:bg-[#fafcfb]">
                <div className="flex items-center justify-between gap-3 border-b border-[#edf0ee] pb-3">
                  <p className="flex items-center gap-1.5 text-xs text-[#74817c]"><Clock3 className="size-4" />{new Intl.DateTimeFormat("vi-VN", { dateStyle: "short", timeStyle: "short" }).format(new Date(trip.createdAt))}</p>
                  <div className="flex items-center gap-2"><p className="font-bold text-[#17211e]">{formatPrice(trip.price)}</p><ChevronRight className="size-4 text-[#89948f]" /></div>
                </div>
                <div className="mt-4 grid grid-cols-[20px_1fr] gap-x-3 gap-y-4">
                  <Navigation className="size-5 text-[#126b4f]" /><div><p className="text-xs text-[#85908c]">Điểm đón</p><p className="mt-0.5 font-medium text-[#25322e]">{trip.pickupName}</p></div>
                  <MapPin className="size-5 text-[#b05d43]" /><div><p className="text-xs text-[#85908c]">Điểm đến</p><p className="mt-0.5 font-medium text-[#25322e]">{trip.destinationName}</p></div>
                </div>
                <div className="mt-4 flex items-center justify-between gap-3 text-xs">
                  <p className={`flex items-center gap-1.5 font-medium ${className}`}><Icon className="size-4" />{label}</p>
                  <p className="text-[#71807a]">{trip.distanceKM ?? 0} km · {Math.ceil(trip.durationMin ?? 0)} phút</p>
                </div>
              </button>
            );
          })}
        </section>
        <BottomNavigation />
      </main>
    </AuthenticatedScreen>
  );
}
