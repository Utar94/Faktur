<script setup lang="ts">
import { TarButton, TarTab, TarTabs, parsingUtils } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import IssuedOnInput from "@/components/receipts/IssuedOnInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import ReceiptLinesTextarea from "@/components/receipts/ReceiptLinesTextarea.vue";
import ReceiptList from "@/components/receipts/ReceiptList.vue";
import StoreSelect from "@/components/stores/StoreSelect.vue";
import type { ApiError, PropertyError, ValidationError } from "@/types/api";
import type { Receipt } from "@/types/receipts";
import type { Store } from "@/types/stores";
import { handleErrorKey } from "@/inject/App";
import { importReceipt } from "@/api/receipts";
import { readStore } from "@/api/stores";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseNumber } = parsingUtils;
const { t } = useI18n();

const errors = ref<PropertyError[]>([]);
const issuedOn = ref<Date>();
const lines = ref<string>("");
const number = ref<string>("");
const receipts = ref<Receipt[]>([]);
const store = ref<Store>();

const hasChanges = computed<boolean>(() => store.value !== undefined || Boolean(issuedOn.value) || number.value !== "" || lines.value !== "");

function parseLineNumber(error: PropertyError): number {
  if (error.propertyName) {
    const startIndex = error.propertyName.indexOf("[");
    if (startIndex >= 0) {
      const endIndex = error.propertyName.indexOf("]", startIndex);
      if (endIndex >= 0) {
        const number = parseNumber(error.propertyName.substring(startIndex + 1, endIndex));
        if (typeof number === "number") {
          return number + 1;
        }
      }
    }
  }
  return 0;
}

function translatePropertyName(error: PropertyError): string {
  if (error.propertyName) {
    const propertyName = error.propertyName.substring(error.propertyName.indexOf(".") + 1);
    switch (propertyName) {
      case "DepartmentName":
        return t("receipts.error.propertyNames.DepartmentName");
      case "DepartmentNumber":
        return t("receipts.error.propertyNames.DepartmentNumber");
      case "Flags":
        return t("flags");
      case "Gtin":
        return t("articles.gtin.label");
      case "Label":
        return t("receipts.label");
      case "Price":
        return t("price");
      case "Quantity":
        return t("receipts.quantity");
      case "Sku":
        return t("products.sku.label");
      case "UnitPrice":
        return t("products.unitPrice");
    }
  }
  return "—";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (store.value) {
    errors.value = [];
    try {
      const receipt = await importReceipt({
        storeId: store.value.id,
        issuedOn: issuedOn.value,
        number: number.value,
        lines: lines.value,
      });
      receipts.value.push(receipt);
      issuedOn.value = undefined;
      number.value = "";
      lines.value = "";
      toasts.success("receipts.imported");
    } catch (e: unknown) {
      const { data, status } = e as ApiError;
      const validation = data as ValidationError;
      if (status === 400 && validation?.code === "Validation") {
        errors.value = validation.errors;
        toasts.warning("receipts.error.validation");
      } else {
        handleError(e);
      }
    }
  }
});

onMounted(async () => {
  try {
    const storeId = route.query.storeId?.toString();
    if (storeId) {
      store.value = await readStore(storeId);
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
    <h1>{{ t("receipts.title.import") }}</h1>
    <TarTabs>
      <TarTab active id="importing" :title="t('receipts.title.importing')">
        <form @submit.prevent="onSubmit">
          <div class="mb-3">
            <TarButton
              class="me-1"
              :disabled="isSubmitting || !hasChanges"
              icon="fas fa-file-invoice-dollar"
              :loading="isSubmitting"
              :status="t('loading')"
              :text="t('actions.import')"
              type="submit"
              variant="success"
            />
            <AppBackButton class="ms-1" :has-changes="hasChanges" />
          </div>
          <StoreSelect :model-value="store?.id" required @selected="store = $event" />
          <IssuedOnInput required v-model="issuedOn" />
          <NumberInput v-model="number" />
          <ReceiptLinesTextarea v-model="lines" />
        </form>
        <h3>{{ t("receipts.error.title") }}</h3>
        <p v-if="errors.length === 0">{{ t("receipts.empty") }}</p>
        <table v-else class="table table-striped">
          <thead>
            <tr>
              <th scope="col">{{ t("receipts.error.lineNumber") }}</th>
              <th scope="col">{{ t("receipts.error.propertyName") }}</th>
              <th scope="col">{{ t("receipts.error.attemptedValue") }}</th>
              <th scope="col">{{ t("receipts.error.code") }}</th>
              <th scope="col">{{ t("receipts.error.message") }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(error, index) in errors" :key="index">
              <td>{{ parseLineNumber(error) }}</td>
              <td>{{ translatePropertyName(error) }}</td>
              <td>{{ error.attemptedValue }}</td>
              <td>{{ error.code }}</td>
              <td>{{ error.message }}</td>
            </tr>
          </tbody>
        </table>
      </TarTab>
      <TarTab :disabled="receipts.length === 0" id="imported" :title="t('receipts.title.imported')">
        <ReceiptList :receipts="receipts" />
      </TarTab>
    </TarTabs>
  </main>
</template>
