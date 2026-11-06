import { CarFront } from "lucide-react";

type SwiftRideLogoProps = {
  compact?: boolean;
  inverted?: boolean;
};

export function SwiftRideLogo({ compact = false, inverted = false }: SwiftRideLogoProps) {
  return (
    <div className="flex items-center gap-2" aria-label="Swift Ride">
      <span
        className={`grid size-10 place-items-center rounded-xl ${
          inverted ? "bg-white text-[#126b4f]" : "bg-[#126b4f] text-white"
        }`}
      >
        <CarFront className="size-6" strokeWidth={2.3} />
      </span>
      {!compact && (
        <span className={`text-xl font-bold tracking-[-0.02em] ${inverted ? "text-white" : "text-[#103d31]"}`}>
          Swift Ride
        </span>
      )}
    </div>
  );
}
