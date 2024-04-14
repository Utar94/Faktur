<script setup lang="ts">
import { orderBy } from "@/helpers/arrayUtils";
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { rt, t, tm } = useI18n();

const props = withDefaults(defineProps<SelectOptions>(), {
  floating: true,
  id: "unit-type",
  label: "products.unitType.label",
  placeholder: "products.unitType.placeholder",
});

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("products.unitType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarSelect v-bind="props" :label="t(label)" :options="options" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
