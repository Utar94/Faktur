<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FlagsInput from "@/components/shared/FlagsInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import PriceInput from "@/components/shared/PriceInput.vue";
import QuantityInput from "./QuantityInput.vue";
import SkuInput from "@/components/products/SkuInput.vue";
import type { DepartmentSummary, Receipt, ReceiptItem } from "@/types/receipts";
import { createOrReplaceReceiptItem } from "@/api/receipts";

const { t } = useI18n();

const department = ref<DepartmentSummary>({ number: "", displayName: "" });
const flags = ref<string>("");
const gtinOrSku = ref<string>("");
const item = ref<ReceiptItem>();
const label = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const price = ref<number>(0);
const quantity = ref<number>(0);
const receipt = ref<Receipt>();
const unitPrice = ref<number>(0);

const hasChanges = computed<boolean>(() =>
  item.value
    ? gtinOrSku.value !== (item.value.gtin ?? item.value.sku ?? "") ||
      label.value !== item.value.label ||
      flags.value !== (item.value.flags ?? "") ||
      quantity.value !== item.value.quantity ||
      unitPrice.value !== item.value.unitPrice ||
      price.value !== item.value.price ||
      department.value.number !== (item.value.department?.number ?? "") ||
      department.value.displayName !== (item.value.department?.displayName ?? "")
    : false,
);
const isDepartmentRequired = computed<boolean>(() => Boolean(department.value.number || department.value.displayName));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: ReceiptItem): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (item.value && receipt.value) {
    try {
      const receiptItem = await createOrReplaceReceiptItem(
        receipt.value.id,
        item.value.number,
        {
          gtinOrSku: gtinOrSku.value,
          label: label.value,
          flags: flags.value,
          quantity: quantity.value || undefined,
          unitPrice: unitPrice.value || undefined,
          price: price.value,
          department: isDepartmentRequired.value ? department.value : undefined,
        },
        receipt.value.version,
      );
      emit("saved", receiptItem);
      hide();
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});

function edit(receiptItem: ReceiptItem, parentReceipt: Receipt): void {
  item.value = receiptItem;
  receipt.value = parentReceipt;
  department.value = {
    number: receiptItem.department?.number ?? "",
    displayName: receiptItem.department?.displayName ?? "",
  };
  flags.value = receiptItem.flags ?? "";
  gtinOrSku.value = receiptItem.gtin ?? receiptItem.sku ?? "";
  label.value = receiptItem.label;
  price.value = receiptItem.price;
  quantity.value = receiptItem.quantity;
  unitPrice.value = receiptItem.unitPrice;
  modalRef.value?.show();
}
defineExpose({ edit });
</script>

<template>
  <TarModal :close="t('actions.close')" ref="modalRef" :title="t('receipts.items.edit')">
    <form @submit.prevent="onSubmit">
      <SkuInput id="gtin-or-sku" label="receipts.items.gtinOrSku" required v-model="gtinOrSku" />
      <DisplayNameInput id="label" label="receipts.label" required v-model="label" />
      <FlagsInput v-model="flags" />
      <QuantityInput v-model="quantity" />
      <PriceInput id="unit-price" label="products.unitPrice" v-model="unitPrice" />
      <PriceInput min="0.01" required v-model="price" />
      <h5>{{ t("departments.select.label") }}</h5>
      <NumberInput :required="isDepartmentRequired" v-model="department.number" />
      <DisplayNameInput :required="isDepartmentRequired" v-model="department.displayName" />
    </form>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="hide" />
      <TarButton
        :disabled="isSubmitting || !hasChanges"
        :icon="department ? 'fas fa-save' : 'fas fa-plus'"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t(department ? 'actions.save' : 'actions.create')"
        :variant="department ? 'primary' : 'success'"
        @click="onSubmit"
      />
    </template>
  </TarModal>
</template>
