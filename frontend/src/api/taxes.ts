import type { Tax, CreateTaxPayload, ReplaceTaxPayload, SearchTaxesPayload } from "@/types/taxes";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, post, put } from ".";

function createUrlBuilder(id?: string): IUrlBuilder {
  if (id) {
    return new UrlBuilder({ path: "/taxes/{id}" }).setParameter("id", id);
  }
  return new UrlBuilder({ path: "/taxes" });
}

export async function createTax(payload: CreateTaxPayload): Promise<Tax> {
  return (await post<CreateTaxPayload, Tax>(createUrlBuilder().buildRelative(), payload)).data;
}

export async function deleteTax(id: string): Promise<Tax> {
  return (await _delete<Tax>(createUrlBuilder(id).buildRelative())).data;
}

export async function readTax(id: string): Promise<Tax> {
  return (await get<Tax>(createUrlBuilder(id).buildRelative())).data;
}

export async function replaceTax(id: string, payload: ReplaceTaxPayload, version?: number): Promise<Tax> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<CreateTaxPayload, Tax>(url, payload)).data;
}

export async function searchTaxes(payload: SearchTaxesPayload): Promise<SearchResults<Tax>> {
  const url: string = createUrlBuilder()
    .setQuery("flag", payload.flag ?? "")
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
  return (await get<SearchResults<Tax>>(url)).data;
}
