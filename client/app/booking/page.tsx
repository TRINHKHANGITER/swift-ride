import { Suspense } from "react";
import { BookingSummary } from "@/components/rider/BookingSummary";

export default function BookingPage() {
  return (
    <Suspense
      fallback={
        <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />
      }
    >
      <BookingSummary />
    </Suspense>
  );
}
