<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { ReceiptTotal } from "@/types/receipts";

const { n, t } = useI18n();

defineProps<ReceiptTotal>();
</script>

<template>
  <div>
    <p>{{ t("receipts.totals.subTotal", { subTotal: n(subTotal, "currency") }) }}</p>
    <p>
      <template v-for="tax in taxes" :key="tax.code">
        {{
          t("receipts.totals.tax", {
            code: tax.code,
            taxableAmount: n(tax.taxableAmount, "currency"),
            rate: n(tax.rate, "percent"),
            amount: n(tax.amount, "currency"),
          })
        }}
        <br />
      </template>
    </p>
    <p>{{ t("receipts.totals.total", { total: n(total, "currency") }) }}</p>
  </div>
</template>
