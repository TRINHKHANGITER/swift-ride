import { Suspense } from "react";
import { SearchScreen } from "@/components/rider/SearchScreen";

export default function SearchPage() {
  return (
    <Suspense
      fallback={
        <main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />
      }
    >
      <SearchScreen />
    </Suspense>
  );
}
