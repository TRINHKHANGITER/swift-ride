import type { Metadata } from "next";
import { Roboto } from "next/font/google";
import { AuthProvider } from "@/components/auth/AuthProvider";
import "./globals.css";

const roboto = Roboto({
  variable: "--font-roboto",
  subsets: ["latin", "vietnamese"],
  weight: ["400", "500", "700", "900"],
});

export const metadata: Metadata = {
  title: "Swift Ride",
  description: "Đặt chuyến đi nhanh chóng và an toàn cùng Swift Ride.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="vi" className={`${roboto.variable} h-full antialiased`}>
      <body className="min-h-full bg-[#f3f5f4]">
        <AuthProvider>{children}</AuthProvider>
      </body>
    </html>
  );
}
