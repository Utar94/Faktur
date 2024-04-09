<script setup lang="ts">
import { onMounted } from "vue";
import { useRouter } from "vue-router";

import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const router = useRouter();

onMounted(() => {
  router.push({ name: account.currentUser ? "Profile" : "SignIn" });
});
</script>

<template>
  <div></div>
</template>
