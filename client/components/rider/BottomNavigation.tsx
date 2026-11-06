"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Clock3, Home, UserRound, WalletCards } from "lucide-react";

const navItems = [
  { name: "Trang chủ", href: "/", icon: Home },
  { name: "Thanh toán", href: "/payments", icon: WalletCards },
  { name: "Hoạt động", href: "/activity", icon: Clock3 },
  { name: "Tài khoản", href: "/account", icon: UserRound },
];

export function BottomNavigation() {
  const pathname = usePathname();

  return (
    <nav className="fixed inset-x-0 bottom-0 z-30 mx-auto flex h-[76px] w-full max-w-140 items-start justify-around border-t border-[#e8ecea] bg-white px-2 pt-3 pb-[max(8px,env(safe-area-inset-bottom))]" aria-label="Điều hướng chính">
      {navItems.map(({ name, href, icon: Icon }) => {
        const active = pathname === href;
        return (
          <Link key={href} href={href} className={`flex min-w-16 flex-col items-center gap-1 text-[11px] ${active ? "font-bold text-[#126b4f]" : "text-[#74807b]"}`}>
            <Icon className="size-5" strokeWidth={active ? 2.5 : 2} />
            <span>{name}</span>
          </Link>
        );
      })}
    </nav>
  );
}
