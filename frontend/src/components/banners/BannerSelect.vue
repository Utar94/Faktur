<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Banner } from "@/types/banners";
import { handleErrorKey } from "@/inject/App";
import { searchBanners } from "@/api/banners";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const banners = ref<Banner[]>([]);

const options = computed<SelectOption[]>(() => banners.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

withDefaults(
  defineProps<{
    ariaLabel?: string;
    floating?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    ariaLabel: "banners.select.ariaLabel",
    floating: true,
    id: "banner",
    label: "banners.select.label",
    placeholder: "banners.select.placeholder",
    required: false,
  },
);

onMounted(async () => {
  try {
    const results = await searchBanners({
      ids: [],
      search: {
        terms: [],
        operator: "And",
      },
      sort: [
        {
          field: "DisplayName",
          isDescending: false,
        },
      ],
      skip: 0,
      limit: 0,
    }); // TODO(fpion): refactor
    banners.value = results.items;
  } catch (e: unknown) {
    handleError(e);
  }
});

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <TarSelect
    :aria-label="t(ariaLabel)"
    :floating="floating"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
