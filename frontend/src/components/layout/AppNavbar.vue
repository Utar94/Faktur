<script setup lang="ts">
import { RouterLink } from "vue-router";
import { TarAvatar, parsingUtils } from "logitar-vue3-ui";
import { computed, watchEffect } from "vue";
import { setLocale } from "@vee-validate/i18n";
import { useI18n } from "vue-i18n";

const { parseBoolean } = parsingUtils;
import locales from "@/resources/locales.json";
import type { CurrentUser } from "@/types/account";
import type { Hyperlink } from "@/types/components";
import type { Locale } from "@/types/i18n";
import { combineURL } from "@/helpers/stringUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { useAccountStore } from "@/stores/account";
import { useI18nStore } from "@/stores/i18n";

const account = useAccountStore();
const apiBaseUrl: string = import.meta.env.VITE_APP_API_BASE_URL ?? "";
const environment = import.meta.env.MODE.toLowerCase();
const i18n = useI18nStore();
const { availableLocales, locale, t } = useI18n();
const isGraphQLEnabled: boolean = parseBoolean(import.meta.env.VITE_APP_ENABLE_GRAPHQL) ?? false;
const isOpenApiEnabled: boolean = parseBoolean(import.meta.env.VITE_APP_ENABLE_OPENAPI) ?? false;

const otherLocales = computed<Locale[]>(() => {
  const otherLocales = new Set<string>(availableLocales.filter((item) => item !== locale.value));
  return orderBy(
    locales.filter(({ code }) => otherLocales.has(code)),
    "nativeName",
  );
});
const swaggerUrl = computed<string | undefined>(() => (isOpenApiEnabled ? combineURL(apiBaseUrl, "/swagger") : undefined));
const graphQLLinks = computed<Hyperlink[]>(() =>
  isGraphQLEnabled
    ? [
        { text: "Altair", url: combineURL(apiBaseUrl, "/ui/altair") },
        { text: "GraphiQL", url: combineURL(apiBaseUrl, "/ui/graphiql") },
        { text: "Playground", url: combineURL(apiBaseUrl, "/ui/playground") },
        { text: "Voyager", url: combineURL(apiBaseUrl, "/ui/voyager") },
      ]
    : [],
);
const user = computed<CurrentUser | undefined>(() => account.currentUser);

watchEffect(() => {
  if (i18n.locale) {
    locale.value = i18n.locale.code;
    setLocale(i18n.locale.code);
  } else {
    const currentLocale = locales.find(({ code }) => code === locale.value);
    if (!currentLocale) {
      throw new Error(`The locale "${locale.value}" is not supported.'`);
    }
    i18n.setLocale(currentLocale);
  }
});
</script>

<template>
  <nav class="navbar navbar-expand-lg bg-body-tertiary" data-bs-theme="dark">
    <div class="container-fluid">
      <RouterLink :to="{ name: 'Home' }" class="navbar-brand">
        <img src="@/assets/img/logo.png" :alt="`${t('brand')} Logo`" height="32" />
        {{ t("brand") }}
        <span v-if="environment !== 'production'" class="badge text-bg-warning">{{ environment }}</span>
      </RouterLink>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li v-if="swaggerUrl" class="nav-item">
            <a class="nav-link" :href="swaggerUrl" target="_blank"> <font-awesome-icon icon="fas fa-vial" /> Swagger</a>
          </li>
          <li v-if="graphQLLinks.length" class="nav-item dropdown d-none d-lg-block">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
              <img src="@/assets/img/graphql.png" alt="GraphQL Logo" height="16" /> GraphQL
            </a>
            <ul class="dropdown-menu dropdown-menu-end">
              <li v-for="(link, index) in graphQLLinks" :key="index">
                <a class="dropdown-item" :href="link.url" target="_blank">{{ link.text }}</a>
              </li>
            </ul>
          </li>
          <template v-if="user">
            <li class="nav-item">
              <RouterLink :to="{ name: 'ArticleList' }" class="nav-link"><font-awesome-icon icon="fas fa-carrot" /> {{ t("articles.title.list") }}</RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'BannerList' }" class="nav-link"><font-awesome-icon icon="fas fa-flag" /> {{ t("banners.title.list") }}</RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'StoreList' }" class="nav-link"><font-awesome-icon icon="fas fa-store" /> {{ t("stores.title.list") }}</RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'ProductList' }" class="nav-link">
                <font-awesome-icon icon="fas fa-shopping-cart" /> {{ t("products.title.list") }}
              </RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'ReceiptList' }" class="nav-link">
                <font-awesome-icon icon="fas fa-file-invoice-dollar" /> {{ t("receipts.title.list") }}
              </RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'TaxList' }" class="nav-link"><font-awesome-icon icon="fas fa-sack-dollar" /> {{ t("taxes.title.list") }}</RouterLink>
            </li>
          </template>
        </ul>

        <ul class="navbar-nav mb-2 mb-lg-0">
          <template v-if="i18n.locale">
            <li v-if="otherLocales.length > 1" class="nav-item dropdown">
              <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">{{ i18n.locale.nativeName }}</a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li v-for="option in otherLocales" :key="option.code">
                  <a class="dropdown-item" href="#" @click.prevent="i18n.setLocale(option)">{{ option.nativeName }}</a>
                </li>
              </ul>
            </li>
            <li v-else-if="otherLocales.length === 1" class="nav-item">
              <a class="nav-link" href="#" @click.prevent="i18n.setLocale(otherLocales[0])">{{ otherLocales[0].nativeName }}</a>
            </li>
          </template>
          <template v-if="user">
            <li class="nav-item d-block d-lg-none">
              <RouterLink class="nav-link" :to="{ name: 'Profile' }">
                <TarAvatar :display-name="user.displayName" :email-address="user.emailAddress" :size="24" :url="user.pictureUrl" />
                {{ user.displayName }}
              </RouterLink>
            </li>
            <li class="nav-item d-block d-lg-none">
              <RouterLink class="nav-link" :to="{ name: 'SignOut' }">
                <font-awesome-icon icon="fas fa-arrow-right-from-bracket" /> {{ t("users.signOut") }}
              </RouterLink>
            </li>
            <li class="nav-item dropdown d-none d-lg-block">
              <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <TarAvatar :display-name="user.displayName" :email-address="user.emailAddress" :size="24" :url="user.pictureUrl" />
              </a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <RouterLink class="dropdown-item" :to="{ name: 'Profile' }"><font-awesome-icon icon="fas fa-user" /> {{ user.displayName }}</RouterLink>
                </li>
                <li>
                  <RouterLink class="dropdown-item" :to="{ name: 'SignOut' }">
                    <font-awesome-icon icon="fas fa-arrow-right-from-bracket" /> {{ t("users.signOut") }}
                  </RouterLink>
                </li>
              </ul>
            </li>
          </template>
          <template v-else>
            <li class="nav-item">
              <RouterLink :to="{ name: 'SignIn' }" class="nav-link">
                <font-awesome-icon icon="fas fa-arrow-right-to-bracket" /> {{ t("users.signIn.title") }}
              </RouterLink>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>
