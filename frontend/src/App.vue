<script setup lang="ts">
import { RouterView, useRoute, useRouter } from "vue-router";
import { TarToaster } from "logitar-vue3-ui";
import { provide } from "vue";

import AppFooter from "./components/layout/AppFooter.vue";
import AppNavbar from "./components/layout/AppNavbar.vue";
import type { ApiError, GraphQLError } from "./types/api";
import { handleErrorKey } from "./inject/App";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const route = useRoute();
const router = useRouter();

function handleError(e: unknown): void {
  if (e) {
    const { data, status } = e as ApiError;
    const errors = data as GraphQLError[];
    if (status === 401 || (typeof errors?.some === "function" && errors.some((error) => error.extensions?.code === "ACCESS_DENIED") === true)) {
      account.signOut();
      // toasts.warning("toasts.warning.signedOut"); // TODO(fpion): toasts
      router.push({ name: "SignIn", query: { redirect: route.fullPath } });
    } else {
      console.error(e);
      // toasts.error(); // TODO(fpion): toasts
    }
  } else {
    // toasts.error(); // TODO(fpion): toasts
  }
}
provide(handleErrorKey, handleError);
</script>

<template>
  <AppNavbar />
  <RouterView />
  <AppFooter />
  <TarToaster />
</template>
