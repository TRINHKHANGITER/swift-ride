"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import {
  Clock3,
  LoaderCircle,
  LocateFixed,
  MapPin,
  Navigation,
  Radar,
  ShieldAlert,
} from "lucide-react";
import { useAuth } from "@/components/auth/AuthProvider";
import { DriverBottomNavigation } from "@/components/driver/DriverBottomNavigation";
import { DriverOnlyScreen } from "@/components/driver/DriverOnlyScreen";
import DriverLocationMap from "@/components/map/DriverLocationMap";
import { Button } from "@/components/ui/button";
import { API_BASE_URL } from "@/lib/http-client";
import { tripService } from "@/service/TripService";
import type { TripContract } from "@/service/contract/TripContract";
import { useQuery } from "@/lib/use-query";
import { driverService } from "@/service/DriverService";
import type { DriverLocation } from "@/service/contract/DriverContract";

export function DriverFindTripsScreen() {
  const router = useRouter();
  const { user } = useAuth();
  const profile = user?.profile;
  const profileUuid = profile?.profileUuid;

  const [driverLocation, setDriverLocation] = useState<DriverLocation | null>(
    null,
  );
  const [tripRequest, setTripRequest] = useState<TripContract | null>(null);
  const [connectionError, setConnectionError] = useState("");
  const [action, setAction] = useState<"accept" | "reject" | null>(null);

  const { execute: dataExecute, inProcess: isLoadingDriver } = useQuery();
  const { execute: onlineExecute, inProcess: isUpdatingOnline } = useQuery();
  const { execute: tripExecute } = useQuery();

  const isOnline = driverLocation?.isOnline ?? false;

  const handleChangeOnline = async () => {
    if (!profileUuid || !driverLocation || isUpdatingOnline) return;

    const nextIsOnline = !driverLocation.isOnline;
    setConnectionError("");

    await onlineExecute(
      async () => {
        await driverService.updateIsOnline(profileUuid, nextIsOnline);
        setDriverLocation((current) =>
          current ? { ...current, isOnline: nextIsOnline } : current,
        );
      },
      {
        onError: (error) => setConnectionError(error.message),
      },
    );
  };

  useEffect(() => {
    if (!profileUuid) return;

    void dataExecute(
      async () => {
        // * Driver location
        const location = await driverService.getDriverLocation(profileUuid);
        setDriverLocation(location);

        // * Active trip
        const activeTrip = await tripService.getActiveTrip();
        if (activeTrip) {
          router.replace("/driver/active");
        }
      },
      {
        onError: (error) => setConnectionError(error.message),
      },
    );
  }, [dataExecute, profileUuid, router]);

  useEffect(() => {
    if (!isOnline || !profile?.isVerified) return;

    const controller = new AbortController();
    let retryTimer: number | undefined;

    // * Stream recieve trip which booked by rider and sent by trip service
    const connect = async () => {
      try {
        const token = window.localStorage.getItem("swift-ride-token");
        if (!token) return;

        const response = await fetch(
          `${API_BASE_URL}/ts/drivers/notifications/stream`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
              Accept: "text/event-stream",
            },
            cache: "no-store",
            signal: controller.signal,
          },
        );
        if (!response.ok || !response.body)
          throw new Error("Không thể mở kết nối thông báo.");

        setConnectionError("");

        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let buffer = "";

        while (!controller.signal.aborted) {
          const { value, done } = await reader.read();

          if (done) break;

          buffer += decoder.decode(value, { stream: true });
          const messages = buffer.split(/\r?\n\r?\n/);
          buffer = messages.pop() ?? "";

          for (const message of messages) {
            const lines = message.split(/\r?\n/);
            const event = lines
              .find((line) => line.startsWith("event:"))
              ?.slice(6)
              .trim();
            const data = lines
              .filter((line) => line.startsWith("data:"))
              .map((line) => line.slice(5).trimStart())
              .join("\n");
            if (event === "trip-request" && data)
              setTripRequest(JSON.parse(data) as TripContract);
          }
        }
      } catch (error) {
        if (!controller.signal.aborted)
          setConnectionError(
            error instanceof Error ? error.message : "Mất kết nối thông báo.",
          );
      } finally {
        if (!controller.signal.aborted)
          retryTimer = window.setTimeout(() => void connect(), 3_000);
      }
    };

    void connect();

    // Unmount component
    return () => {
      controller.abort();
      if (retryTimer) window.clearTimeout(retryTimer);
    };
  }, [isOnline, profile?.isVerified]);

  const respondToTrip = async (nextAction: "accept" | "reject") => {
    if (!tripRequest || action) return;
    setAction(nextAction);
    setConnectionError("");

    await tripExecute(
      async () => {
        if (nextAction === "accept") {
          await driverService.acceptTrip(tripRequest.uuid);
          setTripRequest(null);
          window.localStorage.setItem(
            "swift-ride-active-trip",
            tripRequest.uuid,
          );
          router.push("/driver/active");
        } else {
          await driverService.rejectTrip(tripRequest.uuid);
          setTripRequest(null);
        }
      },
      {
        onSucess: () => {
          setAction(null);
        },
        onError: (error) => {
          setConnectionError(error.message);
          setAction(null);
        },
      },
    );
  };

  return (
    <DriverOnlyScreen>
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-[#f5f7f6] pb-24 shadow-[0_0_35px_rgba(15,45,35,0.08)]">
        <header className="border-b border-[#edf0ee] bg-white px-5 pb-5 pt-[max(22px,env(safe-area-inset-top))]">
          <p className="text-sm text-[#71807a]">
            Xin chào, {user?.account.fullname}
          </p>
          <h1 className="mt-1 text-2xl font-bold text-[#17211e]">Dò đơn</h1>
        </header>

        <section className="mt-3 bg-white px-5 py-5">
          <div className="flex items-center justify-between gap-4">
            <div>
              <h2 className="font-bold text-[#17211e]">
                {isLoadingDriver
                  ? "Đang tải trạng thái"
                  : isOnline
                    ? "Đang sẵn sàng nhận chuyến"
                    : "Đang ngoại tuyến"}
              </h2>
              <p className="mt-1 text-sm text-[#71807a]">
                {isOnline
                  ? "Hệ thống đang tìm chuyến gần bạn"
                  : "Bật trạng thái để bắt đầu dò đơn"}
              </p>
            </div>
            <button
              type="button"
              role="switch"
              aria-checked={isOnline}
              disabled={!driverLocation || isUpdatingOnline}
              onClick={() => void handleChangeOnline()}
              className={`relative h-7 w-12 shrink-0 rounded-full transition-colors disabled:cursor-not-allowed disabled:opacity-60 ${isOnline ? "bg-[#126b4f]" : "bg-[#c9d0cd]"}`}
              aria-label="Trạng thái nhận chuyến"
            >
              <span
                className={`absolute top-1 size-5 rounded-full bg-white transition-all ${isOnline ? "left-6" : "left-1"}`}
              />
            </button>
          </div>
        </section>

        <section className="mt-3 overflow-hidden bg-white" aria-label="Vị trí tài xế">
          {driverLocation ? (
            <DriverLocationMap
              latitude={driverLocation.latitude}
              longitude={driverLocation.longitude}
            />
          ) : (
            <div className="grid h-72 place-items-center bg-[#eef2f0]">
              <LoaderCircle className="size-8 animate-spin text-[#126b4f]" aria-label="Đang tải bản đồ" />
            </div>
          )}
        </section>

        {!profile?.isVerified && (
          <section className="mt-3 flex gap-3 bg-white px-5 py-4 text-sm text-[#8a4b35]">
            <ShieldAlert className="size-5 shrink-0" />
            <p>Hồ sơ tài xế chưa được xác minh. Bạn chưa thể nhận chuyến.</p>
          </section>
        )}

        {tripRequest ? (
          <section className="mt-3 bg-white px-5 py-5" aria-live="assertive">
            <div className="flex items-center justify-between gap-3">
              <div>
                <p className="text-xs font-bold uppercase tracking-wide text-[#126b4f]">
                  Chuyến mới
                </p>
                <h2 className="mt-1 text-xl font-bold text-[#17211e]">
                  Yêu cầu chuyến đi
                </h2>
              </div>
              <p className="font-bold text-[#17211e]">
                {new Intl.NumberFormat("vi-VN").format(tripRequest.price ?? 0)}đ
              </p>
            </div>
            <div className="mt-5 space-y-4 border-y border-[#edf0ee] py-5">
              <div className="flex gap-3">
                <Navigation className="mt-0.5 size-5 shrink-0 text-[#126b4f]" />
                <div>
                  <p className="text-xs text-[#71807a]">Điểm đón</p>
                  <p className="font-medium text-[#17211e]">
                    {tripRequest.pickupName}
                  </p>
                </div>
              </div>
              <div className="flex gap-3">
                <MapPin className="mt-0.5 size-5 shrink-0 text-[#b05d43]" />
                <div>
                  <p className="text-xs text-[#71807a]">Điểm đến</p>
                  <p className="font-medium text-[#17211e]">
                    {tripRequest.destinationName}
                  </p>
                </div>
              </div>
              <p className="flex items-center gap-2 text-sm text-[#71807a]">
                <Clock3 className="size-4" /> {tripRequest.distanceKM ?? 0} km ·
                khoảng {Math.ceil(tripRequest.durationMin ?? 0)} phút
              </p>
            </div>
            <div className="mt-5 grid grid-cols-2 gap-3">
              <Button
                variant="outline"
                className="h-12 rounded-xl"
                disabled={action !== null}
                onClick={() => void respondToTrip("reject")}
              >
                {action === "reject" && (
                  <LoaderCircle className="animate-spin" />
                )}
                Từ chối
              </Button>
              <Button
                className="h-12 rounded-xl bg-[#126b4f] hover:bg-[#0e5942]"
                disabled={action !== null}
                onClick={() => void respondToTrip("accept")}
              >
                {action === "accept" && (
                  <LoaderCircle className="animate-spin" />
                )}
                Chấp nhận
              </Button>
            </div>
          </section>
        ) : (
          <section
            className="mt-3 bg-white px-5 py-12 text-center"
            aria-live="polite"
          >
            <span className="mx-auto grid size-16 place-items-center rounded-full bg-[#eef3f1] text-[#51605b]">
              {isOnline ? (
                <Radar className="size-8" />
              ) : (
                <LocateFixed className="size-8" />
              )}
            </span>
            <h2 className="mt-5 text-lg font-bold text-[#17211e]">
              {isOnline ? "Đang tìm chuyến phù hợp" : "Bạn đang ngoại tuyến"}
            </h2>
            <p className="mx-auto mt-2 max-w-70 text-sm leading-relaxed text-[#71807a]">
              {isOnline
                ? "Chuyến mới trong khu vực sẽ xuất hiện tại đây."
                : "Bạn sẽ không nhận được đề xuất chuyến mới."}
            </p>
          </section>
        )}

        {connectionError && (
          <p
            role="alert"
            className="mx-5 mt-3 rounded-xl bg-[#fff0ec] px-4 py-3 text-sm text-[#9a422d]"
          >
            {connectionError}
          </p>
        )}

        <DriverBottomNavigation />
      </main>
    </DriverOnlyScreen>
  );
}
