"use client";

import { ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";

export function DriverOnlyScreen({ children }: { children: ReactNode }) {
  const router = useRouter();
  const { status, user } = useAuth();

  useEffect(() => {
    if (status === "unauthenticated") {
      router.replace("/login");
    } else if (status === "authenticated" && !user?.profile) {
      router.replace("/");
    }
  }, [router, status, user]);

  if (status !== "authenticated" || !user?.profile) {
    return <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />;
  }

  return children;
}
