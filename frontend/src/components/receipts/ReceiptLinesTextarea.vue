<script setup lang="ts">
import AppTextarea from "@/components/shared/AppTextarea.vue";

defineProps<{
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppTextarea
    floating
    id="receipt-lines"
    label="receipts.lines"
    :model-value="modelValue"
    placeholder="receipts.lines"
    rows="15"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
