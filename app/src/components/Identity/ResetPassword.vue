<template>
  <b-container>
    <h1 v-t="'resetPassword.title'" />
    <b-alert variant="success" v-model="success">
      <p>
        <strong v-t="'success'" />
        {{ $t('resetPassword.success') }}
      </p>
      <icon-button icon="sign-in-alt" text="signIn.title" :to="{ name: 'SignIn' }" variant="primary" />
    </b-alert>
    <validation-observer ref="form">
      <b-form v-if="!success" @submit.prevent="submit">
        <password-field
          id="password"
          label="user.password.label"
          :min-length="6"
          placeholder="user.password.placeholder"
          ref="password"
          required
          :rules="{ password: true }"
          v-model="password"
        />
        <password-field
          id="confirmation"
          label="user.password.confirmation.label"
          placeholder="user.password.confirmation.placeholder"
          required
          :rules="{ confirmed: 'password' }"
          v-model="confirmation"
        />
        <icon-submit :disabled="loading" icon="key" :loading="loading" text="resetPassword.submit" variant="primary" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import { mapActions } from 'vuex'
import { resetPassword } from '@/api/identity'

export default {
  data: () => ({
    confirmation: null,
    id: null,
    loading: false,
    password: null,
    success: false,
    token: null
  }),
  methods: {
    ...mapActions(['translate']),
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            await resetPassword({
              id: this.id,
              password: this.password,
              token: this.token
            })
            this.password = null
            this.confirmation = null
            this.success = true
          }
        } catch (e) {
          this.password = ''
          this.confirmation = ''
          this.$refs.password.focus()
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  created() {
    this.id = this.$route.query.id
    this.token = this.$route.query.token
    if (!this.id || !this.token) {
      return this.$router.push({ name: 'SignIn' })
    }
    const locale = this.$route.query.locale
    if (locale) {
      this.translate(locale)
    }
  }
}
</script>
