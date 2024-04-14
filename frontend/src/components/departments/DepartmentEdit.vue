<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import type { Department } from "@/types/departments";
import type { Store } from "@/types/stores";
import { createOrReplaceDepartment } from "@/api/departments";

const { t } = useI18n();

const props = defineProps<{
  department?: Department;
  store: Store;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const number = ref<string>("");

const hasChanges = computed<boolean>(
  () =>
    number.value !== (props.department?.number ?? "") ||
    displayName.value !== (props.department?.displayName ?? "") ||
    description.value !== (props.department?.description ?? ""),
);
const id = computed<string>(() => (props.department ? `edit-department_${props.department.number}` : "create-department"));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: Department): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const department = await createOrReplaceDepartment(
      props.store.id,
      props.department?.number ?? number.value,
      {
        displayName: displayName.value,
        description: description.value,
      },
      props.store.version,
    );
    emit("saved", department);
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});

watchEffect(() => {
  const department = props.department;
  number.value = department?.number ?? "";
  displayName.value = department?.displayName ?? "";
  description.value = department?.description ?? "";
});
</script>

<template>
  <span>
    <TarButton
      :icon="department ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(department ? 'actions.edit' : 'actions.create')"
      :variant="department ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" fade :id="id" ref="modalRef" size="large" :title="t(department ? 'departments.title.edit' : 'departments.title.new')">
      <form @submit.prevent="onSubmit">
        <NumberInput required v-model="number" />
        <DisplayNameInput required v-model="displayName" />
        <DescriptionTextarea v-model="description" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="hide" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="department ? 'fas fa-save' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(department ? 'actions.save' : 'actions.create')"
          :variant="department ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
