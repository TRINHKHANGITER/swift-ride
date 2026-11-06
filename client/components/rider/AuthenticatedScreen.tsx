"use client";

import { ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";
import { AuthSheet } from "@/components/rider/AuthSheet";
import { useAuth } from "@/components/auth/AuthProvider";

export function AuthenticatedScreen({ children }: { children: ReactNode }) {
  const router = useRouter();
  const { status, user } = useAuth();

  useEffect(() => {
    if (status === "authenticated" && user?.profile) {
      router.replace("/driver");
    }
  }, [router, status, user]);

  if (status === "loading") {
    return <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />;
  }

  if (status === "unauthenticated") {
    return (
      <main className="mx-auto min-h-dvh w-full max-w-140 bg-white">
        <AuthSheet open onClose={() => router.replace("/")} />
      </main>
    );
  }

  if (user?.profile) {
    return <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />;
  }

  return children;
}
