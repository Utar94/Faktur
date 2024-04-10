<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Actor } from "@/types/actor";

const { d, t } = useI18n();

const props = defineProps<{
  actor: Actor;
  date: string;
}>();

const displayName = computed<string>(() => {
  const { displayName, type } = props.actor;
  return type === "System" ? t("system") : displayName;
});
const href = computed<string | undefined>(() => {
  const { id, isDeleted, type } = props.actor;
  if (!isDeleted) {
    switch (type) {
      case "ApiKey":
        return `/api-keys/${id}`;
      case "User":
        return `/users/${id}`;
    } // TODO(fpion): expose Portal URL?
  }
  return undefined;
});
const icon = computed<string | undefined>(() => {
  switch (props.actor.type) {
    case "ApiKey":
      return "fas fa-key";
    case "System":
      return "fas fa-robot";
    case "User":
      return "fas fa-user";
  }
  return undefined;
});
const variant = computed<string | undefined>(() => (props.actor.type === "ApiKey" ? "info" : undefined));
</script>

<template>
  <div class="d-flex">
    <div class="d-flex">
      <div class="d-flex align-content-center flex-wrap mx-1">
        <a v-if="href" :href="href" target="_blank">
          <TarAvatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :url="actor.pictureUrl" :variant="variant" />
        </a>
        <TarAvatar v-else :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :url="actor.pictureUrl" :variant="variant" />
      </div>
    </div>
    <div>
      {{ d(date, "medium") }}
      <br />
      {{ t("by") }}
      <a v-if="href" :href="href" target="_blank">{{ displayName }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" /></a>
      <template v-else>{{ displayName }}</template>
    </div>
  </div>
</template>
