<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

const router = useRouter();
const { t } = useI18n();

defineProps<{
  hasChanges?: boolean;
}>();
</script>

<template>
  <TarButton icon="fas fa-chevron-left" :text="t('actions.back')" :variant="hasChanges ? 'warning' : 'secondary'" @click="router.back()" />
</template>
