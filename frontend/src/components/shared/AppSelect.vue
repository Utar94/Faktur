<script setup lang="ts">
import { TarSelect, type SelectOptions, type SelectStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { useField } from "vee-validate";
import { useI18n } from "vue-i18n";

import type { ValidationListeners, ValidationRules } from "@/types/validation";

const { t } = useI18n();

const props = withDefaults(
  defineProps<
    SelectOptions & {
      rules?: ValidationRules;
    }
  >(),
  {
    id: () => nanoid(),
  },
);

const selectRef = ref<InstanceType<typeof TarSelect> | null>(null);

const describedBy = computed<string>(() => `${props.id}_invalid-feedback`);
const inputName = computed<string>(() => props.name ?? props.id);

const validationRules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};
  if (props.required) {
    rules.required = true;
  }

  return { ...rules, ...props.rules };
});
const displayLabel = computed<string>(() => (props.label ? t(props.label).toLowerCase() : inputName.value));
const { errorMessage, handleChange, meta, value } = useField<string>(inputName, validationRules, {
  initialValue: props.modelValue,
  label: displayLabel,
});
const status = computed<SelectStatus | undefined>(() => {
  if (!meta.dirty && !meta.touched) {
    return undefined;
  }
  return meta.valid ? "valid" : "invalid";
});
const validationListeners = computed<ValidationListeners>(() => ({
  blur: handleChange,
  change: handleChange,
  input: errorMessage.value ? handleChange : (e: unknown) => handleChange(e, false),
}));

function focus(): void {
  selectRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarSelect
    :aria-label="ariaLabel ? t(ariaLabel) : undefined"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="value"
    :multiple="multiple"
    :name="name"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    ref="selectRef"
    :required="required ? 'label' : undefined"
    :size="size"
    :status="status"
    v-on="validationListeners"
  >
    <template #after>
      <div v-if="errorMessage" class="invalid-feedback" :id="describedBy">{{ errorMessage }}</div>
      <slot name="after"></slot>
    </template>
  </TarSelect>
</template>
