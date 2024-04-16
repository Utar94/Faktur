<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import CategoryList from "@/components/receipts/CategoryList.vue";
import IssuedOnInput from "@/components/receipts/IssuedOnInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import ReceiptCategorization from "@/components/receipts/ReceiptCategorization.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import type { ApiError } from "@/types/api";
import type { CategorySavedEvent, Receipt } from "@/types/receipts";
import { categorizeReceipt, deleteReceipt, readReceipt, replaceReceipt } from "@/api/receipts";
import { formatReceipt } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { toDateTimeLocal } from "@/helpers/dateUtils";
import { useCategoryStore } from "@/stores/categories";
import { useToastStore } from "@/stores/toast";

const categories = useCategoryStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { d, t } = useI18n();

const categorization = ref<InstanceType<typeof ReceiptCategorization> | null>(null);
const isDeleting = ref<boolean>(false);
const isProcessing = ref<boolean>(false);
const issuedOn = ref<string>("");
const number = ref<string>("");
const receipt = ref<Receipt>();

const displayName = computed<string>(() => (receipt.value ? formatReceipt(receipt.value) : ""));
const hasChanges = computed<boolean>(() =>
  receipt.value ? issuedOn.value !== toDateTimeLocal(new Date(receipt.value.issuedOn)) || number.value !== (receipt.value.number ?? "") : false,
);
const title = computed<string>(() => t("receipts.title.edit", { receipt: displayName.value }));

async function onCategorized(categorizedItems: Map<number, string>): Promise<void> {
  if (receipt.value && !isProcessing.value) {
    isProcessing.value = true;
    try {
      const categorizedReceipt = await categorizeReceipt(receipt.value.id, {
        itemCategories: receipt.value.items.map(({ number }) => ({ number, category: categorizedItems.get(number) })),
      });
      setModel(categorizedReceipt);
      toasts.success("receipts.processed");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isProcessing.value = false;
    }
  }
}

function onCategoryDeleted(category: string): void {
  categorization.value?.deleteCategory(category);
}
function onCategorySaved(event: CategorySavedEvent): void {
  if (event.oldCategory) {
    categorization.value?.renameCategory(event.oldCategory, event.newCategory);
  }
}

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

function setModel(model: Receipt): void {
  receipt.value = model;
  categories.load(model);
  issuedOn.value = toDateTimeLocal(new Date(model.issuedOn));
  number.value = model.number ?? "";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (receipt.value) {
    try {
      const updatedReceipt = await replaceReceipt(
        receipt.value.id,
        {
          issuedOn: new Date(issuedOn.value),
          number: number.value,
        },
        receipt.value.version,
      );
      setModel(updatedReceipt);
      toasts.success("receipts.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const receipt = await readReceipt(id);
      setModel(receipt);
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
          <ReceiptCategorization :processing="isProcessing" :receipt="receipt" ref="categorization" @categorized="onCategorized" />
        </TarTab>
        <TarTab id="categories" :title="t('receipts.categories.title.list')">
          <CategoryList @deleted="onCategoryDeleted" @saved="onCategorySaved" />
        </TarTab>
        <TarTab id="edit" :title="t('actions.edit')">
          <form @submit.prevent="onSubmit">
            <div class="mb-3">
              <AppSaveButton :disabled="isSubmitting || !hasChanges" exists :loading="isSubmitting" />
            </div>
            <IssuedOnInput v-model="issuedOn" />
            <NumberInput v-model="number" />
          </form>
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
