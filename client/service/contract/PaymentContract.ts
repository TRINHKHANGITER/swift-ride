export type PaymentStatus = "Pending" | "Settled" | "Failed" | "Refunded" | number;

export type PaymentContract = {
  uuid: string;
  tripUuid: string;
  riderUuid: string;
  driverUuid: string;
  amount: number;
  qrUrl: string;
  transferCode: string;
  status: PaymentStatus;
};
