<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import EmptySelect from "@/components/receipts/EmptySelect.vue";
import ReceiptList from "@/components/receipts/ReceiptList.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusSelect from "@/components/receipts/StatusSelect.vue";
import StoreSelect from "@/components/stores/StoreSelect.vue";
import type { Receipt, ReceiptSort, SearchReceiptsPayload } from "@/types/receipts";
import type { Store } from "@/types/stores";
import { handleErrorKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { readStore } from "@/api/stores";
import { searchReceipts } from "@/api/receipts";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const receipts = ref<Receipt[]>([]);
const store = ref<Store>();
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const hasBeenProcessed = computed<boolean | undefined>(() => parseBoolean(route.query.hasBeenProcessed?.toString()));
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const isReceiptEmpty = computed<boolean | undefined>(() => parseBoolean(route.query.isEmpty?.toString()));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const storeId = computed<string>(() => route.query.storeId?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("receipts.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  if (storeId.value && store.value?.id !== storeId.value) {
    store.value = await readStore(storeId.value);
  }
  const payload: SearchReceiptsPayload = {
    hasBeenProcessed: hasBeenProcessed.value,
    ids: [],
    isEmpty: isReceiptEmpty.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    storeId: storeId.value,
    sort: sort.value ? [{ field: sort.value as ReceiptSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchReceipts(payload);
    if (now === timestamp.value) {
      receipts.value = results.items;
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
    case "hasBeenProcessed":
    case "isEmpty":
    case "search":
    case "storeId":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ReceiptList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                hasBeenProcessed: "",
                isEmpty: "",
                search: "",
                storeId: "",
                sort: "IssuedOn",
                isDescending: "true",
                page: 1,
                count: 100,
              }
            : {
                page: 1,
                count: 100,
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
    <h1>{{ t("receipts.title.list") }}</h1>
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
      <RouterLink :to="{ name: 'ImportReceipt', query: { storeId: storeId || undefined } }" class="btn btn-success ms-1">
        <font-awesome-icon icon="fas fa-file-invoice-dollar" /> {{ t("actions.import") }}
      </RouterLink>
    </div>
    <div class="row">
      <StoreSelect class="col-lg-4" :model-value="storeId" @error="handleError" @update:model-value="setQuery('storeId', $event ?? '')" />
      <EmptySelect class="col-lg-4" :model-value="isReceiptEmpty" @update:model-value="setQuery('isEmpty', $event?.toString() ?? '')" />
      <StatusSelect class="col-lg-4" :model-value="hasBeenProcessed" @update:model-value="setQuery('hasBeenProcessed', $event?.toString() ?? '')" />
    </div>
    <div class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="receipts.length">
      <ReceiptList :receipts="receipts" />
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("receipts.empty") }}</p>
  </main>
</template>
