<script setup lang="ts">
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
  required?: boolean | string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
function onModelValueUpdate(value: any): void {
  const number: number | undefined = parseNumber(value);
  emit("update:model-value", number ? number / 100 : undefined);
}
</script>

<template>
  <AppInput
    floating
    id="rate"
    label="taxes.rate"
    min="0.001"
    max="99.999"
    :model-value="((modelValue ?? 0) * 100).toString()"
    placeholder="taxes.rate"
    :required="required"
    step="0.001"
    type="number"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <span class="input-group-text">
        <font-awesome-icon icon="fas fa-percent" />
      </span>
    </template>
  </AppInput>
</template>
