import Vue from 'vue'
import VueI18n from 'vue-i18n'
import fr from './fr.json'

Vue.use(VueI18n)

export default new VueI18n({
  dateTimeFormats: {
    en: {
      medium: {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric',
        second: 'numeric'
      }
    },
    fr: {
      medium: {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric',
        second: 'numeric'
      }
    }
  },
  locale: 'fr',
  fallbackLocale: 'fr',
  messages: { fr }
})
