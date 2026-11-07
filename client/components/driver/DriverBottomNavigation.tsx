"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { BriefcaseBusiness, History, Radar, UserRound } from "lucide-react";

const navItems = [
  { name: "Dò đơn", href: "/driver", icon: Radar },
  { name: "Đang làm", href: "/driver/active", icon: BriefcaseBusiness },
  { name: "Lịch sử", href: "/driver/history", icon: History },
  { name: "Tài khoản", href: "/driver/account", icon: UserRound },
];

export function DriverBottomNavigation() {
  const pathname = usePathname();

  return (
    <nav
      className="fixed inset-x-0 bottom-0 z-30 mx-auto flex h-[76px] w-full max-w-140 items-start justify-around border-t border-[#e8ecea] bg-white px-2 pb-[max(8px,env(safe-area-inset-bottom))] pt-3"
      aria-label="Điều hướng tài xế"
    >
      {navItems.map(({ name, href, icon: Icon }) => {
        const active = pathname === href;
        return (
          <Link
            key={href}
            href={href}
            className={`flex min-w-16 flex-col items-center gap-1 text-[11px] ${active ? "font-bold text-[#126b4f]" : "text-[#74807b]"}`}
          >
            <Icon className="size-5" strokeWidth={active ? 2.5 : 2} />
            <span>{name}</span>
          </Link>
        );
      })}
    </nav>
  );
}
