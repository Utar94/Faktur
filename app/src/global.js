import Vue from 'vue'
import Gravatar from 'vue-gravatar'
import AuditInfo from './components/shared/AuditInfo.vue'
import CountSelect from './components/shared/CountSelect.vue'
import CreateModal from './components/shared/CreateModal.vue'
import DeleteModal from './components/shared/DeleteModal.vue'
import DescriptionField from './components/shared/DescriptionField.vue'
import FormField from './components/shared/FormField.vue'
import FormSelect from './components/shared/FormSelect.vue'
import FormTextarea from './components/shared/FormTextarea.vue'
import IconButton from './components/shared/IconButton.vue'
import IconSubmit from './components/shared/IconSubmit.vue'
import NameField from './components/shared/NameField.vue'
import NumberField from './components/shared/NumberField.vue'
import PasswordField from './components/shared/PasswordField.vue'
import SearchField from './components/shared/SearchField.vue'
import SortSelect from './components/shared/SortSelect.vue'

Vue.component('v-gravatar', Gravatar)

Vue.component('audit-info', AuditInfo)
Vue.component('count-select', CountSelect)
Vue.component('create-modal', CreateModal)
Vue.component('delete-modal', DeleteModal)
Vue.component('description-field', DescriptionField)
Vue.component('form-field', FormField)
Vue.component('form-select', FormSelect)
Vue.component('form-textarea', FormTextarea)
Vue.component('icon-button', IconButton)
Vue.component('icon-submit', IconSubmit)
Vue.component('name-field', NameField)
Vue.component('number-field', NumberField)
Vue.component('password-field', PasswordField)
Vue.component('search-field', SearchField)
Vue.component('sort-select', SortSelect)

Vue.mixin({
  methods: {
    getValidationState({ dirty, validated, valid = null }) {
      return dirty || validated ? valid : null
    },
    handleError(e = null) {
      if (e) {
        console.error(e)
      }
      this.toast('errorToast.title', 'errorToast.body', 'danger')
    },
    toast(title, body = '', variant = 'success') {
      this.$bvToast.toast(this.$i18n.t(body), {
        solid: true,
        title: this.$i18n.t(title),
        variant
      })
    }
  }
})
