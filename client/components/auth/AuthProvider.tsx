"use client";

import { createContext, ReactNode, useCallback, useContext, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import type { AboutMeView, LoginView } from "@/service/contract/AuthContract";
import { authService } from "@/service/AuthService";

type AuthStatus = "loading" | "authenticated" | "unauthenticated";

type AuthContextValue = {
  status: AuthStatus;
  user: AboutMeView | null;
  completeLogin: (result: LoginView) => Promise<AboutMeView | null>;
  refreshAuth: () => Promise<AboutMeView | null>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const router = useRouter();
  const [status, setStatus] = useState<AuthStatus>("loading");
  const [user, setUser] = useState<AboutMeView | null>(null);

  const refreshAuth = useCallback(async () => {
    const token = window.localStorage.getItem("swift-ride-token");

    if (!token) {
      setUser(null);
      setStatus("unauthenticated");
      return null;
    }

    setStatus("loading");
    try {
      const aboutMe = await authService.getAboutMe();
      setUser(aboutMe);
      setStatus("authenticated");
      return aboutMe;
    } catch {
      window.localStorage.removeItem("swift-ride-token");
      window.localStorage.removeItem("swift-ride-account-uuid");
      window.localStorage.removeItem("swift-ride-account-role");
      window.localStorage.removeItem("swift-ride-account-iat");
      window.localStorage.removeItem("swift-ride-account-exp");
      setUser(null);
      setStatus("unauthenticated");
      return null;
    }
  }, []);

  useEffect(() => {
    let active = true;
    const token = window.localStorage.getItem("swift-ride-token");

    if (!token) {
      const timeout = window.setTimeout(() => {
        if (active) setStatus("unauthenticated");
      }, 0);

      return () => {
        active = false;
        window.clearTimeout(timeout);
      };
    }

    authService.getAboutMe()
      .then((aboutMe) => {
        if (!active) return;
        setUser(aboutMe);
        setStatus("authenticated");
      })
      .catch(() => {
        if (!active) return;
        window.localStorage.removeItem("swift-ride-token");
        window.localStorage.removeItem("swift-ride-account-uuid");
        window.localStorage.removeItem("swift-ride-account-role");
        window.localStorage.removeItem("swift-ride-account-iat");
        window.localStorage.removeItem("swift-ride-account-exp");
        setUser(null);
        setStatus("unauthenticated");
      });

    return () => {
      active = false;
    };
  }, []);

  const completeLogin = useCallback(async (result: LoginView) => {
    window.localStorage.setItem("swift-ride-token", result.token);
    window.localStorage.setItem("swift-ride-account-uuid", result.accountUuid);
    window.localStorage.setItem("swift-ride-account-role", result.role);
    window.localStorage.setItem("swift-ride-account-iat", result.iat.toString());
    window.localStorage.setItem("swift-ride-account-exp", result.exp.toString());
    return refreshAuth();
  }, [refreshAuth]);

  const logout = useCallback(() => {
    window.localStorage.removeItem("swift-ride-token");
    window.localStorage.removeItem("swift-ride-account-uuid");
    window.localStorage.removeItem("swift-ride-account-role");
    window.localStorage.removeItem("swift-ride-account-iat");
    window.localStorage.removeItem("swift-ride-account-exp");
    setUser(null);
    setStatus("unauthenticated");
    router.replace("/");
  }, [router]);

  return (
    <AuthContext.Provider value={{ status, user, completeLogin, refreshAuth, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth phải được sử dụng bên trong AuthProvider");
  return context;
}
