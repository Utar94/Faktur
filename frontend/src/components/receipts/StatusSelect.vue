<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import { orderBy } from "@/helpers/arrayUtils";

const { t } = useI18n();

const props = withDefaults(defineProps<SelectOptions>(), {
  floating: true,
  id: "status",
  label: "receipts.status.label",
  placeholder: "receipts.status.placeholder",
});

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
</script>

<template>
  <TarSelect v-bind="props" :label="t(label)" :options="options" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
