<script setup lang="ts">
import { TarTextarea } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    floating?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
    rows?: number;
  }>(),
  {
    floating: true,
    id: "description",
    label: "description",
    placeholder: "description",
    required: false,
    rows: 15,
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarTextarea
    :floating="floating"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :placeholder="t(placeholder)"
    :required="required"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
