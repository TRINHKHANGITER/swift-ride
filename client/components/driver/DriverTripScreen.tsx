"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft, CheckCircle2, LoaderCircle, MapPin, Navigation, Route } from "lucide-react";
import { DriverOnlyScreen } from "@/components/driver/DriverOnlyScreen";
import ViewMapLine from "@/components/map/ViewMapLine";
import { Button } from "@/components/ui/button";
import { tripService } from "@/service/TripService";
import type { TripContract } from "@/service/contract/TripContract";
import { tripStatusName } from "@/service/trip-status";
import { useTripRoutes } from "@/lib/use-trip-routes";
import { osrmService } from "@/service/OsrmService";

const distanceMeters = (lat1: number, lon1: number, lat2: number, lon2: number) => {
  const toRadians = (value: number) => value * Math.PI / 180;
  const dLat = toRadians(lat2 - lat1);
  const dLon = toRadians(lon2 - lon1);
  const a = Math.sin(dLat / 2) ** 2 + Math.cos(toRadians(lat1)) * Math.cos(toRadians(lat2)) * Math.sin(dLon / 2) ** 2;
  return 6_371_000 * 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
};

export function DriverTripScreen({ tripUuid }: { tripUuid: string }) {
  const router = useRouter();
  const [trip, setTrip] = useState<TripContract | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [driverAnimation, setDriverAnimation] = useState<{ key: number; coordinates: number[][]; durationMs: number } | null>(null);
  const [movementRoute, setMovementRoute] = useState<{ type: string; coordinates: number[][] } | null>(null);
  const [movementTarget, setMovementTarget] = useState<{ latitude: number; longitude: number } | null>(null);
  const [originalLocation, setOriginalLocation] = useState<{ latitude: number; longitude: number } | null>(null);
  const previousStatusRef = useRef<string | null>(null);
  const { tripRoute, driverRoute } = useTripRoutes(trip);

  const loadTrip = useCallback(async () => {
    try {
      const result = await tripService.getTrip(tripUuid);
      if (result.driverLatitude !== null && result.driverLongitude !== null) {
        setOriginalLocation((currentLocation) => currentLocation ?? {
          latitude: result.driverLatitude!,
          longitude: result.driverLongitude!,
        });
      }
      setTrip(result);
    } catch { setError("Không thể tải chuyến đi."); }
  }, [tripUuid]);

  const status = trip ? tripStatusName(trip.status) : "";
  const isTerminalStatus = status === "Paid" || status === "Cancelled" || status === "Failed";

  useEffect(() => {
    if (isTerminalStatus) return;

    const initial = window.setTimeout(() => void loadTrip(), 0);
    const interval = window.setInterval(() => void loadTrip(), 3_000);
    return () => { window.clearTimeout(initial); window.clearInterval(interval); };
  }, [isTerminalStatus, loadTrip]);

  const isCarryingPassenger = status === "PickedUp";
  const isAwaitingPayment = status === "DroppedOff" || status === "PaymentPending";
  const isPaid = status === "Paid";
  const isFinished = isAwaitingPayment || isPaid;

  useEffect(() => {
    if (!status) return;

    const previousStatus = previousStatusRef.current;
    if (previousStatus && previousStatus !== status) {
      const statusTitle = {
        EnRoute: "Đang tới điểm đón",
        PickedUp: "Đã đón khách",
        DroppedOff: "Đã trả khách",
        PaymentPending: "Đang chờ khách thanh toán",
        Paid: "Khách đã thanh toán",
      }[status] ?? "Trạng thái chuyến đi đã thay đổi";
      window.alert(`Cập nhật chuyến đi: ${statusTitle}`);
    }

    previousStatusRef.current = status;

    if (status === "Paid") {
      window.localStorage.removeItem("swift-ride-active-trip");
      router.replace("/driver");
    }
  }, [router, status]);
  const current = trip && trip.driverLatitude !== null && trip.driverLongitude !== null
    ? { latitude: trip.driverLatitude, longitude: trip.driverLongitude } : null;
  const operationTarget = trip ? (isCarryingPassenger
    ? { latitude: trip.destinationLatitude, longitude: trip.destinationLongitude }
    : { latitude: trip.pickupLatitude, longitude: trip.pickupLongitude }) : null;
  const isAtOperationTarget = current && operationTarget
    ? distanceMeters(current.latitude, current.longitude, operationTarget.latitude, operationTarget.longitude) < 80
    : false;

  const moveDriverTo = async (target: { latitude: number; longitude: number } | null) => {
    if (!trip || !current || !target || busy) return;
    setBusy(true); setError("");
    try {
      if (distanceMeters(current.latitude, current.longitude, target.latitude, target.longitude) < 10) {
        setTrip(await tripService.updateDriverLocation(trip.uuid, target.latitude, target.longitude));
        setBusy(false);
        return;
      }

      const route = await osrmService.getRoute(current, target);
      setMovementTarget(target);
      setMovementRoute(route);
      setDriverAnimation({ key: Date.now(), coordinates: route.coordinates, durationMs: 5_000 });
    } catch {
      setError("Không thể bắt đầu di chuyển.");
      setBusy(false);
    }
  };

  const finishMovement = useCallback(async () => {
    if (!trip || !movementTarget) return;
    try {
      setTrip(await tripService.updateDriverLocation(
        trip.uuid,
        movementTarget.latitude,
        movementTarget.longitude,
      ));
    } catch { setError("Không thể cập nhật vị trí tài xế."); }
    finally {
      setDriverAnimation(null);
      setMovementRoute(null);
      setMovementTarget(null);
      setBusy(false);
    }
  }, [movementTarget, trip]);

  const advanceTrip = async () => {
    if (!trip || busy || !isAtOperationTarget) return;
    setBusy(true); setError("");
    try {
      if (isCarryingPassenger) {
        setTrip(await tripService.dropoffTrip(trip.uuid));
        window.localStorage.removeItem("swift-ride-active-trip");
      } else {
        setTrip(await tripService.pickupTrip(trip.uuid));
      }
    } catch { setError("Không thể cập nhật trạng thái chuyến đi."); }
    finally { setBusy(false); }
  };

  return (
    <DriverOnlyScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-72 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="flex items-center gap-3 border-b border-[#edf0ee] bg-white px-5 pb-4 pt-[max(16px,env(safe-area-inset-top))]">
          <button type="button" onClick={() => router.push("/driver/active")} className="grid size-10 place-items-center rounded-full hover:bg-[#f0f3f2]" aria-label="Quay lại"><ArrowLeft className="size-5" /></button>
          <div><h1 className="text-lg font-bold text-[#17211e]">Chi tiết chuyến đi</h1><p className="text-xs text-[#71807a]">{isPaid ? "Đã thanh toán" : isAwaitingPayment ? "Đang chờ khách thanh toán" : isCarryingPassenger ? "Đang tới điểm trả" : "Đang tới điểm đón"}</p></div>
        </header>

        {!trip && !error && <div className="grid min-h-80 place-items-center"><LoaderCircle className="size-8 animate-spin text-[#126b4f]" /></div>}
        {error && <p role="alert" className="m-5 rounded-xl bg-[#fff0ec] p-3 text-sm text-[#9a422d]">{error}</p>}

        {trip && <div className="space-y-3">
          <section className="overflow-hidden bg-white">
            <ViewMapLine
              pickup={{ name: trip.pickupName, latitude: trip.pickupLatitude, longitude: trip.pickupLongitude }}
              destination={{ name: trip.destinationName, latitude: trip.destinationLatitude, longitude: trip.destinationLongitude }}
              geometry={tripRoute}
              driverGeometry={movementRoute ?? driverRoute}
              driverLocation={current}
              driverAnimation={driverAnimation}
              onDriverAnimationComplete={finishMovement}
            />
          </section>
          <section className="space-y-4 bg-white px-5 py-5">
            <div className="flex gap-3"><Navigation className="size-5 shrink-0 text-[#126b4f]" /><div><p className="text-xs text-[#71807a]">Điểm đón</p><p className="font-medium">{trip.pickupName}</p></div></div>
            <div className="flex gap-3"><MapPin className="size-5 shrink-0 text-[#b05d43]" /><div><p className="text-xs text-[#71807a]">Điểm trả</p><p className="font-medium">{trip.destinationName}</p></div></div>
          </section>
        </div>}

        {trip && <aside className="fixed inset-x-0 bottom-0 z-30 mx-auto w-full max-w-140 border-t border-[#dfe6e3] bg-white px-5 pb-[max(18px,env(safe-area-inset-bottom))] pt-4 shadow-[0_-12px_30px_rgba(15,45,35,0.12)]" aria-label="Toolbox mô phỏng tài xế">
          <div className="flex items-center gap-2 text-sm font-bold text-[#17211e]"><Route className="size-4 text-[#126b4f]" /> Toolbox mô phỏng</div>
          {isFinished ? (
            <div className="mt-3 flex items-center gap-3 rounded-xl bg-[#e8f3ef] p-4 text-[#126b4f]">
              {isPaid ? <CheckCircle2 className="size-6" /> : <LoaderCircle className="size-6 animate-spin" />}
              <span className="font-bold">{isPaid ? "Khách đã thanh toán" : status === "PaymentPending" ? "Đang chờ khách thanh toán" : "Đang tạo hóa đơn thanh toán"}</span>
            </div>
          ) : (
            <>
              <p className="mt-2 text-xs text-[#71807a]">Chọn nhanh vị trí để kiểm thử hành trình của tài xế.</p>
              <div className="mt-3 grid grid-cols-3 gap-2">
                <Button variant="outline" className="h-10 rounded-xl px-2 text-xs" disabled={busy || !originalLocation} onClick={() => void moveDriverTo(originalLocation)}>Vị trí cũ</Button>
                <Button variant="outline" className="h-10 rounded-xl px-2 text-xs" disabled={busy} onClick={() => void moveDriverTo({ latitude: trip.pickupLatitude, longitude: trip.pickupLongitude })}>Điểm đón</Button>
                <Button variant="outline" className="h-10 rounded-xl px-2 text-xs" disabled={busy} onClick={() => void moveDriverTo({ latitude: trip.destinationLatitude, longitude: trip.destinationLongitude })}>Điểm trả</Button>
              </div>
              <div className="mt-4 border-t border-[#edf0ee] pt-4">
                <p className="text-xs font-medium text-[#71807a]">Thao tác chuyến đi</p>
                <Button className="mt-2 h-12 w-full rounded-xl bg-[#126b4f] hover:bg-[#0e5942]" disabled={busy || !isAtOperationTarget} onClick={() => void advanceTrip()}>
                  {busy && <LoaderCircle className="animate-spin" />}{isCarryingPassenger ? "Trả khách" : "Đón khách"}
                </Button>
                {!isAtOperationTarget && <p className="mt-2 text-center text-[11px] text-[#8a5b35]">Cần di chuyển tới {isCarryingPassenger ? "điểm trả" : "điểm đón"} trước.</p>}
              </div>
            </>
          )}
        </aside>}
      </main>
    </DriverOnlyScreen>
  );
}
