import Vue from 'vue'
import VueRouter from 'vue-router'
import store from './store'
import ArticleEdit from './components/Articles/ArticleEdit.vue'
import ArticleList from './components/Articles/ArticleList.vue'
import BannerEdit from './components/Banners/BannerEdit.vue'
import BannerList from './components/Banners/BannerList.vue'
import Confirm from './components/Identity/Confirm.vue'
import DepartmentEdit from './components/Departments/DepartmentEdit.vue'
import Home from './components/Home.vue'
import NotFound from './components/NotFound.vue'
import ProductEdit from './components/Products/ProductEdit.vue'
import ProductList from './components/Products/ProductList.vue'
import Profile from './components/Identity/Profile.vue'
import RecoverPassword from './components/Identity/RecoverPassword.vue'
import ResetPassword from './components/Identity/ResetPassword.vue'
import SignIn from './components/Identity/SignIn.vue'
import SignUp from './components/Identity/SignUp.vue'
import StoreEdit from './components/Stores/StoreEdit.vue'
import StoreList from './components/Stores/StoreList.vue'
Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  routes: [
    {
      name: 'Home',
      path: '/',
      component: Home,
      meta: { public: true }
    },
    {
      name: 'NotFound',
      path: '/404',
      component: NotFound,
      meta: { public: true }
    },
    {
      name: 'Articles',
      path: '/articles',
      component: ArticleList
    },
    {
      name: 'Article',
      path: '/articles/:id',
      component: ArticleEdit
    },
    {
      name: 'Banners',
      path: '/banners',
      component: BannerList
    },
    {
      name: 'Banner',
      path: '/banners/:id',
      component: BannerEdit
    },
    {
      name: 'Confirm',
      path: '/user/confirm',
      component: Confirm,
      meta: { public: true }
    },
    {
      name: 'Department',
      path: '/stores/:storeId/departments/:id',
      component: DepartmentEdit
    },
    {
      name: 'Products',
      path: '/products',
      component: ProductList
    },
    {
      name: 'Product',
      path: '/products/:id',
      component: ProductEdit
    },
    {
      name: 'Profile',
      path: '/user/profile',
      component: Profile
    },
    {
      name: 'RecoverPassword',
      path: '/user/recover-password',
      component: RecoverPassword,
      meta: { public: true }
    },
    {
      name: 'ResetPassword',
      path: '/user/reset-password',
      component: ResetPassword,
      meta: { public: true }
    },
    {
      name: 'SignIn',
      path: '/user/sign-in',
      component: SignIn,
      meta: { public: true }
    },
    {
      name: 'Stores',
      path: '/stores',
      component: StoreList
    },
    {
      name: 'Store',
      path: '/stores/:id',
      component: StoreEdit
    },
    {
      name: 'SignUp',
      path: '/user/sign-up',
      component: SignUp,
      meta: { public: true }
    },
    {
      path: '*',
      redirect: '/404'
    }
  ]
})

router.beforeEach((to, _, next) => {
  if (!to.meta.public && !store.state.token) {
    return next({ name: 'SignIn' })
  }
  next()
})

export default router
