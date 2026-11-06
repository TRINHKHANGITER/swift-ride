"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";
import { ArrowLeft, ChevronRight, Eye, EyeOff, LockKeyhole, Phone } from "lucide-react";
import { SwiftRideLogo } from "@/components/brand/SwiftRideLogo";
import { useAuth } from "@/components/auth/AuthProvider";
import { authService } from "@/service/AuthService";

export function LoginForm() {
  const router = useRouter();
  const { completeLogin } = useAuth();
  const [showPassword, setShowPassword] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const phone = String(formData.get("phone") ?? "");
    const password = String(formData.get("password") ?? "");

    // TODO: Kết nối server khi hoàn thiện luồng xác thực.
    // await authService.login({
    //   phone,
    //   password: String(formData.get("password") ?? ""),
    // });
    const result = await authService.login({ phone, password });
    const account = await completeLogin(result);

    if (!account) {
      alert("Không thể xác minh thông tin tài khoản.");
      return;
    }

    alert("Đăng nhập thành công!");

    router.replace(account.profile ? "/driver" : "/search");
  }

  return (
    <main className="mx-auto min-h-dvh w-full max-w-140 bg-white px-6 pb-10 pt-[max(20px,env(safe-area-inset-top))] shadow-[0_0_35px_rgba(15,45,35,0.08)]">
      <div className="flex h-12 items-center justify-between">
        <button type="button" onClick={() => router.back()} className="grid size-10 place-items-center rounded-full border border-[#e2e7e5] text-[#24332e]" aria-label="Quay lại">
          <ArrowLeft className="size-5" />
        </button>
        <SwiftRideLogo compact />
      </div>

      <section className="mt-12">
        <h1 className="text-[30px] font-bold tracking-[-0.03em] text-[#17211e]">Đăng nhập</h1>
        <form className="mt-10 space-y-5" onSubmit={handleSubmit}>
          <label className="block">
            <span className="mb-2 block text-sm font-medium text-[#39453f]">Số điện thoại</span>
            <span className="flex h-14 items-center gap-3 rounded-xl border border-[#dce3e0] px-4 focus-within:border-[#126b4f] focus-within:ring-2 focus-within:ring-[#126b4f]/10">
              <Phone className="size-5 text-[#6c7974]" />
              <input name="phone" type="tel" inputMode="tel" autoComplete="tel" pattern="[0-9]{9,11}" placeholder="Nhập số điện thoại" className="h-full min-w-0 flex-1 bg-transparent text-base outline-none placeholder:text-[#a1aaa6]" />
            </span>
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-medium text-[#39453f]">Mật khẩu</span>
            <span className="flex h-14 items-center gap-3 rounded-xl border border-[#dce3e0] px-4 focus-within:border-[#126b4f] focus-within:ring-2 focus-within:ring-[#126b4f]/10">
              <LockKeyhole className="size-5 text-[#6c7974]" />
              <input name="password" type={showPassword ? "text" : "password"} autoComplete="current-password" minLength={6} placeholder="Nhập mật khẩu" className="h-full min-w-0 flex-1 bg-transparent text-base outline-none placeholder:text-[#a1aaa6]" />
              <button type="button" onClick={() => setShowPassword((current) => !current)} className="text-[#68756f]" aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}>
                {showPassword ? <EyeOff className="size-5" /> : <Eye className="size-5" />}
              </button>
            </span>
          </label>

          <div className="text-right">
            <button type="button" className="text-sm font-medium text-[#126b4f]">Quên mật khẩu?</button>
          </div>

          <button
            type="submit"
            className="flex h-14 w-full items-center justify-center gap-2 rounded-xl bg-[#126b4f] px-5 text-base font-bold text-white transition-colors hover:bg-[#0e5942] focus-visible:outline-none focus-visible:ring-3 focus-visible:ring-[#126b4f]/20"
          >
            Đăng nhập
            <ChevronRight className="size-5" />
          </button>
        </form>

        <p className="mt-7 text-center text-sm text-[#66736e]">
          Chưa có tài khoản rider?{" "}
          <Link href="/register" className="font-bold text-[#126b4f]">Đăng ký</Link>
        </p>
      </section>
    </main>
  );
}
