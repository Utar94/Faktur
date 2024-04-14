<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import CategoryList from "@/components/receipts/CategoryList.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import type { ApiError } from "@/types/api";
import type { Receipt } from "@/types/receipts";
import { deleteReceipt, readReceipt } from "@/api/receipts";
import { formatReceipt } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { d, t } = useI18n();

const isDeleting = ref<boolean>(false);
const receipt = ref<Receipt>();

const displayName = computed<string>(() => (receipt.value ? formatReceipt(receipt.value) : ""));
const title = computed<string>(() => t("receipts.title.edit", { receipt: displayName.value }));

async function onDelete(hideModal: () => void): Promise<void> {
  if (receipt.value && !isDeleting.value) {
    isDeleting.value = true;
    try {
      await deleteReceipt(receipt.value.id);
      hideModal();
      toasts.success("receipts.delete.success");
      router.push({ name: "ReceiptList" });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isDeleting.value = false;
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      receipt.value = await readReceipt(id);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container-fluid">
    <template v-if="receipt">
      <h1>{{ title }}</h1>
      <StatusDetail :aggregate="receipt" />
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
      <div class="mb-3">
        <AppBackButton class="me-1" />
        <AppDelete
          v-if="receipt"
          class="ms-1"
          confirm="receipts.delete.confirm"
          :displayName="displayName"
          :loading="isDeleting"
          title="receipts.delete.title"
          @confirmed="onDelete"
        />
      </div>
      <TarTabs>
        <TarTab active id="items" :title="`${t('receipts.items.title')} (${receipt.itemCount})`">
          <!-- <ReceiptItemList :categories="categories" :receipt="receipt" /> -->
        </TarTab>
        <TarTab id="categories" :title="t('receipts.categories.title.list')">
          <CategoryList />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
