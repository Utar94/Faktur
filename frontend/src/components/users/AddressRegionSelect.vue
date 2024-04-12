<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { CountrySettings } from "@/types/stores";
import { orderBy } from "@/helpers/arrayUtils";

const { t } = useI18n();

const props = withDefaults(
  defineProps<
    SelectOptions & {
      country?: CountrySettings;
    }
  >(),
  {
    ariaLabel: "users.address.region.ariaLabel",
    floating: true,
    id: "address-region",
    label: "users.address.region.label",
    placeholder: "users.address.region.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(props.country?.regions.map((region) => ({ text: t(`countries.${props.country?.code}.regions.${region}`), value: region })) ?? [], "text"),
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarSelect
    v-bind="props"
    :aria-label="t(ariaLabel)"
    :label="t(label)"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
