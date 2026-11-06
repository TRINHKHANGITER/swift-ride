export const tripStatusName = (status: string | number) => {
  if (typeof status === "string") return status;

  return [
    "Requested",
    "DriverAccepted",
    "EnRoute",
    "PickedUp",
    "DroppedOff",
    "PaymentPending",
    "Paid",
    "Cancelled",
    "Failed",
  ][status] ?? "Unknown";
};
