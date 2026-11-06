"use client";

import { LocateFixed, MapPinOff, NavigationOff, TimerOff } from "lucide-react";
import type { LocationStatus } from "@/components/location/LocationProvider";

type LocationPermissionSheetProps = {
  status: Exclude<LocationStatus, "checking" | "granted">;
  onRetry: () => void;
};

const statusContent = {
  prompt: {
    message: "Người dùng chưa cấp quyền vị trí.",
    detail: "Hãy chọn cho phép trong yêu cầu của trình duyệt để tiếp tục.",
    icon: LocateFixed,
  },
  denied: {
    message: "Người dùng đã từ chối quyền vị trí.",
    detail: "Swift Ride cần quyền định vị để xác định điểm đón của bạn.",
    icon: MapPinOff,
  },
  unsupported: {
    message: "Thiết bị không sử dụng được định vị.",
    detail: "Trình duyệt hoặc thiết bị hiện tại không hỗ trợ tính năng này.",
    icon: NavigationOff,
  },
  unavailable: {
    message: "Không thể xác định vị trí của thiết bị.",
    detail: "Vui lòng kiểm tra GPS hoặc kết nối mạng rồi thử lại.",
    icon: NavigationOff,
  },
  timeout: {
    message: "Quá thời gian lấy vị trí.",
    detail: "Thiết bị phản hồi quá chậm. Vui lòng thử lại.",
    icon: TimerOff,
  },
} satisfies Record<Exclude<LocationStatus, "checking" | "granted">, {
  message: string;
  detail: string;
  icon: typeof LocateFixed;
}>;

export function LocationPermissionSheet({ status, onRetry }: LocationPermissionSheetProps) {
  const content = statusContent[status];
  const Icon = content.icon;

  return (
    <div className="fixed inset-0 z-60 flex items-end justify-center bg-black/55">
      <section
        className="animate-sheet-in w-full max-w-140 rounded-t-[28px] bg-white px-6 pb-[max(28px,env(safe-area-inset-bottom))] pt-5"
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="location-sheet-title"
        aria-describedby="location-sheet-description"
      >
        <div className="mx-auto mb-6 h-1 w-10 rounded-full bg-[#d8ddda]" />
        <span className="grid size-13 place-items-center rounded-2xl bg-[#e8f5ef] text-[#126b4f]">
          <Icon className="size-7" />
        </span>
        <h2 id="location-sheet-title" className="mt-5 text-[25px] font-bold tracking-[-0.02em] text-[#17211e]">
          Vui lòng bật định vị để sử dụng ứng dụng
        </h2>
        <div id="location-sheet-description" className="mt-4">
          <p className="font-medium text-[#34413c]">{content.message}</p>
          <p className="mt-1.5 text-sm leading-relaxed text-[#73807b]">{content.detail}</p>
        </div>
        {status !== "unsupported" && (
          <button
            type="button"
            onClick={onRetry}
            className="mt-7 h-13 w-full rounded-xl bg-[#126b4f] font-bold text-white hover:bg-[#0e5942]"
          >
            Thử lại
          </button>
        )}
      </section>
    </div>
  );
}
