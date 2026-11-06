"use client";

import { BadgeCheck, CarFront, CreditCard, LogOut, Phone, UserRound } from "lucide-react";
import { useAuth } from "@/components/auth/AuthProvider";
import { DriverBottomNavigation } from "@/components/driver/DriverBottomNavigation";
import { DriverOnlyScreen } from "@/components/driver/DriverOnlyScreen";

export function DriverAccountScreen() {
  const { user, logout } = useAuth();
  const account = user?.account;
  const profile = user?.profile;

  return (
    <DriverOnlyScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="bg-white px-5 pb-6 pt-[max(24px,env(safe-area-inset-top))]">
          <h1 className="text-2xl font-bold text-[#17211e]">Tài khoản</h1>
          <div className="mt-6 flex items-center gap-4">
            <span className="grid size-16 place-items-center rounded-full bg-[#e2f2eb] text-[#126b4f]">
              <UserRound className="size-8" />
            </span>
            <div className="min-w-0">
              <h2 className="truncate text-lg font-bold text-[#17211e]">{account?.fullname}</h2>
              <p className="mt-1 text-sm text-[#71807a]">Tài xế Swift Ride</p>
            </div>
          </div>
        </header>

        <section className="mt-3 bg-white px-5" aria-label="Thông tin tài xế">
          <InfoRow icon={Phone} label="Số điện thoại" value={account?.phone || "Chưa cập nhật"} />
          <InfoRow icon={CarFront} label="Phương tiện" value={profile?.vehicleName || "Chưa cập nhật"} />
          <InfoRow icon={CreditCard} label="Biển số xe" value={profile?.licensePlate || "Chưa cập nhật"} />
          <InfoRow icon={BadgeCheck} label="Giấy phép lái xe" value={profile?.licenseNumber || "Chưa cập nhật"} last />
        </section>

        <section className="mt-3 bg-white px-5 py-4">
          <div className="flex items-center justify-between gap-3">
            <span className="text-sm text-[#71807a]">Trạng thái hồ sơ</span>
            <span className={`text-sm font-bold ${profile?.isVerified ? "text-[#126b4f]" : "text-[#9a5a32]"}`}>
              {profile?.isVerified ? "Đã xác minh" : "Chờ xác minh"}
            </span>
          </div>
        </section>

        <section className="mt-3 bg-white p-5">
          <button
            type="button"
            onClick={logout}
            className="flex h-14 w-full items-center justify-center gap-2 rounded-xl border border-[#e4bcb2] text-base font-bold text-[#a33f2c] hover:bg-[#fff7f5]"
          >
            <LogOut className="size-5" />
            Đăng xuất
          </button>
        </section>

        <DriverBottomNavigation />
      </main>
    </DriverOnlyScreen>
  );
}

function InfoRow({
  icon: Icon,
  label,
  value,
  last = false,
}: {
  icon: typeof Phone;
  label: string;
  value: string;
  last?: boolean;
}) {
  return (
    <div className={`flex items-center gap-3 py-4 ${last ? "" : "border-b border-[#edf0ee]"}`}>
      <Icon className="size-5 shrink-0 text-[#5d6a65]" />
      <div className="min-w-0">
        <p className="text-xs text-[#7b8782]">{label}</p>
        <p className="mt-0.5 truncate font-medium text-[#25322e]">{value}</p>
      </div>
    </div>
  );
}
