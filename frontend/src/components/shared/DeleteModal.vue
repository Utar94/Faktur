<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    close?: string;
    confirm: string;
    displayName: string;
    id: string;
    loading?: boolean;
    title: string;
  }>(),
  {
    close: "actions.close",
    loading: false,
  },
);

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

defineEmits<{
  (e: "ok", hide: () => void): void;
}>();
</script>

<template>
  <TarModal :close="t(close)" fade :id="id" ref="modalRef" :title="t(title)">
    <p>
      {{ t(confirm) }}
      <br />
      <span class="text-danger">{{ displayName }}</span>
    </p>
    <slot></slot>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="hide" />
      <TarButton
        :disabled="loading"
        icon="fas fa-trash"
        :loading="loading"
        :status="t('loading')"
        :text="t('actions.delete')"
        variant="danger"
        @click="$emit('ok', hide)"
      />
    </template>
  </TarModal>
</template>
