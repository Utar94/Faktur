<script setup lang="ts">
import DateTimeInput from "@/components/shared/DateTimeInput.vue";

defineProps<{
  modelValue?: Date;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: Date): void;
}>();
</script>

<template>
  <DateTimeInput
    floating
    id="issued-on"
    label="receipts.issuedOn.label"
    :max="new Date()"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
