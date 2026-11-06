import { httpClient } from "@/lib/http-client";
import {
  QuoteRequestContract,
  QuoteResponseContract,
} from "./contract/QuoteContract";

class QuoteService {
  async requestQuote(
    contract: QuoteRequestContract,
  ): Promise<QuoteResponseContract> {
    const response = await httpClient.post<QuoteResponseContract>(
      "/mp/quote",
      contract,
    );

    return response.data;
  }
}

export const quoteService = new QuoteService();
