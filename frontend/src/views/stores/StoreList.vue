<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import BannerSelect from "@/components/banners/BannerSelect.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Store, StoreSort, SearchStoresPayload } from "@/types/stores";
import { handleErrorKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { searchStores } from "@/api/stores";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const stores = ref<Store[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const bannerId = computed<string>(() => route.query.bannerId?.toString() ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("stores.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchStoresPayload = {
    bannerId: bannerId.value,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as StoreSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchStores(payload);
    if (now === timestamp.value) {
      stores.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "bannerId":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "StoreList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                bannerId: "",
                search: "",
                sort: "UpdatedOn",
                isDescending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("stores.title.list") }}</h1>
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <RouterLink :to="{ name: 'CreateStore', query: { bannerId: bannerId || undefined } }" class="btn btn-success ms-1">
        <font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}
      </RouterLink>
    </div>
    <div class="row">
      <BannerSelect class="col-lg-3" :model-value="bannerId" no-status @error="handleError" @update:model-value="setQuery('bannerId', $event ?? '')" />
      <SearchInput class="col-lg-3" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-3"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-3" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="stores.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("stores.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("banners.select.label") }}</th>
            <th scope="col">{{ t("stores.sort.options.Number") }}</th>
            <th scope="col">{{ t("stores.sort.options.DepartmentCount") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="store in stores" :key="store.id">
            <td>
              <RouterLink :to="{ name: 'StoreEdit', params: { id: store.id } }"> <font-awesome-icon icon="fas fa-edit" />{{ store.displayName }} </RouterLink>
            </td>
            <td scope="col">
              <RouterLink v-if="store.banner" :to="{ name: 'BannerEdit', params: { id: store.banner.id } }">
                <font-awesome-icon icon="fas fa-eye" />{{ store.banner.displayName }}
              </RouterLink>
              <template v-else>{{ "—" }}</template>
            </td>
            <td>{{ store.number ?? "—" }}</td>
            <td>{{ store.departmentCount }}</td>
            <td><StatusBlock :actor="store.updatedBy" :date="store.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("stores.empty") }}</p>
  </main>
</template>
