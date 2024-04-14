import type { CreateOrReplaceProductPayload, Product, SearchProductsPayload } from "@/types/products";
import type { SearchResults } from "@/types/search";
import { _delete, get, put } from ".";

export async function createOrReplaceProduct(storeId: string, articleId: string, payload: CreateOrReplaceProductPayload, version?: number): Promise<Product> {
  const query: string | undefined = version ? `?version=${version}` : "";
  return (await put<CreateOrReplaceProductPayload, Product>(`/stores/${storeId}/products/${articleId}${query}`, payload)).data;
}

export async function deleteProduct(id: string): Promise<Product> {
  return (await _delete<Product>(`/products/${id}`)).data;
}

export async function readProduct(id: string): Promise<Product> {
  return (await get<Product>(`/products/${id}`)).data;
}

export async function searchProducts(payload: SearchProductsPayload): Promise<SearchResults<Product>> {
  const params: string[] = [];
  payload.ids.forEach((id) => params.push(`ids=${id}`));
  if (payload.search.terms.length) {
    payload.search.terms.forEach((term) => params.push(`search_terms=${term.value}`));
    params.push(`search_operator=${payload.search.operator}`);
  }
  if (payload.departmentNumber) {
    params.push(`department=${payload.departmentNumber}`);
  }
  if (payload.unitType) {
    params.push(`unit_type=${payload.unitType}`);
  }
  payload.sort.forEach((sort) => params.push(`sort=${sort.isDescending ? `DESC.${sort.field}` : sort.field}`));
  params.push(`skip=${payload.skip}`);
  params.push(`limit=${payload.limit}`);
  const query: string | undefined = params.length ? `?${params.join("&")}` : "";
  return (await get<SearchResults<Product>>(`/stores/${payload.storeId}/products${query}`)).data;
}
