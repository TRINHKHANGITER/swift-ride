"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import {
  ArrowRight,
  Clock3,
  Navigation,
  Search,
  ShieldCheck,
  UserRound,
} from "lucide-react";
import { SwiftRideLogo } from "@/components/brand/SwiftRideLogo";
import { AuthSheet } from "@/components/rider/AuthSheet";
import { BottomNavigation } from "@/components/rider/BottomNavigation";
import { useAuth } from "@/components/auth/AuthProvider";

const recentPlaces = ["488/9 Cộng Hòa", "Sân bay Tân Sơn Nhất", "Ga Sài Gòn"];

export function RiderHome() {
  const router = useRouter();

  const [authOpen, setAuthOpen] = useState(false);
  const { status, user } = useAuth();
  const isSignedIn = status === "authenticated";

  useEffect(() => {
    if (status === "authenticated" && user?.profile) {
      router.replace("/driver");
    }
  }, [router, status, user]);

  if (status === "authenticated" && user?.profile) {
    return <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />;
  }

  function openBooking() {
    if (isSignedIn) {
      router.push("/search");
      return;
    }
    setAuthOpen(true);
  }

  function openAccount() {
    if (isSignedIn) {
      router.push("/account");
      return;
    }
    setAuthOpen(true);
  }

  return (
    <main className="mx-auto min-h-dvh w-full max-w-140 overflow-hidden bg-[#f5f7f6] pb-24">
      <header className="relative overflow-hidden rounded-b-[32px] bg-[#0b4c3a] px-5 pb-8 pt-[max(18px,env(safe-area-inset-top))] text-white">
        <span className="pointer-events-none absolute -right-14 top-16 size-40 rounded-full border border-white/10" />
        <span className="pointer-events-none absolute -right-2 top-27 size-20 rounded-full border border-white/10" />

        <div className="relative z-10 flex items-center justify-between">
          <SwiftRideLogo inverted />
          <button
            type="button"
            onClick={openAccount}
            className="grid size-11 place-items-center rounded-full border border-white/25 bg-white/10 transition-colors hover:bg-white/15"
            aria-label="Tài khoản"
          >
            <UserRound className="size-6" />
          </button>
        </div>

        <div className="relative z-10 mt-9">
          <p className="text-sm font-medium text-white/70">Swift Ride</p>
          <h1 className="mt-1 max-w-75 text-[28px] font-bold leading-tight tracking-[-0.03em]">
            Hôm nay bạn muốn đi đâu?
          </h1>
        </div>

        <button
          type="button"
          onClick={openBooking}
          className="relative z-10 mt-6 flex h-15 w-full items-center gap-3 rounded-xl bg-white px-4 text-left text-[#66736e] transition-colors hover:bg-[#f7faf8]"
        >
          <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-[#e9f5f0] text-[#126b4f]">
            <Search className="size-5" />
          </span>
          <span className="min-w-0 flex-1 font-medium">Tìm điểm đến</span>
          <ArrowRight className="size-5 shrink-0 text-[#126b4f]" />
        </button>
      </header>

      <div className="space-y-6 px-5 py-6">
        <button
          type="button"
          onClick={openBooking}
          className="flex w-full items-center gap-4 rounded-2xl bg-[#126b4f] p-4 text-left text-white transition-colors hover:bg-[#0e5942]"
        >
          <span className="grid size-12 shrink-0 place-items-center rounded-xl bg-white/15">
            <Navigation className="size-6" />
          </span>
          <span className="min-w-0 flex-1">
            <span className="block text-base font-bold">Đặt chuyến ngay</span>
            <span className="mt-0.5 block text-sm text-white/75">
              Chọn điểm đón và điểm đến
            </span>
          </span>
          <ArrowRight className="size-5 shrink-0" />
        </button>

        <section aria-labelledby="recent-title">
          <h2 id="recent-title" className="text-lg font-bold text-[#17211e]">
            Địa điểm gần đây
          </h2>
          <div className="mt-3 overflow-hidden rounded-2xl border border-[#e3e8e6] bg-white px-4">
            {recentPlaces.map((place) => (
              <button
                key={place}
                type="button"
                onClick={openBooking}
                className="flex w-full items-center gap-3 border-b border-[#edf0ee] py-4 text-left transition-colors last:border-0 hover:bg-[#fafcfb]"
              >
                <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-[#f0f4f2] text-[#5e6c67]">
                  <Clock3 className="size-5" />
                </span>
                <span className="min-w-0 flex-1 truncate font-medium text-[#26332f]">
                  {place}
                </span>
                <ArrowRight className="size-4 shrink-0 text-[#87928e]" />
              </button>
            ))}
          </div>
        </section>

        <section className="flex items-center gap-4 rounded-2xl border border-[#dce8e3] bg-white p-4">
          <span className="grid size-11 shrink-0 place-items-center rounded-xl bg-[#e2f2eb] text-[#126b4f]">
            <ShieldCheck className="size-6" />
          </span>
          <div>
            <h2 className="font-bold text-[#17211e]">An toàn trên mọi hành trình</h2>
            <p className="mt-1 text-sm text-[#6f7d77]">
              Theo dõi chuyến đi từ lúc đón đến khi kết thúc
            </p>
          </div>
        </section>
      </div>

      <BottomNavigation />
      <AuthSheet open={authOpen} onClose={() => setAuthOpen(false)} />
    </main>
  );
}
