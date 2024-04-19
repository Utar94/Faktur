<script setup lang="ts">
import { parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import { orderBy } from "@/helpers/arrayUtils";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

defineProps<{
  modelValue?: boolean;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("receipts.status.options.processed"),
        value: "true",
      },
      {
        text: t("receipts.status.options.new"),
        value: "false",
      },
    ],
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: boolean): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="status"
    label="receipts.status.label"
    :model-value="modelValue?.toString()"
    :options="options"
    placeholder="receipts.status.placeholder"
    @update:model-value="$emit('update:model-value', parseBoolean($event))"
  />
</template>
