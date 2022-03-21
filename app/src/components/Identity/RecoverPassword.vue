<template>
  <b-container>
    <h1 v-t="'recoverPassword.title'" />
    <b-alert variant="success" v-model="success">
      <strong v-t="'success'" />
      {{ $t('recoverPassword.success') }}
    </b-alert>
    <template v-if="!success">
      <b-alert show>{{ $t('recoverPassword.description') }}</b-alert>
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <username-field ref="username" v-model="username" />
          <icon-submit :disabled="loading" icon="paper-plane" :loading="loading" text="recoverPassword.submit" variant="primary" />
        </b-form>
      </validation-observer>
    </template>
  </b-container>
</template>

<script>
import { recoverPassword } from '@/api/identity'
import UsernameField from './shared/UsernameField.vue'

export default {
  components: {
    UsernameField
  },
  data: () => ({
    loading: false,
    success: false,
    username: null
  }),
  methods: {
    onSuccess() {
      this.username = null
      this.success = true
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            await recoverPassword({
              username: this.username
            })
            this.onSuccess()
          }
        } catch (e) {
          if (e.status === 400 && e.data?.succeeded === false) {
            this.onSuccess()
          } else {
            this.username = ''
            this.$refs.username.focus()
            this.handleError(e)
          }
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
