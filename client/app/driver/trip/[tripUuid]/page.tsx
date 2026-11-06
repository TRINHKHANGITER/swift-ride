import { Suspense } from "react";
import { DriverTripScreen } from "@/components/driver/DriverTripScreen";

async function TripContent({ params }: { params: Promise<{ tripUuid: string }> }) {
  const { tripUuid } = await params;
  return <DriverTripScreen tripUuid={tripUuid} />;
}

export default function DriverTripPage({ params }: { params: Promise<{ tripUuid: string }> }) {
  return <Suspense fallback={<main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />}><TripContent params={params} /></Suspense>;
}
