<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { ReceiptItem } from "@/types/receipts";

const { n } = useI18n();

defineProps<{
  item: ReceiptItem;
}>();

function onClick(): void {
  // TODO(fpion): implement
}
</script>

<template>
  <table class="table table-borderless" @click="onClick">
    <tr>
      <td>
        <strong>{{ item.label }}</strong>
        <template v-if="item.flags"
          >&nbsp;<sub>({{ item.flags }})</sub></template
        >
      </td>
      <td class="text-right">
        <strong>{{ n(item.price, "currency") }}</strong>
      </td>
    </tr>
    <tr>
      <td>{{ item.gtin ?? item.sku }}</td>
      <td class="text-right">{{ n(item.quantity) }} &times; {{ n(item.unitPrice, "currency") }}</td>
    </tr>
  </table>
</template>

<style scoped>
table {
  border: 1px solid lightgray;
}
/* table:hover {
  cursor: pointer;
  background-color: lightgray;
} TODO(fpion): not working correctly */
</style>
