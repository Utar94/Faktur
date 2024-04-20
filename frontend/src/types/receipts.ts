import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";
import type { Store } from "./stores";

export type CategorizeReceiptPayload = {
  itemCategories: ReceiptItemCategory[];
};

export type CategorySavedEvent = {
  newCategory: string;
  oldCategory?: string;
};

export type CreateOrReplaceReceiptItemPayload = {
  gtinOrSku: string;
  label: string;
  flags?: string;
  quantity?: number;
  unitPrice?: number;
  price: number;
  department?: DepartmentSummary;
};

export type DepartmentSummary = {
  number: string;
  displayName: string;
  description?: string;
};

export type ImportReceiptPayload = {
  storeId: string;
  issuedOn?: Date;
  number?: string;
  locale?: string;
  lines?: string;
};

export type Receipt = Aggregate & {
  issuedOn: string;
  number?: string;
  itemCount: number;
  items: ReceiptItem[];
  subTotal: number;
  total: number;
  taxes: ReceiptTax[];
  hasBeenProcessed: boolean;
  processedBy?: Actor;
  processedOn?: string;
  store: Store;
};

export type ReceiptItem = {
  number: number;
  gtin?: string;
  sku?: string;
  label: string;
  flags?: string;
  quantity: number;
  unitPrice: number;
  price: number;
  department?: DepartmentSummary;
  category?: string;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
  receipt: Receipt;
};

export type ReceiptItemCategory = {
  number: number;
  category?: string;
};

export type ReceiptSort = "IssuedOn" | "Number" | "ProcessedOn" | "SubTotal " | "Total" | "UpdatedOn";

export type ReceiptSortOption = SortOption & {
  field: ReceiptSort;
};

export type ReceiptTax = {
  code: string;
  flags: string;
  rate: number;
  taxableAmount: number;
  amount: number;
};

export type ReceiptTotal = {
  subTotal: number;
  taxes: ReceiptTax[];
  total: number;
};

export type ReplaceReceiptPayload = {
  issuedOn: Date;
  number?: string;
};

export type SearchReceiptsPayload = SearchPayload & {
  storeId?: string;
  isEmpty?: boolean;
  hasBeenProcessed?: boolean;
  sort: ReceiptSortOption[];
};
