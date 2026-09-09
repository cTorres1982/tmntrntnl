"use client";

import { useQuery } from "@tanstack/react-query";
import type { CustomerSummary } from "@/core/domain/customer-summary.type";

/** Backs the create-job form's customer picker — a dropdown of real customers instead of a free-text GUID field, so an unknown customer id can't be typed in the first place. */
export function useCustomers() {
  const query = useQuery({
    queryKey: ["customers"],
    queryFn: async (): Promise<CustomerSummary[]> => {
      const response = await fetch("/api/customers");
      if (!response.ok) {
        throw new Error(`Failed to load customers (${response.status}).`);
      }
      return (await response.json()) as CustomerSummary[];
    },
  });

  return {
    customers: query.data ?? [],
    isLoading: query.isLoading,
    isError: query.isError,
  };
}
