"use client";

import { ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";

export function GuestOnlyScreen({ children }: { children: ReactNode }) {
  const router = useRouter();
  const { status, user } = useAuth();

  useEffect(() => {
    if (status === "authenticated") {
      router.replace(user?.profile ? "/driver" : "/");
    }
  }, [router, status, user]);

  if (status !== "unauthenticated") {
    return <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />;
  }

  return children;
}
