<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import PersonalInformation from "@/components/users/PersonalInformation.vue";
import type { UserProfile } from "@/types/account";
import { getProfile } from "@/api/account";
import { handleErrorKey } from "@/inject/App";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { t } = useI18n();

const user = ref<UserProfile>();

function onSaved(profile: UserProfile) {
  user.value = profile;
  toasts.success("users.profile.saved");
}

onMounted(async () => {
  try {
    user.value = await getProfile();
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="user">
      <h1>{{ t("users.profile.title") }}</h1>
      <!-- TODO(fpion): header -->
      <TarTabs>
        <TarTab active id="personal" :title="t('users.tabs.personal')">
          <PersonalInformation :user="user" @error="handleError" @saved="onSaved" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
