import type { CreateOrReplaceDepartmentPayload, Department, SearchDepartmentsPayload } from "@/types/departments";
import type { SearchResults } from "@/types/search";
import { UrlBuilder, type IUrlBuilder } from "@/helpers/urlUtils";
import { _delete, get, put } from ".";

function createUrlBuilder(storeId: string, number?: string): IUrlBuilder {
  if (number) {
    return new UrlBuilder({ path: "/stores/{storeId}/departments/{number}" }).setParameter("storeId", storeId).setParameter("number", number);
  }
  return new UrlBuilder({ path: "/stores/{storeId}/departments" }).setParameter("storeId", storeId);
}

export async function createOrReplaceDepartment(
  storeId: string,
  number: string,
  payload: CreateOrReplaceDepartmentPayload,
  version?: number,
): Promise<Department> {
  const url: string = createUrlBuilder(storeId, number).setQueryString(`?version=${version}`).buildRelative();
  return (await put<CreateOrReplaceDepartmentPayload, Department>(url, payload)).data;
}

export async function deleteDepartment(storeId: string, number: string): Promise<Department> {
  return (await _delete<Department>(createUrlBuilder(storeId, number).buildRelative())).data;
}

export async function searchDepartments(payload: SearchDepartmentsPayload): Promise<SearchResults<Department>> {
  const url: string = createUrlBuilder(payload.storeId)
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
  return (await get<SearchResults<Department>>(url)).data;
}
