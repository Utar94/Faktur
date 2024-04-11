<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import FlagsInput from "@/components/shared/FlagsInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TaxCodeInput from "@/components/taxes/TaxCodeInput.vue";
import TaxRateInput from "@/components/taxes/TaxRateInput.vue";
import type { ApiError } from "@/types/api";
import type { Tax } from "@/types/taxes";
import { createTax, readTax, replaceTax } from "@/api/taxes";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const code = ref<string>("");
const flags = ref<string>("");
const hasLoaded = ref<boolean>(false);
const rate = ref<number>(0);
const tax = ref<Tax>();

const hasChanges = computed<boolean>(() => {
  return code.value !== (tax.value?.code ?? "") || rate.value !== (tax.value?.rate ?? 0) || flags.value !== (tax.value?.flags ?? "");
});

function setModel(model: Tax): void {
  tax.value = model;
  code.value = model.code;
  flags.value = model.flags ?? "";
  rate.value = model.rate;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (tax.value) {
      const updatedTax = await replaceTax(
        tax.value.id,
        {
          code: code.value,
          rate: rate.value,
          flags: flags.value,
        },
        tax.value.version,
      );
      setModel(updatedTax);
      toasts.success("taxes.updated");
    } else {
      const createdTax = await createTax({
        code: code.value,
        rate: rate.value,
        flags: flags.value,
      });
      setModel(createdTax);
      toasts.success("taxes.created");
      router.replace({ name: "TaxEdit", params: { id: createdTax.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const tax = await readTax(id);
      setModel(tax);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="hasLoaded">
      <h1>{{ tax?.code ?? t("taxes.title.new") }}</h1>
      <StatusDetail v-if="tax" :aggregate="tax" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <TarButton
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="tax ? 'fas fa-save' : 'fas fa-plus'"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t(tax ? 'actions.save' : 'actions.create')"
            type="submit"
            :variant="tax ? 'primary' : 'success'"
          />
          <TarButton class="ms-1" icon="fas fa-chevron-left" :text="t('actions.back')" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <TaxCodeInput required v-model="code" />
        <TaxRateInput required :model-value="rate.toString()" @update:model-value="rate = $event ?? 0" />
        <FlagsInput v-model="flags" />
      </form>
    </template>
  </main>
</template>
