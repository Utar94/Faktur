<script setup lang="ts">
import { TarInput, type InputOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { CountrySettings } from "@/types/stores";

const { t } = useI18n();

const props = withDefaults(
  defineProps<
    InputOptions & {
      country?: CountrySettings;
    }
  >(),
  {
    floating: true,
    id: "postal-code",
    label: "users.address.postalCode",
    max: 255,
    placeholder: "users.address.postalCode",
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarInput
    v-bind="props"
    :label="t(label)"
    :pattern="country?.postalCode ?? pattern"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
