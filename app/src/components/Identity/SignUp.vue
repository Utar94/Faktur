<template>
  <b-container>
    <h1 v-t="'signUp.title'" />
    <b-alert variant="success" v-model="success">
      <strong v-t="'success'" />
      {{ $t('signUp.success') }}
    </b-alert>
    <validation-observer ref="form">
      <b-form v-if="!success" @submit.prevent="submit">
        <b-row>
          <form-field
            class="col"
            id="firstName"
            label="user.firstName.label"
            :max-length="128"
            placeholder="user.firstName.placeholder"
            required
            v-model="firstName"
          />
          <form-field
            class="col"
            id="lastName"
            label="user.lastName.label"
            :max-length="128"
            placeholder="user.lastName.placeholder"
            required
            v-model="lastName"
          />
        </b-row>
        <b-row>
          <form-field
            class="col"
            id="email"
            label="user.email.label"
            :max-length="256"
            placeholder="user.email.placeholder"
            required
            :rules="{ email: true }"
            type="email"
            v-model="email"
          />
        </b-row>
        <b-row>
          <password-field
            class="col"
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
            class="col"
            id="confirmation"
            label="user.password.confirmation.label"
            placeholder="user.password.confirmation.placeholder"
            required
            :rules="{ confirmed: 'password' }"
            v-model="confirmation"
          />
        </b-row>
        <icon-submit :disabled="loading" icon="user" :loading="loading" text="signUp.submit" variant="primary" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import { signUp } from '@/api/identity'

export default {
  data: () => ({
    confirmation: null,
    email: null,
    firstName: null,
    lastName: null,
    loading: false,
    password: null,
    success: false
  }),
  methods: {
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            await signUp({
              email: this.email,
              firstName: this.firstName,
              lastName: this.lastName,
              locale: this.$i18n.locale,
              password: this.password
            })
            this.confirmation = null
            this.email = null
            this.firstName = null
            this.lastName = null
            this.password = null
            this.success = true
          }
        } catch (e) {
          let hasError = true
          if (e.status === 400 && e.data?.succeeded === false && e.data.errors.length) {
            hasError = false
            for (const error of e.data.errors) {
              if (error.code === 'DuplicateEmail' || error.code === 'DuplicateUserName') {
                this.$refs.form.setErrors({
                  email: this.$i18n.t('signUp.duplicateEmail')
                })
              } else {
                hasError = true
              }
            }
          }
          if (hasError) {
            this.handleError(e)
          }
          this.password = ''
          this.confirmation = ''
          this.$refs.password.focus()
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
