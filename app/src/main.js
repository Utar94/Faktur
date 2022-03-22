import Vue from 'vue'
import App from './App.vue'
import i18n from './i18n'
import router from './router'
import store from './store'
import './config/bootstrap'
import './config/fontAwesome'
import './config/vee-validate'
import './global'

Vue.config.productionTip = false

new Vue({
  i18n,
  render: h => h(App),
  router,
  store
}).$mount('#app')
