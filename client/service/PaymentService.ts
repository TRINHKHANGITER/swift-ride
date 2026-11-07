import { httpClient } from "@/lib/http-client";
import type { PaymentContract } from "@/service/contract/PaymentContract";

class PaymentService {
  async getMyPayments(): Promise<PaymentContract[]> {
    const response = await httpClient.get<PaymentContract[]>("/ts/payments");
    return response.data;
  }

  async getMyDriverPayments(): Promise<PaymentContract[]> {
    const response = await httpClient.get<PaymentContract[]>("/ts/payments/driver");
    return response.data;
  }
}

export const paymentService = new PaymentService();
