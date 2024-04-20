<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { ReceiptItem } from "@/types/receipts";

const { n } = useI18n();

defineProps<{
  item: ReceiptItem;
}>();

defineEmits<{
  (e: "clicked"): void;
}>();
</script>

<template>
  <table class="table table-borderless" @click="$emit('clicked')">
    <tr>
      <td>
        <strong>{{ item.label }}</strong>
        <template v-if="item.flags"
          >&nbsp;<sub>({{ item.flags }})</sub></template
        >
      </td>
      <td class="text-end">
        <strong>{{ n(item.price, "currency") }}</strong>
      </td>
    </tr>
    <tr>
      <td>{{ item.gtin ?? item.sku }}</td>
      <td class="text-end">{{ n(item.quantity) }} &times; {{ n(item.unitPrice, "currency") }}</td>
    </tr>
  </table>
</template>

<style scoped>
table {
  border: 1px solid lightgray;
  --bs-table-bg: transparent;
}
table:hover {
  cursor: pointer;
  background-color: lightgray;
}

td {
  padding: 0.75rem;
}
</style>
