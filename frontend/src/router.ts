import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";

import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
      meta: { isPublic: true },
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      // route level code-splitting
      // this generates a separate chunk (ProfileView.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/account/ProfileView.vue"),
    },
    {
      name: "SignIn",
      path: "/sign-in",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/sign-out",
      component: () => import("./views/account/SignOutView.vue"),
    },
    // Articles
    {
      name: "ArticleList",
      path: "/articles",
      component: () => import("./views/articles/ArticleList.vue"),
    },
    {
      name: "ArticleEdit",
      path: "/articles/:id",
      component: () => import("./views/articles/ArticleEdit.vue"),
    },
    {
      name: "CreateArticle",
      path: "/create-article",
      component: () => import("./views/articles/ArticleEdit.vue"),
    },
    // Banners
    {
      name: "BannerList",
      path: "/banners",
      component: () => import("./views/banners/BannerList.vue"),
    },
    {
      name: "BannerEdit",
      path: "/banners/:id",
      component: () => import("./views/banners/BannerEdit.vue"),
    },
    {
      name: "CreateBanner",
      path: "/create-banner",
      component: () => import("./views/banners/BannerEdit.vue"),
    },
    // Products
    {
      name: "ProductList",
      path: "/products",
      component: () => import("./views/products/ProductList.vue"),
    },
    {
      name: "ProductEdit",
      path: "/products/:id",
      component: () => import("./views/products/ProductEdit.vue"),
    },
    {
      name: "CreateProduct",
      path: "/create-product",
      component: () => import("./views/products/ProductEdit.vue"),
    },
    // Receipts
    {
      name: "ReceiptList",
      path: "/receipts",
      component: () => import("./views/receipts/ReceiptListView.vue"),
    },
    {
      name: "ReceiptEdit",
      path: "/receipts/:id",
      component: () => import("./views/receipts/ReceiptEdit.vue"),
    },
    {
      name: "ImportReceipt",
      path: "/import-receipt",
      component: () => import("./views/receipts/ImportReceipt.vue"),
    },
    // Stores
    {
      name: "StoreList",
      path: "/stores",
      component: () => import("./views/stores/StoreList.vue"),
    },
    {
      name: "StoreEdit",
      path: "/stores/:id",
      component: () => import("./views/stores/StoreEdit.vue"),
    },
    {
      name: "CreateStore",
      path: "/create-store",
      component: () => import("./views/stores/StoreEdit.vue"),
    },
    // Taxes
    {
      name: "TaxList",
      path: "/taxes",
      component: () => import("./views/taxes/TaxList.vue"),
    },
    {
      name: "TaxEdit",
      path: "/taxes/:id",
      component: () => import("./views/taxes/TaxEdit.vue"),
    },
    {
      name: "CreateTax",
      path: "/create-tax",
      component: () => import("./views/taxes/TaxEdit.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      component: () => import("./views/NotFound.vue"),
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
