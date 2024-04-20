<script setup lang="ts">
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "./AppInput.vue";

const { parseNumber } = parsingUtils;

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    min?: number | string;
    modelValue?: number;
    required?: boolean | string;
  }>(),
  {
    id: "price",
    label: "price",
    min: 0,
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
    :min="min"
    :model-value="modelValue?.toString()"
    :placeholder="label"
    :required="required"
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
