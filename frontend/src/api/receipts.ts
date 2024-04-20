import type {
  CategorizeReceiptPayload,
  CreateOrReplaceReceiptItemPayload,
  ImportReceiptPayload,
  Receipt,
  ReceiptItem,
  ReplaceReceiptPayload,
  SearchReceiptsPayload,
} from "@/types/receipts";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): IUrlBuilder {
  if (id) {
    return new UrlBuilder({ path: "/receipts/{id}" }).setParameter("id", id);
  }
  return new UrlBuilder({ path: "/receipts" });
}

export async function categorizeReceipt(id: string, payload: CategorizeReceiptPayload): Promise<Receipt> {
  const url: string = new UrlBuilder({ path: "/receipts/{id}/categorize" }).setParameter("id", id).buildRelative();
  return (await patch<CategorizeReceiptPayload, Receipt>(url, payload)).data;
}

export async function createOrReplaceReceiptItem(
  receiptId: string,
  number: number,
  payload: CreateOrReplaceReceiptItemPayload,
  version?: number,
): Promise<ReceiptItem> {
  const url: string = new UrlBuilder({ path: "/receipts/{id}/items/{number}" })
    .setParameter("id", receiptId)
    .setParameter("number", number.toString())
    .setQueryString(`?version=${version}`)
    .buildRelative();
  return (await put<CreateOrReplaceReceiptItemPayload, ReceiptItem>(url, payload)).data;
}

export async function deleteReceipt(id: string): Promise<Receipt> {
  return (await _delete<Receipt>(createUrlBuilder(id).buildRelative())).data;
}

export async function importReceipt(payload: ImportReceiptPayload): Promise<Receipt> {
  const url: string = new UrlBuilder({ path: "/receipts/import" }).buildRelative();
  return (await post<ImportReceiptPayload, Receipt>(url, payload)).data;
}

export async function readReceipt(id: string): Promise<Receipt> {
  return (await get<Receipt>(createUrlBuilder(id).buildRelative())).data;
}

export async function replaceReceipt(id: string, payload: ReplaceReceiptPayload, version?: number): Promise<Receipt> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<ReplaceReceiptPayload, Receipt>(url, payload)).data;
}

export async function searchReceipts(payload: SearchReceiptsPayload): Promise<SearchResults<Receipt>> {
  const params: string[] = [];
  if (payload.storeId) {
    params.push(`store=${payload.storeId}`);
  }
  if (typeof payload.isEmpty !== "undefined") {
    params.push(`empty=${payload.isEmpty}`);
  }
  if (typeof payload.hasBeenProcessed !== "undefined") {
    params.push(`processed=${payload.hasBeenProcessed}`);
  }

  const url: string = createUrlBuilder()
    .setQuery("store", payload.storeId ?? "")
    .setQuery("empty", payload.isEmpty?.toString() ?? "")
    .setQuery("processed", payload.hasBeenProcessed?.toString() ?? "")
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Receipt>>(url)).data;
}
