import Vue from 'vue'
import Vuex from 'vuex'
import VuexPersistence from 'vuex-persist'

Vue.use(Vuex)

const vuexLocal = new VuexPersistence({
  key: 'faktur-dev',
  storage: window.sessionStorage
})

export default new Vuex.Store({
  state: {
    locale: null,
    token: null
  },
  actions: {
    signIn({ commit }, token) {
      commit('setToken', token)
    },
    signOut({ commit }) {
      commit('setToken', null)
    },
    translate({ commit }, locale) {
      commit('setLocale', locale)
    }
  },
  mutations: {
    setLocale(state, locale) {
      state.locale = locale
    },
    setToken(state, token) {
      state.token = token
    }
  },
  plugins: [vuexLocal.plugin]
})
