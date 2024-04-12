<script setup lang="ts">
import { TarSelect, type SelectOption, type SelectOptions } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Banner } from "@/types/banners";
import { handleErrorKey } from "@/inject/App";
import { searchBanners } from "@/api/banners";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const banners = ref<Banner[]>([]);

const options = computed<SelectOption[]>(() => banners.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

const props = withDefaults(defineProps<SelectOptions>(), {
  ariaLabel: "banners.select.ariaLabel",
  floating: true,
  id: "banner",
  label: "banners.select.label",
  placeholder: "banners.select.placeholder",
});

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
    });
    banners.value = results.items;
  } catch (e: unknown) {
    handleError(e);
  }
});

const emit = defineEmits<{
  (e: "selected", value?: Banner): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id?: string): void {
  emit("update:model-value", id);
  if (id) {
    const banner = banners.value.find((banner) => banner.id === id);
    emit("selected", banner);
  } else {
    emit("selected");
  }
}
</script>

<template>
  <TarSelect
    v-bind="props"
    :aria-label="t(ariaLabel)"
    :label="t(label)"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>
