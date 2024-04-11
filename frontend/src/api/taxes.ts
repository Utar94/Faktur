import type { Tax, CreateTaxPayload, ReplaceTaxPayload, SearchTaxesPayload } from "@/types/taxes";
import type { SearchResults } from "@/types/search";
import { _delete, get, post, put } from ".";

export async function createTax(payload: CreateTaxPayload): Promise<Tax> {
  return (await post<CreateTaxPayload, Tax>("/taxes", payload)).data;
}

export async function deleteTax(id: string): Promise<Tax> {
  return (await _delete<Tax>(`/taxes/${id}`)).data;
}

export async function readTax(id: string): Promise<Tax> {
  return (await get<Tax>(`/taxes/${id}`)).data;
}

export async function replaceTax(id: string, payload: ReplaceTaxPayload, version?: number): Promise<Tax> {
  const query: string | undefined = version ? `?version=${version}` : undefined;
  return (await put<CreateTaxPayload, Tax>(`/taxes/${id}${query}`, payload)).data;
}

export async function searchTaxes(payload: SearchTaxesPayload): Promise<SearchResults<Tax>> {
  const params: string[] = [];
  if (payload.flag) {
    params.push(`flag=${payload.flag}`);
  }
  payload.ids.forEach((id) => params.push(`ids=${id}`));
  if (payload.search.terms.length) {
    payload.search.terms.forEach((term) => params.push(`search_terms=${term.value}`));
    params.push(`search_operator=${payload.search.operator}`);
  }
  payload.sort.forEach((sort) => params.push(`sort=${sort.isDescending ? `DESC.${sort.field}` : sort.field}`));
  params.push(`skip=${payload.skip}`);
  params.push(`limit=${payload.limit}`);
  const query: string | undefined = params.length ? `?${params.join("&")}` : undefined;
  return (await get<SearchResults<Tax>>(`/taxes${query}`)).data;
}
