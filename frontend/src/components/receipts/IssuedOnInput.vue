<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";
import { toDateTimeLocal } from "@/helpers/dateUtils";

defineProps<{
  modelValue?: Date;
  required?: boolean | string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value?: Date): void;
}>();
function onModelValueUpdate(value: string): void {
  try {
    const date = new Date(value);
    emit("update:model-value", isNaN(date.getTime()) ? undefined : date);
  } catch (e: unknown) {
    emit("update:model-value", undefined);
  }
}

// TODO(fpion): create a DateTimeInput
</script>

<template>
  <AppInput
    floating
    id="issued-on"
    label="receipts.issuedOn.label"
    :max="toDateTimeLocal(new Date())"
    :model-value="modelValue ? toDateTimeLocal(modelValue) : undefined"
    placeholder="receipts.issuedOn.label"
    :required="required"
    type="datetime-local"
    @update:model-value="onModelValueUpdate"
  />
</template>
