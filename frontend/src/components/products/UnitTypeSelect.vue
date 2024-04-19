<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { UnitType } from "@/types/products";
import { orderBy } from "@/helpers/arrayUtils";

const { rt, tm } = useI18n();

defineProps<{
  modelValue?: UnitType;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("products.unitType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: UnitType): void;
}>();

// TODO(fpion): validation in ProductEdit
</script>

<template>
  <AppSelect
    floating
    id="unit-type"
    label="products.unitType.label"
    :model-value="modelValue"
    :options="options"
    placeholder="products.unitType.placeholder"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
