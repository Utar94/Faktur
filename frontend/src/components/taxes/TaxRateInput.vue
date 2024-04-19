<script setup lang="ts">
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="rate"
    label="taxes.rate"
    min="0.00001"
    max="1"
    :model-value="modelValue?.toString()"
    placeholder="taxes.rate"
    :required="required"
    step="0.00001"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  />
</template>
