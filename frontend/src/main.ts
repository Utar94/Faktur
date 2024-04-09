import persistedState from "pinia-plugin-persistedstate";
import { createApp } from "vue";
import { createPinia } from "pinia";

import App from "./App.vue";
import fontAwesome from "./fontAwesome";
import i18n from "./i18n";
import router from "./router";

import "bootstrap";
import "bootstrap/dist/css/bootstrap.min.css";

import "./assets/styles/main.css";
// import "./validation"; // TODO(fpion): validation

const app = createApp(App);

const pinia = createPinia();
pinia.use(persistedState);

app.use(fontAwesome);
app.use(i18n);
app.use(pinia);
app.use(router);

app.mount("#app");
