"use client";

import { useEffect, useMemo, useState } from "react";
import { CheckCircle2, Clock3, LoaderCircle, MapPin, Navigation, ReceiptText, WalletCards } from "lucide-react";
import { DriverBottomNavigation } from "@/components/driver/DriverBottomNavigation";
import { DriverOnlyScreen } from "@/components/driver/DriverOnlyScreen";
import { paymentService } from "@/service/PaymentService";
import { tripService } from "@/service/TripService";
import type { PaymentContract } from "@/service/contract/PaymentContract";
import type { TripHistoryContract } from "@/service/contract/TripContract";

const formatPrice = (amount: number | null) => new Intl.NumberFormat("vi-VN", {
  style: "currency",
  currency: "VND",
  maximumFractionDigits: 0,
}).format(amount ?? 0);

const isSettled = (payment: PaymentContract) => payment.status === "Settled" || payment.status === 1;

export function DriverHistoryScreen() {
  const [trips, setTrips] = useState<TripHistoryContract[]>([]);
  const [payments, setPayments] = useState<PaymentContract[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    Promise.all([
      tripService.getDriverTripHistory(),
      paymentService.getMyDriverPayments(),
    ])
      .then(([tripHistory, paymentHistory]) => {
        if (!active) return;
        setTrips(tripHistory);
        setPayments(paymentHistory);
      })
      .catch(() => {
        if (active) setError("Không thể tải lịch sử và thu nhập tài xế.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, []);

  const settledPayments = useMemo(() => payments.filter(isSettled), [payments]);
  const totalIncome = useMemo(
    () => settledPayments.reduce((total, payment) => total + payment.amount, 0),
    [settledPayments],
  );
  const paymentsByTrip = useMemo(
    () => new Map(payments.map((payment) => [payment.tripUuid, payment])),
    [payments],
  );

  return (
    <DriverOnlyScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="border-b border-[#edf0ee] bg-white px-5 pb-5 pt-[max(22px,env(safe-area-inset-top))]">
          <h1 className="text-2xl font-bold text-[#17211e]">Lịch sử tài xế</h1>
          <p className="mt-1 text-sm text-[#71807a]">Thu nhập và các chuyến đã hoàn thành</p>
        </header>

        {loading && <div className="grid min-h-80 place-items-center"><LoaderCircle className="size-8 animate-spin text-[#126b4f]" aria-label="Đang tải lịch sử tài xế" /></div>}
        {error && <p role="alert" className="m-5 rounded-xl bg-[#fff0ec] p-4 text-sm text-[#9a422d]">{error}</p>}

        {!loading && !error && (
          <>
            <section className="mt-3 bg-white px-5 py-5">
              <div className="rounded-2xl bg-[#126b4f] p-5 text-white shadow-sm">
                <div className="flex items-center gap-2 text-sm text-white/80"><WalletCards className="size-5" />Tổng thu nhập đã thanh toán</div>
                <p className="mt-3 text-3xl font-bold">{formatPrice(totalIncome)}</p>
                <p className="mt-2 text-xs text-white/75">Từ {settledPayments.length} chuyến đã nhận thanh toán</p>
              </div>
            </section>

            {trips.length === 0 ? (
              <section className="mt-3 bg-white px-5 py-14 text-center">
                <ReceiptText className="mx-auto size-12 text-[#9ba6a2]" />
                <h2 className="mt-4 text-lg font-bold text-[#17211e]">Chưa có lịch sử chuyến đi</h2>
                <p className="mt-2 text-sm text-[#71807a]">Các chuyến đã trả khách sẽ xuất hiện tại đây.</p>
              </section>
            ) : (
              <section className="space-y-3 px-5 py-5" aria-label="Lịch sử chuyến đi của tài xế">
                {trips.map((trip) => {
                  const payment = paymentsByTrip.get(trip.uuid);
                  const paid = payment ? isSettled(payment) : trip.status === "Paid";
                  return (
                    <article key={trip.uuid} className="rounded-2xl border border-[#e3e8e6] bg-white p-4">
                      <div className="flex items-start justify-between gap-3 border-b border-[#edf0ee] pb-3">
                        <div>
                          <p className="flex items-center gap-1.5 text-xs text-[#71807a]"><Clock3 className="size-4" />{new Intl.DateTimeFormat("vi-VN", { dateStyle: "short", timeStyle: "short" }).format(new Date(trip.updatedAt))}</p>
                          <p className={`mt-2 flex items-center gap-1.5 text-xs font-semibold ${paid ? "text-[#126b4f]" : "text-[#8a6a22]"}`}>
                            {paid ? <CheckCircle2 className="size-4" /> : <Clock3 className="size-4" />}
                            {paid ? "Đã nhận thanh toán" : "Đang chờ thanh toán"}
                          </p>
                        </div>
                        <p className="font-bold text-[#17211e]">{formatPrice(payment?.amount ?? trip.price)}</p>
                      </div>
                      <div className="mt-4 grid grid-cols-[20px_1fr] gap-x-3 gap-y-4">
                        <Navigation className="size-5 text-[#126b4f]" /><div><p className="text-xs text-[#85908c]">Điểm đón</p><p className="font-medium text-[#25322e]">{trip.pickupName}</p></div>
                        <MapPin className="size-5 text-[#b05d43]" /><div><p className="text-xs text-[#85908c]">Điểm đến</p><p className="font-medium text-[#25322e]">{trip.destinationName}</p></div>
                      </div>
                      <div className="mt-4 flex items-center justify-between gap-3 text-xs">
                        <span className="text-[#71807a]">{trip.distanceKM ?? 0} km · {Math.ceil(trip.durationMin ?? 0)} phút</span>
                        <span className="font-medium text-[#71807a]">#{trip.uuid.slice(0, 8)}</span>
                      </div>
                    </article>
                  );
                })}
              </section>
            )}
          </>
        )}

        <DriverBottomNavigation />
      </main>
    </DriverOnlyScreen>
  );
}
