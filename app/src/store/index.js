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
    headers: null,
    locale: null,
    token: null
  },
  actions: {
    saveHeaders({ commit }, { left, right }) {
      commit('setHeaders', { left, right })
    },
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
    setHeaders(state, headers) {
      state.headers = headers
    },
    setLocale(state, locale) {
      state.locale = locale
    },
    setToken(state, token) {
      state.token = token
    }
  },
  plugins: [vuexLocal.plugin]
})
