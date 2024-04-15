<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Receipt } from "@/types/receipts";

const { d, n, t } = useI18n();

defineProps<{
  receipts: Receipt[];
}>();
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th scope="col">{{ t("receipts.sort.options.IssuedOn") }}</th>
        <th scope="col">{{ t("receipts.sort.options.Number") }}</th>
        <th scope="col">{{ t("receipts.itemCount") }}</th>
        <th scope="col">{{ t("receipts.sort.options.Total") }}</th>
        <th scope="col">{{ t("receipts.sort.options.ProcessedOn") }}</th>
        <th scope="col">{{ t("receipts.sort.options.UpdatedOn") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="receipt in receipts" :key="receipt.id">
        <td>
          <RouterLink :to="{ name: 'ReceiptEdit', params: { id: receipt.id } }">
            <font-awesome-icon icon="fas fa-edit" />{{ d(receipt.issuedOn, "medium") }}
          </RouterLink>
        </td>
        <td>{{ receipt.number ?? "—" }}</td>
        <td>{{ receipt.itemCount }}</td>
        <td>{{ n(receipt.total, "currency") }}</td>
        <td>
          <template v-if="receipt.processedOn">{{ d(receipt.processedOn, "medium") }}</template>
          <TarBadge v-else variant="warning">{{ t("receipts.status.options.new") }}</TarBadge>
        </td>
        <td><StatusBlock :actor="receipt.updatedBy" :date="receipt.updatedOn" /></td>
      </tr>
    </tbody>
  </table>
</template>
