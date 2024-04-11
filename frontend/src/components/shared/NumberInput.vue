<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    floating?: boolean;
    id?: string;
    label?: string;
    maxLength?: number;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    floating: true,
    id: "number",
    label: "number",
    maxLength: 16,
    placeholder: "number",
    required: false,
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarInput
    :floating="floating"
    :id="id"
    :label="t(label)"
    :max="maxLength"
    :model-value="modelValue"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
