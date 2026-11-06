"use client";

import { LogOut, Phone, ShieldCheck, UserRound } from "lucide-react";
import { useAuth } from "@/components/auth/AuthProvider";
import { AuthenticatedScreen } from "@/components/rider/AuthenticatedScreen";
import { BottomNavigation } from "@/components/rider/BottomNavigation";

export function AccountScreen() {
  const { user, logout } = useAuth();
  const account = user?.account;

  return (
    <AuthenticatedScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="bg-white px-5 pb-6 pt-[max(24px,env(safe-area-inset-top))]">
          <h1 className="text-2xl font-bold tracking-[-0.02em] text-[#17211e]">Tài khoản</h1>
          <div className="mt-6 flex items-center gap-4">
            <span className="grid size-16 place-items-center rounded-full bg-[#e2f2eb] text-[#126b4f]"><UserRound className="size-8" /></span>
            <div className="min-w-0">
              <h2 className="truncate text-lg font-bold text-[#17211e]">{account?.fullname}</h2>
              <p className="mt-1 text-sm text-[#71807a]">{account?.role === "DRIVER" ? "Tài xế" : "Rider"}</p>
            </div>
          </div>
        </header>

        <section className="mt-3 bg-white px-5 py-2" aria-label="Thông tin tài khoản">
          <div className="flex items-center gap-3 border-b border-[#edf0ee] py-4">
            <span className="grid size-10 place-items-center rounded-xl bg-[#f0f4f2] text-[#55645e]"><Phone className="size-5" /></span>
            <div><p className="text-xs text-[#7b8782]">Số điện thoại</p><p className="mt-0.5 font-medium text-[#25322e]">{account?.phone || "Chưa cập nhật"}</p></div>
          </div>
          <div className="flex items-center gap-3 py-4">
            <span className="grid size-10 place-items-center rounded-xl bg-[#e9f5f0] text-[#126b4f]"><ShieldCheck className="size-5" /></span>
            <div><p className="text-xs text-[#7b8782]">Trạng thái</p><p className="mt-0.5 font-medium text-[#25322e]">Tài khoản đã đăng nhập</p></div>
          </div>
        </section>

        <section className="mt-3 bg-white p-5">
          <button
            type="button"
            onClick={logout}
            className="flex h-14 w-full items-center justify-center gap-2 rounded-xl border border-[#e4bcb2] bg-white text-base font-bold text-[#a33f2c] transition-colors hover:bg-[#fff7f5] focus-visible:outline-none focus-visible:ring-3 focus-visible:ring-[#a33f2c]/15"
          >
            <LogOut className="size-5" />
            Đăng xuất
          </button>
        </section>
        <BottomNavigation />
      </main>
    </AuthenticatedScreen>
  );
}
