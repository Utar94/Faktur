<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { nanoid } from "nanoid";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    category?: string;
    id?: string;
  }>(),
  {
    id: () => nanoid(),
  },
);

const displayName = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const hasChanges = computed<boolean>(() => displayName.value.trim() !== (props.category?.trim() ?? ""));
const modalId = computed<string>(() => `edit-category_${props.id}`);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "saved", category: string): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  emit("saved", displayName.value);
  hide();
});

watchEffect(() => (displayName.value = props.category ?? ""));
</script>

<template>
  <span>
    <TarButton
      :icon="category ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(category ? 'actions.edit' : 'actions.create')"
      :variant="category ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${modalId}`"
    />
    <TarModal
      :close="t('actions.close')"
      fade
      :id="modalId"
      ref="modalRef"
      :title="t(category ? 'receipts.categories.title.edit' : 'receipts.categories.title.new')"
    >
      <form @submit.prevent="onSubmit">
        <DisplayNameInput required v-model="displayName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="hide" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="category ? 'fas fa-save' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(category ? 'actions.save' : 'actions.create')"
          :variant="category ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
