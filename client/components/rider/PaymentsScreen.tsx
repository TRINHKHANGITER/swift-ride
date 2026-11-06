"use client";

import { CheckCircle2, CreditCard, ReceiptText } from "lucide-react";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import { BottomNavigation } from "@/components/rider/BottomNavigation";

const payments = [
  { id: "SR-240615", date: "15/06/2026 · 08:42", amount: "85.000đ" },
  { id: "SR-190512", date: "12/05/2026 · 18:20", amount: "62.000đ" },
  { id: "SR-110409", date: "09/04/2026 · 07:15", amount: "48.000đ" },
];

export function PaymentsScreen() {
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

          <div className="space-y-3">
            {payments.map((payment) => (
              <article key={payment.id} className="rounded-2xl border border-[#e4e9e7] p-4">
                <div className="flex items-start gap-3">
                  <span className="grid size-10 shrink-0 place-items-center rounded-full bg-[#f0f4f2] text-[#50605a]"><ReceiptText className="size-5" /></span>
                  <div className="min-w-0 flex-1">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <h2 className="font-bold text-[#17211e]">Thanh toán chuyến đi</h2>
                        <p className="mt-1 text-xs text-[#7a8782]">{payment.id} · {payment.date}</p>
                      </div>
                      <span className="shrink-0 font-bold text-[#17211e]">{payment.amount}</span>
                    </div>
                    <p className="mt-3 flex items-center gap-1.5 text-xs font-medium text-[#126b4f]"><CheckCircle2 className="size-4" /> Đã thanh toán</p>
                  </div>
                </div>
              </article>
            ))}
          </div>
        </section>
        <BottomNavigation />
      </main>
    </AuthenticatedScreen>
  );
}
