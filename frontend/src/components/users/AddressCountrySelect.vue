<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import countries from "@/resources/countries.json";
import type { CountrySettings } from "@/types/stores";
import { orderBy } from "@/helpers/arrayUtils";

const { t } = useI18n();

const props = withDefaults(defineProps<SelectOptions>(), {
  ariaLabel: "users.address.country.ariaLabel",
  floating: true,
  id: "address-country",
  label: "users.address.country.label",
  placeholder: "users.address.country.placeholder",
});

const options = computed<SelectOption[]>(() =>
  orderBy(
    countries.map(({ code }) => ({ text: t(`countries.${code}.name`), value: code })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "selected", value?: CountrySettings): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(code?: string): void {
  emit("update:model-value", code);
  if (code) {
    const country = countries.find((country) => country.code === code);
    emit("selected", country);
  } else {
    emit("selected");
  }
}
</script>

<template>
  <TarSelect
    v-bind="props"
    :aria-label="t(ariaLabel)"
    :label="t(label)"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>
