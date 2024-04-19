<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Banner } from "@/types/banners";
import { searchBanners } from "@/api/banners";

defineProps<{
  modelValue?: string;
  noStatus?: boolean | string;
}>();

const banners = ref<Banner[]>([]);

const options = computed<SelectOption[]>(() => banners.value.map(({ id, displayName }) => ({ value: id, text: displayName })));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
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
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    floating
    id="banner"
    label="banners.select.label"
    :model-value="modelValue"
    :no-status="noStatus"
    :options="options"
    placeholder="banners.select.placeholder"
    @update:model-value="onModelValueUpdate"
  />
</template>
