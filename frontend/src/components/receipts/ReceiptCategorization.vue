<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import ReceiptItemCard from "./ReceiptItemCard.vue";
import ReceiptTotal from "./ReceiptTotal.vue";
import type { Receipt, ReceiptItem, ReceiptTotal as TReceiptTotal } from "@/types/receipts";
import { isTaxable } from "@/helpers/taxUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { useCategoryStore } from "@/stores/categories";

const categories = useCategoryStore();
const { t } = useI18n();

type ItemGroup = {
  department: string;
  items: ReceiptItem[];
};

const props = withDefaults(
  defineProps<{
    processing?: boolean;
    receipt: Receipt;
  }>(),
  {
    processing: false,
  },
);

const categorizedItems = ref<Map<number, string>>(new Map<number, string>());

const groupedItems = computed<ItemGroup[]>(() => {
  const itemsByDepartment = new Map<string, ReceiptItem[]>();
  for (const item of props.receipt.items) {
    const department = item.department ? [item.department.number, item.department.displayName].join("-") : "";
    const items = itemsByDepartment.get(department) ?? [];
    items.push(item);
    itemsByDepartment.set(department, items);
  }
  const groupedItems: ItemGroup[] = [];
  itemsByDepartment.forEach((items, department) => groupedItems.push({ department, items: orderBy(items, "number") }));
  return orderBy(groupedItems, "department");
});
const taxFlags = computed<Map<string, string>>(() => {
  const taxFlags = new Map<string, string>();
  props.receipt.taxes.forEach((tax) => taxFlags.set(tax.code, tax.flags));
  return taxFlags;
});

const categoryClass = computed<string>(() => {
  const width = Math.trunc(12 / (1 + categories.categories.length));
  return `col-${width}`;
});
const noCategoryClass = computed<string>(() => {
  let width = Math.trunc(12 / (1 + categories.categories.length));
  width += 12 % (1 + categories.categories.length);
  return `col-${width}`;
});

function calculateTotal(category?: string): TReceiptTotal {
  const total: TReceiptTotal = {
    subTotal: 0,
    taxes: props.receipt.taxes.map(({ code, flags, rate }) => ({ code, flags, rate, taxableAmount: 0, amount: 0 })),
    total: 0,
  };
  props.receipt.items.forEach((item) => {
    const value: string | undefined = categorizedItems.value.get(item.number);
    if (value === category) {
      total.subTotal += item.price;
      if (item.flags) {
        total.taxes.forEach((tax) => {
          const flags: string | undefined = taxFlags.value.get(tax.code);
          if (flags && isTaxable(flags, item.flags ?? "")) {
            tax.taxableAmount += item.price;
          }
        });
      }
    }
  });
  total.total = total.subTotal;
  total.taxes.forEach((tax) => {
    tax.amount = tax.taxableAmount * tax.rate;
    total.total += tax.amount;
  });
  return total;
}

function categorize(item: ReceiptItem, category?: string): void {
  if (category) {
    categorizedItems.value.set(item.number, category);
  } else {
    categorizedItems.value.delete(item.number);
  }
}

function scrollToTop(): void {
  window.scrollTo(0, 0);
}

function deleteCategory(category: string): void {
  categorizedItems.value.forEach((value, number, map) => {
    if (value === category) {
      map.delete(number);
    }
  });
}
function renameCategory(oldCategory: string, newCategory: string): void {
  categorizedItems.value.forEach((value, number, map) => {
    if (value === oldCategory) {
      map.set(number, newCategory);
    }
  });
}
defineExpose({ deleteCategory, renameCategory });

const emit = defineEmits<{
  (e: "categorized", value: Map<number, string>): void;
  (e: "edit", value: ReceiptItem): void;
}>();

function onProcess(): void {
  emit("categorized", categorizedItems.value);
}

watchEffect(() => {
  const receipt = props.receipt;
  categorizedItems.value.clear();
  receipt.items.forEach((item) => {
    if (item.category) {
      categorizedItems.value.set(item.number, item.category);
    }
  });
});
</script>

<template>
  <div>
    <div class="row text-center">
      <div :class="noCategoryClass">
        <h2>{{ t("receipts.categories.none") }}</h2>
      </div>
      <div :class="categoryClass" v-for="category in categories.categories" :key="category">
        <h2>{{ category }}</h2>
      </div>
    </div>
    <div v-for="group in groupedItems" :key="group.department">
      <h4>{{ group.department }}</h4>
      <div class="row" v-for="item in group.items" :key="item.number">
        <div v-if="!categorizedItems.has(item.number)" :class="noCategoryClass">
          <ReceiptItemCard :item="item" @clicked="$emit('edit', item)" />
        </div>
        <div v-else :class="`clickable ${noCategoryClass}`" @click="categorize(item)"></div>
        <template v-for="category in categories.categories" :key="category">
          <div v-if="categorizedItems.get(item.number) === category" :class="categoryClass">
            <ReceiptItemCard :item="item" @clicked="$emit('edit', item)" />
          </div>
          <div v-else :class="`clickable ${categoryClass}`" @click="categorize(item, category)"></div>
        </template>
      </div>
    </div>
    <div>
      <h2>{{ t("receipts.totals.title") }}</h2>
      <ReceiptTotal v-bind="receipt" />
      <div class="row">
        <ReceiptTotal :class="noCategoryClass" v-bind="calculateTotal()" />
        <ReceiptTotal :class="categoryClass" v-for="category in categories.categories" :key="category" v-bind="calculateTotal(category)" />
      </div>
    </div>
    <div class="mb-3">
      <TarButton
        class="me-1"
        :disabled="processing"
        icon="fas fa-dollar-sign"
        :loading="processing"
        :status="t('loading')"
        :text="t('actions.process')"
        @click="onProcess"
      />
      <AppBackButton class="ms-1" />
      <TarButton class="float-end" icon="fas fa-arrow-up" :text="t('actions.back')" variant="info" @click="scrollToTop" />
    </div>
  </div>
</template>

<style scoped>
.clickable:hover {
  cursor: pointer;
  background-color: lightgray;
}
</style>
