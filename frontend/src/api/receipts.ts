import type { CategorizeReceiptPayload, ImportReceiptPayload, Receipt, ReplaceReceiptPayload, SearchReceiptsPayload } from "@/types/receipts";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

export async function categorizeReceipt(id: string, payload: CategorizeReceiptPayload): Promise<Receipt> {
  return (await patch<CategorizeReceiptPayload, Receipt>(`/receipts/${id}/categorize`, payload)).data;
}

export async function deleteReceipt(id: string): Promise<Receipt> {
  return (await _delete<Receipt>(`/receipts/${id}`)).data;
}

export async function importReceipt(payload: ImportReceiptPayload): Promise<Receipt> {
  return (await post<ImportReceiptPayload, Receipt>("/receipts/import", payload)).data;
}

export async function readReceipt(id: string): Promise<Receipt> {
  return (await get<Receipt>(`/receipts/${id}`)).data;
}

export async function replaceReceipt(id: string, payload: ReplaceReceiptPayload, version?: number): Promise<Receipt> {
  const query: string | undefined = version ? `?version=${version}` : "";
  return (await put<ReplaceReceiptPayload, Receipt>(`/receipts/${id}${query}`, payload)).data;
}

export async function searchReceipts(payload: SearchReceiptsPayload): Promise<SearchResults<Receipt>> {
  const params: string[] = [];
  payload.ids.forEach((id) => params.push(`ids=${id}`));
  if (payload.search.terms.length) {
    payload.search.terms.forEach((term) => params.push(`search_terms=${term.value}`));
    params.push(`search_operator=${payload.search.operator}`);
  }
  if (payload.storeId) {
    params.push(`store=${payload.storeId}`);
  }
  if (typeof payload.isEmpty !== "undefined") {
    params.push(`empty=${payload.isEmpty}`);
  }
  if (typeof payload.hasBeenProcessed !== "undefined") {
    params.push(`processed=${payload.hasBeenProcessed}`);
  }
  payload.sort.forEach((sort) => params.push(`sort=${sort.isDescending ? `DESC.${sort.field}` : sort.field}`));
  params.push(`skip=${payload.skip}`);
  params.push(`limit=${payload.limit}`);
  const query: string | undefined = params.length ? `?${params.join("&")}` : "";
  return (await get<SearchResults<Receipt>>(`/receipts${query}`)).data;
}
