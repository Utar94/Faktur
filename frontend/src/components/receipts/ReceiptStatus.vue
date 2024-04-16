<script setup lang="ts">
import { useI18n } from "vue-i18n";

import StatusInfo from "@/components/shared/StatusInfo.vue";
import type { Receipt } from "@/types/receipts";

defineProps<{
  receipt: Receipt;
}>();

const { d, t } = useI18n();
</script>

<template>
  <p>
    <span>
      {{ t("receipts.issuedOn.format", { date: d(receipt.issuedOn, "medium") }) }}
      <RouterLink :to="{ name: 'StoreEdit', params: { id: receipt.store.id } }">
        <font-awesome-icon icon="fas fa-store" />{{ receipt.store.displayName }}
      </RouterLink>
    </span>
    <template v-if="receipt.processedBy && receipt.processedOn">
      <br />
      <StatusInfo :actor="receipt.processedBy" :date="receipt.processedOn" format="receipts.processedOn" />
    </template>
  </p>
</template>
