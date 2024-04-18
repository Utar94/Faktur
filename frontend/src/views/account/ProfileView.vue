<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import { handleErrorKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const emailAddress = ref<string>("");
const password = ref<string>("");

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    alert(`Hello ${emailAddress.value}!`);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <h1>Profile</h1>
    <form @submit.prevent="onSubmit">
      <AppInput floating label="users.email.address" placeholder="users.email.address" required type="email" v-model="emailAddress" />
      <AppInput floating label="users.password" placeholder="users.password" required type="password" v-model="password" />
      <TarButton :disabled="isSubmitting" icon="fas fa-user" :loading="isSubmitting" :status="t('loading')" :text="t('actions.save')" type="submit" />
    </form>
  </main>
</template>
