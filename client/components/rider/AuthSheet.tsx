"use client";

import Link from "next/link";
import { X } from "lucide-react";

type AuthSheetProps = {
  open: boolean;
  onClose: () => void;
};

export function AuthSheet({ open, onClose }: AuthSheetProps) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center bg-black/55" role="presentation" onMouseDown={onClose}>
      <section
        className="animate-sheet-in w-full max-w-140 rounded-t-[28px] bg-white px-6 pb-[max(28px,env(safe-area-inset-bottom))] pt-5"
        role="dialog"
        aria-modal="true"
        aria-labelledby="auth-sheet-title"
        onMouseDown={(event) => event.stopPropagation()}
      >
        <div className="mx-auto mb-5 h-1 w-10 rounded-full bg-[#d8ddda]" />
        <button
          type="button"
          onClick={onClose}
          className="absolute right-5 top-5 grid size-9 place-items-center rounded-full text-[#61706a] hover:bg-[#f0f3f2]"
          aria-label="Đóng"
        >
          <X className="size-5" />
        </button>
        <h2 id="auth-sheet-title" className="text-[26px] font-bold tracking-[-0.02em] text-[#17211e]">
          Bắt đầu cùng Swift Ride
        </h2>
        <div className="mt-7 grid grid-cols-2 gap-3">
          <Link
            href="/register"
            className="flex h-13 items-center justify-center rounded-xl border border-[#b9d9ce] bg-[#eef8f4] font-bold text-[#0b5943]"
          >
            Đăng ký
          </Link>
          <Link
            href="/login"
            className="flex h-13 items-center justify-center rounded-xl bg-[#126b4f] font-bold text-white hover:bg-[#0e5942]"
          >
            Đăng nhập
          </Link>
        </div>
      </section>
    </div>
  );
}
