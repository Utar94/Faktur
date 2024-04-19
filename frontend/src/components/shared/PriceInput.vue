<script setup lang="ts">
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "./AppInput.vue";

const { parseNumber } = parsingUtils;

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: number;
    placeholder?: string;
  }>(),
  {
    id: "price",
    label: "price",
    placeholder: "price",
  },
);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="id"
    :label="label"
    min="0"
    :model-value="modelValue?.toString()"
    :placeholder="placeholder"
    step="0.01"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  >
    <template #append>
      <span class="input-group-text">
        <font-awesome-icon icon="fas fa-dollar-sign" />
      </span>
    </template>
  </AppInput>
</template>
