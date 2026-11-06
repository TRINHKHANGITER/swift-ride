"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowRight, BriefcaseBusiness, Clock3, MapPin, Navigation } from "lucide-react";
import { DriverBottomNavigation } from "@/components/driver/DriverBottomNavigation";
import { DriverOnlyScreen } from "@/components/driver/DriverOnlyScreen";
import { tripService } from "@/service/TripService";
import type { TripContract } from "@/service/contract/TripContract";
import { tripStatusName } from "@/service/trip-status";

export function DriverActiveScreen() {
  const router = useRouter();
  const [trip, setTrip] = useState<TripContract | null>(null);

  useEffect(() => {
    tripService.getActiveTrip().then(setTrip).catch(() => setTrip(null));
  }, []);

  const status = trip ? tripStatusName(trip.status) : "";
  const statusLabel = status === "PickedUp" ? "Đang chở khách" : status === "EnRoute" ? "Đang tới điểm đón" : "Đã nhận chuyến";

  return (
    <DriverOnlyScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="border-b border-[#edf0ee] bg-white px-5 pb-5 pt-[max(22px,env(safe-area-inset-top))]">
          <h1 className="text-2xl font-bold text-[#17211e]">Đang làm</h1>
          <p className="mt-1 text-sm text-[#71807a]">Chuyến bạn đã nhận</p>
        </header>

        {trip ? (
          <button type="button" onClick={() => router.push(`/driver/trip/${trip.uuid}`)} className="mt-3 w-full bg-white px-5 py-5 text-left transition-colors hover:bg-[#fafcfb]">
            <div className="flex items-center justify-between">
              <div><p className="text-xs font-bold uppercase text-[#126b4f]">{statusLabel}</p><h2 className="mt-1 text-lg font-bold text-[#17211e]">Chi tiết chuyến đi</h2></div>
              <ArrowRight className="size-5 text-[#126b4f]" />
            </div>
            <div className="mt-5 space-y-4 border-y border-[#edf0ee] py-5">
              <div className="flex gap-3"><Navigation className="mt-0.5 size-5 shrink-0 text-[#126b4f]" /><div><p className="text-xs text-[#71807a]">Điểm đón</p><p className="font-medium text-[#17211e]">{trip.pickupName}</p></div></div>
              <div className="flex gap-3"><MapPin className="mt-0.5 size-5 shrink-0 text-[#b05d43]" /><div><p className="text-xs text-[#71807a]">Điểm đến</p><p className="font-medium text-[#17211e]">{trip.destinationName}</p></div></div>
            </div>
            <p className="mt-4 flex items-center gap-2 text-sm text-[#71807a]"><Clock3 className="size-4" /> {trip.distanceKM ?? 0} km · khoảng {Math.ceil(trip.durationMin ?? 0)} phút</p>
          </button>
        ) : (
          <section className="mt-3 bg-white px-5 py-16 text-center">
            <span className="mx-auto grid size-16 place-items-center rounded-full bg-[#eef3f1] text-[#51605b]"><BriefcaseBusiness className="size-8" /></span>
            <h2 className="mt-5 text-lg font-bold text-[#17211e]">Chưa có chuyến đang làm</h2>
            <p className="mt-2 text-sm text-[#71807a]">Chuyến đã nhận sẽ được hiển thị tại đây.</p>
          </section>
        )}

        <DriverBottomNavigation />
      </main>
    </DriverOnlyScreen>
  );
}
