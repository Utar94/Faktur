<script setup lang="ts">
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="quantity"
    label="receipts.quantity"
    min="0"
    :model-value="modelValue?.toString()"
    placeholder="receipts.quantity"
    step="0.001"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  />
</template>
