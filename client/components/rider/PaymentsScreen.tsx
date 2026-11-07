"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { AlertCircle, CheckCircle2, Clock3, CreditCard, LoaderCircle, ReceiptText, RotateCcw } from "lucide-react";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import { BottomNavigation } from "@/components/rider/BottomNavigation";
import { paymentService } from "@/service/PaymentService";
import type { PaymentContract, PaymentStatus } from "@/service/contract/PaymentContract";

const formatPrice = (amount: number) => new Intl.NumberFormat("vi-VN", {
  style: "currency",
  currency: "VND",
  maximumFractionDigits: 0,
}).format(amount);

const paymentStatusName = (status: PaymentStatus) => {
  if (typeof status === "string") return status;
  return ["Pending", "Settled", "Failed", "Refunded"][status] ?? "Unknown";
};

const paymentStatusView = (status: PaymentStatus) => {
  switch (paymentStatusName(status)) {
    case "Settled":
      return { label: "Đã thanh toán", className: "text-[#126b4f]", Icon: CheckCircle2 };
    case "Pending":
      return { label: "Chờ thanh toán", className: "text-[#8a6a22]", Icon: Clock3 };
    case "Refunded":
      return { label: "Đã hoàn tiền", className: "text-[#50605a]", Icon: RotateCcw };
    default:
      return { label: "Thanh toán thất bại", className: "text-[#a34b35]", Icon: AlertCircle };
  }
};

export function PaymentsScreen() {
  const [payments, setPayments] = useState<PaymentContract[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    paymentService.getMyPayments()
      .then((result) => {
        if (active) setPayments(result);
      })
      .catch(() => {
        if (active) setError("Không thể tải lịch sử thanh toán.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, []);

  return (
    <AuthenticatedScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-white pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="border-b border-[#edf0ee] px-5 pb-5 pt-[max(24px,env(safe-area-inset-top))]">
          <h1 className="text-2xl font-bold tracking-[-0.02em] text-[#17211e]">Lịch sử thanh toán</h1>
        </header>

        <section className="px-5 py-6" aria-label="Các giao dịch">
          <div className="mb-5 flex items-center gap-3 rounded-2xl bg-[#f3f8f6] p-4">
            <span className="grid size-11 place-items-center rounded-xl bg-[#e2f2eb] text-[#126b4f]"><CreditCard className="size-6" /></span>
            <div>
              <p className="text-sm text-[#6e7b76]">Phương thức thanh toán</p>
              <p className="mt-0.5 font-bold text-[#17211e]">Thanh toán khi kết thúc chuyến</p>
            </div>
          </div>

          {loading && (
            <div className="grid min-h-60 place-items-center">
              <LoaderCircle className="size-8 animate-spin text-[#126b4f]" aria-label="Đang tải lịch sử thanh toán" />
            </div>
          )}

          {error && <p role="alert" className="rounded-xl bg-[#fff0ec] p-4 text-sm text-[#9a422d]">{error}</p>}

          {!loading && !error && payments.length === 0 && (
            <div className="py-12 text-center">
              <ReceiptText className="mx-auto size-12 text-[#9ba6a2]" />
              <h2 className="mt-4 text-lg font-bold text-[#17211e]">Chưa có hóa đơn</h2>
              <p className="mt-2 text-sm text-[#71807a]">Hóa đơn chuyến đi sẽ xuất hiện tại đây sau khi kết thúc chuyến.</p>
            </div>
          )}

          {!loading && !error && (
            <div className="space-y-3">
              {payments.map((payment) => {
                const { label, className, Icon } = paymentStatusView(payment.status);
                return (
                  <article key={payment.uuid} className="rounded-2xl border border-[#e4e9e7] p-4">
                    <div className="flex items-start gap-3">
                      <span className="grid size-10 shrink-0 place-items-center rounded-full bg-[#f0f4f2] text-[#50605a]"><ReceiptText className="size-5" /></span>
                      <div className="min-w-0 flex-1">
                        <div className="flex items-start justify-between gap-3">
                          <div>
                            <h2 className="font-bold text-[#17211e]">Thanh toán chuyến đi</h2>
                            <p className="mt-1 text-xs text-[#7a8782]">Mã chuyển khoản: {payment.transferCode}</p>
                          </div>
                          <span className="shrink-0 font-bold text-[#17211e]">{formatPrice(payment.amount)}</span>
                        </div>
                        <div className="mt-3 flex items-center justify-between gap-3">
                          <p className={`flex items-center gap-1.5 text-xs font-medium ${className}`}><Icon className="size-4" />{label}</p>
                          <Link href={`/trip/${payment.tripUuid}`} className="text-xs font-semibold text-[#126b4f] underline underline-offset-4">Xem chuyến đi</Link>
                        </div>
                      </div>
                    </div>
                  </article>
                );
              })}
            </div>
          )}
        </section>
        <BottomNavigation />
      </main>
    </AuthenticatedScreen>
  );
}
