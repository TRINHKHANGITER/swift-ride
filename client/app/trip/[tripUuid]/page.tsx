import { Suspense } from "react";
import { RiderTripScreen } from "@/components/rider/RiderTripScreen";

async function TripContent({
  params,
}: {
  params: Promise<{ tripUuid: string }>;
}) {
  const { tripUuid } = await params;
  return <RiderTripScreen tripUuid={tripUuid} />;
}

export default function TripPage({
  params,
}: {
  params: Promise<{ tripUuid: string }>;
}) {
  return (
    <Suspense fallback={<main className="mx-auto min-h-dvh w-full max-w-140 bg-white" />}>
      <TripContent params={params} />
    </Suspense>
  );
}
