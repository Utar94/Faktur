<script setup lang="ts">
import { parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { ref } from "vue";

import AppSelect from "./AppSelect.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
}>();

const options = ref<SelectOption[]>([
  { text: "10", value: "10" },
  { text: "25", value: "25" },
  { text: "50", value: "50" },
  { text: "100", value: "100" },
]);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();

// TODO(fpion): always ignore validation
</script>

<template>
  <AppSelect
    floating
    id="count"
    label="count"
    :model-value="modelValue?.toString()"
    :options="options"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  />
</template>
