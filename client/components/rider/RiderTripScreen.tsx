"use client";

import Image from "next/image";
import { useCallback, useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import {
  ArrowLeft,
  Bike,
  Clock3,
  CircleCheck,
  House,
  LoaderCircle,
  MapPin,
  Phone,
  QrCode,
  Route,
  UserRound,
  X,
} from "lucide-react";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import ViewMapLine from "@/components/map/ViewMapLine";
import { Button } from "@/components/ui/button";
import { tripService } from "@/service/TripService";
import type { TripContract } from "@/service/contract/TripContract";
import { tripStatusName } from "@/service/trip-status";
import { useTripRoutes } from "@/lib/use-trip-routes";

const formatPrice = (price: number | null) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(price ?? 0);

const statusContents: Record<string, readonly [string, string]> = {
  Requested: ["Đang chờ tài xế xác nhận", "Yêu cầu đã được gửi tới tài xế. Vui lòng chờ trong giây lát."],
  DriverAccepted: ["Tài xế đã nhận chuyến", "Tài xế đang chuẩn bị đến điểm đón của bạn."],
  EnRoute: ["Tài xế đang đến", "Tài xế đang di chuyển tới điểm đón."],
  PickedUp: ["Chuyến đi đang diễn ra", "Chúc bạn có một chuyến đi an toàn."],
  DroppedOff: ["Đã đến nơi", "Chuyến đi đã hoàn thành và hệ thống đang tạo thông tin thanh toán."],
  PaymentPending: ["Đang chờ thanh toán", "Quét mã QR bên dưới để thanh toán chuyến đi."],
  Paid: ["Thanh toán thành công", "Cảm ơn bạn đã sử dụng Swift Ride."],
  Failed: ["Chưa tìm được tài xế", "Vui lòng quay lại và đặt một chuyến mới."],
  Cancelled: ["Chuyến đi đã hủy", "Chuyến đi này không còn hoạt động."],
};

export function RiderTripScreen({ tripUuid }: { tripUuid: string }) {
  const router = useRouter();
  const [trip, setTrip] = useState<TripContract | null>(null);
  const [error, setError] = useState("");
  const [isCancelling, setIsCancelling] = useState(false);
  const previousStatusRef = useRef<string | null>(null);
  const { tripRoute, driverRoute } = useTripRoutes(trip);

  const loadTrip = useCallback(async () => {
    try {
      setTrip(await tripService.getTrip(tripUuid));
      setError("");
    } catch {
      setError("Không thể tải thông tin chuyến đi.");
    }
  }, [tripUuid]);

  const status = trip ? tripStatusName(trip.status) : "";
  const isTerminalStatus = status === "Paid" || status === "Cancelled" || status === "Failed";
  const canCancel = status === "Requested" || status === "DriverAccepted" || status === "EnRoute";

  useEffect(() => {
    if (isTerminalStatus) return;

    const initialLoad = window.setTimeout(() => void loadTrip(), 0);
    const interval = window.setInterval(() => void loadTrip(), 3_000);
    return () => {
      window.clearTimeout(initialLoad);
      window.clearInterval(interval);
    };
  }, [isTerminalStatus, loadTrip]);

  const statusContent = statusContents[status] ?? ["Đang cập nhật chuyến đi", "Thông tin mới nhất sẽ tự động hiển thị tại đây."];

  useEffect(() => {
    if (!status) return;

    const previousStatus = previousStatusRef.current;
    if (previousStatus && previousStatus !== status) {
      const statusTitle = statusContents[status]?.[0] ?? "Trạng thái chuyến đi đã thay đổi";
      window.alert(`Cập nhật chuyến đi: ${statusTitle}`);
    }

    previousStatusRef.current = status;
  }, [status]);

  const cancelTrip = async () => {
    if (!trip || !canCancel || isCancelling) return;
    if (!window.confirm("Bạn có chắc muốn hủy chuyến đi này?")) return;

    setIsCancelling(true);
    setError("");
    try {
      setTrip(await tripService.cancelTrip(trip.uuid));
    } catch {
      await loadTrip();
      setError("Không thể hủy chuyến đi. Vui lòng thử lại.");
    } finally {
      setIsCancelling(false);
    }
  };

  return (
    <AuthenticatedScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-8 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="flex items-center justify-between gap-4 border-b border-[#edf0ee] bg-white px-5 pb-5 pt-[max(22px,env(safe-area-inset-top))]">
          <div>
            <p className="text-sm text-[#71807a]">Chuyến đi của bạn</p>
            <h1 className="mt-1 text-2xl font-bold text-[#17211e]">Theo dõi chuyến đi</h1>
          </div>
          <button type="button" onClick={() => router.push("/")} className="grid size-11 shrink-0 place-items-center rounded-full border border-[#dce5e1] bg-white text-[#126b4f] transition-colors hover:bg-[#edf6f2]" aria-label="Về trang chủ" title="Về trang chủ">
            <House className="size-5" />
          </button>
        </header>

        {!trip && !error && (
          <div className="grid min-h-80 place-items-center text-[#126b4f]">
            <LoaderCircle className="size-8 animate-spin" aria-label="Đang tải chuyến đi" />
          </div>
        )}

        {error && !trip && <p className="m-5 rounded-xl bg-[#fff0ec] p-4 text-sm text-[#9a422d]">{error}</p>}

        {trip && (
          <div className="space-y-3">
            <section className="bg-white px-5 py-5" aria-live="polite">
              <div className="flex items-center gap-4">
                <span className={`grid size-12 shrink-0 place-items-center rounded-full ${status === "Failed" || status === "Cancelled" ? "bg-[#fff0ec] text-[#a34b35]" : "bg-[#e8f3ef] text-[#126b4f]"}`}>
                  {status === "Requested" ? <LoaderCircle className="size-6 animate-spin" /> : <Bike className="size-6" />}
                </span>
                <div>
                  <h2 className="font-bold text-[#17211e]">{statusContent[0]}</h2>
                  <p className="mt-1 text-sm leading-relaxed text-[#71807a]">{statusContent[1]}</p>
                </div>
              </div>
            </section>

            {error && (
              <p
                role="alert"
                className="mx-5 rounded-xl bg-[#fff0ec] px-4 py-3 text-sm text-[#9a422d]"
              >
                {error}
              </p>
            )}

            <section className="overflow-hidden bg-white">
              <ViewMapLine
                pickup={{ name: trip.pickupName, latitude: trip.pickupLatitude, longitude: trip.pickupLongitude }}
                destination={{ name: trip.destinationName, latitude: trip.destinationLatitude, longitude: trip.destinationLongitude }}
                geometry={tripRoute}
                driverGeometry={driverRoute}
                driverLocation={trip.driverLatitude !== null && trip.driverLongitude !== null ? { latitude: trip.driverLatitude, longitude: trip.driverLongitude } : null}
              />
            </section>

            {trip.driver && (
              <section className="bg-white px-5 py-5">
                <p className="text-xs font-bold uppercase tracking-wide text-[#71807a]">Tài xế của bạn</p>
                <div className="mt-3 flex items-center gap-4">
                  <span className="grid size-12 shrink-0 place-items-center rounded-full bg-[#e8f3ef] text-[#126b4f]"><UserRound className="size-6" /></span>
                  <div className="min-w-0 flex-1">
                    <h2 className="font-bold text-[#17211e]">{trip.driver.fullname}</h2>
                    <p className="mt-0.5 text-sm text-[#71807a]">{trip.driver.vehicleName} · {trip.driver.licensePlate}</p>
                  </div>
                  <a href={`tel:${trip.driver.phone}`} className="grid size-11 place-items-center rounded-full bg-[#126b4f] text-white" aria-label="Gọi tài xế"><Phone className="size-5" /></a>
                </div>
              </section>
            )}

            <section className="bg-white px-5 py-5">
              <div className="space-y-5">
                <div className="flex gap-3"><Route className="mt-0.5 size-5 shrink-0 text-[#126b4f]" /><div><p className="text-xs text-[#71807a]">Điểm đón</p><p className="font-medium text-[#17211e]">{trip.pickupName}</p></div></div>
                <div className="flex gap-3"><MapPin className="mt-0.5 size-5 shrink-0 text-[#b05d43]" /><div><p className="text-xs text-[#71807a]">Điểm đến</p><p className="font-medium text-[#17211e]">{trip.destinationName}</p></div></div>
              </div>
              <div className="mt-5 grid grid-cols-3 gap-3 border-t border-[#edf0ee] pt-5 text-center">
                <div><p className="font-bold text-[#17211e]">{trip.distanceKM ?? 0} km</p><p className="text-xs text-[#71807a]">Quãng đường</p></div>
                <div><p className="flex items-center justify-center gap-1 font-bold text-[#17211e]"><Clock3 className="size-4" />{Math.ceil(trip.durationMin ?? 0)} phút</p><p className="text-xs text-[#71807a]">Thời gian</p></div>
                <div><p className="font-bold text-[#17211e]">{formatPrice(trip.price)}</p><p className="text-xs text-[#71807a]">Giá dự kiến</p></div>
              </div>
            </section>

            {trip.qrUrl && status === "PaymentPending" && (
              <section className="bg-white px-5 py-6 text-center" aria-live="polite">
                <span className="mx-auto grid size-12 place-items-center rounded-full bg-[#e8f3ef] text-[#126b4f]">
                  <QrCode className="size-6" />
                </span>
                <h2 className="mt-3 text-lg font-bold text-[#17211e]">Thanh toán chuyến đi</h2>
                <p className="mt-1 text-sm text-[#71807a]">Quét mã QR bằng ứng dụng ngân hàng để thanh toán {formatPrice(trip.price)}.</p>
                <div className="mx-auto mt-5 w-fit overflow-hidden rounded-2xl border border-[#dce5e1] bg-white p-3 shadow-sm">
                  <Image
                    src={trip.qrUrl}
                    alt="Mã QR thanh toán chuyến đi"
                    width={280}
                    height={280}
                    className="size-70 object-contain"
                  />
                </div>
                <a href={trip.qrUrl} target="_blank" rel="noreferrer" className="mt-4 inline-block text-sm font-semibold text-[#126b4f] underline underline-offset-4">
                  Mở mã QR trong tab mới
                </a>
              </section>
            )}

            {status === "Paid" && (
              <section className="bg-white px-5 py-6 text-center" aria-live="polite">
                <span className="mx-auto grid size-12 place-items-center rounded-full bg-[#e8f3ef] text-[#126b4f]">
                  <CircleCheck className="size-7" />
                </span>
                <h2 className="mt-3 text-lg font-bold text-[#17211e]">Đã thanh toán</h2>
                <p className="mt-1 text-sm text-[#71807a]">Khoản thanh toán {formatPrice(trip.price)} đã được xác nhận.</p>
              </section>
            )}

            {canCancel && (
              <section className="bg-white px-5 py-5">
                <Button
                  type="button"
                  variant="outline"
                  disabled={isCancelling}
                  onClick={() => void cancelTrip()}
                  className="h-12 w-full rounded-xl border-[#d26a50] text-[#a34b35] hover:bg-[#fff0ec] hover:text-[#8f3f2c]"
                >
                  {isCancelling ? (
                    <LoaderCircle className="size-5 animate-spin" />
                  ) : (
                    <X className="size-5" />
                  )}
                  Hủy chuyến
                </Button>
              </section>
            )}

            {status === "Failed" && (
              <section className="bg-white px-5 py-5">
                <Button
                  type="button"
                  onClick={() => router.replace("/search")}
                  className="h-12 w-full rounded-xl bg-[#126b4f] hover:bg-[#0e5942]"
                >
                  <ArrowLeft className="size-5" />
                  Quay lại màn hình tìm kiếm
                </Button>
              </section>
            )}
          </div>
        )}
      </main>
    </AuthenticatedScreen>
  );
}
