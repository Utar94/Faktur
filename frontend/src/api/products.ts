import type { CreateOrReplaceProductPayload, Product, SearchProductsPayload } from "@/types/products";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, put } from ".";

function createStoreUrlBuilder(storeId: string, articleId?: string): IUrlBuilder {
  if (articleId) {
    return new UrlBuilder({ path: "/stores/{storeId}/products/{articleId}" }).setParameter("storeId", storeId).setParameter("articleId", articleId);
  }
  return new UrlBuilder({ path: "/stores/{storeId}/products" }).setParameter("storeId", storeId);
}
function createUrlBuilder(id: string): IUrlBuilder {
  return new UrlBuilder({ path: "/products/{id}" }).setParameter("id", id);
}

export async function createOrReplaceProduct(storeId: string, articleId: string, payload: CreateOrReplaceProductPayload, version?: number): Promise<Product> {
  const url: string = createStoreUrlBuilder(storeId, articleId).setQueryString(`?version=${version}`).buildRelative();
  return (await put<CreateOrReplaceProductPayload, Product>(url, payload)).data;
}

export async function deleteProduct(id: string): Promise<Product> {
  return (await _delete<Product>(createUrlBuilder(id).buildRelative())).data;
}

export async function readProduct(id: string): Promise<Product> {
  return (await get<Product>(createUrlBuilder(id).buildRelative())).data;
}
export async function readProductByArticle(storeId: string, articleId: string): Promise<Product> {
  return (await get<Product>(createStoreUrlBuilder(storeId, articleId).buildRelative())).data;
}

export async function searchProducts(payload: SearchProductsPayload): Promise<SearchResults<Product>> {
  const url: string = createStoreUrlBuilder(payload.storeId)
    .setQuery("department", payload.departmentNumber ?? "")
    .setQuery("unit_type", payload.unitType ?? "")
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
  return (await get<SearchResults<Product>>(url)).data;
}
