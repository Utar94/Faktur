import i18n from "@/i18n";
import type { Department } from "@/types/departments";
import type { Receipt } from "@/types/receipts";

export function formatDepartment(department: Department): string {
  return `${department.displayName} (#${department.number})`;
} // TODO(fpion): tests

export function formatReceipt(receipt: Receipt): string {
  const { d } = i18n.global;
  const date = new Date(receipt.issuedOn);
  return receipt.number ? `${d(date, "medium")} (#${receipt.number})` : d(date, "medium");
} // TODO(fpion): tests
