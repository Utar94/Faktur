<template>
  <b-container>
    <h1 v-t="'signIn.title'" />
    <b-alert dismissible variant="warning" v-model="invalidCredentials">
      <strong v-t="'signIn.failed'" />
      {{ $t('signIn.invalidCredentials') }}
    </b-alert>
    <b-alert dismissible variant="warning" v-model="notConfirmed">
      <strong v-t="'signIn.failed'" />
      {{ $t('signIn.notConfirmed') }}
    </b-alert>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <username-field v-model="username" />
        <password-field id="password" label="user.password.label" placeholder="user.password.placeholder" ref="password" required v-model="password" />
        <b-form-group>
          <b-form-checkbox v-model="remember">{{ $t('signIn.remember') }}</b-form-checkbox>
        </b-form-group>
        <p>
          <router-link :to="{ name: 'RecoverPassword' }" v-t="'signIn.recoverPassword'" />
        </p>
        <icon-submit :disabled="loading" icon="sign-in-alt" :loading="loading" text="signIn.submit" variant="primary" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import { mapActions, mapState } from 'vuex'
import { signIn } from '@/api/identity'
import UsernameField from './shared/UsernameField.vue'

export default {
  components: {
    UsernameField
  },
  data: () => ({
    invalidCredentials: false,
    loading: false,
    notConfirmed: false,
    password: null,
    remember: false,
    username: null
  }),
  computed: {
    ...mapState(['token'])
  },
  methods: {
    ...mapActions(['signIn']),
    async submit() {
      if (!this.loading) {
        this.loading = true
        this.invalidCredentials = false
        this.notConfirmed = false
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await signIn({
              password: this.password,
              username: this.username
            })
            this.signIn(data)
            if (this.remember) {
              localStorage.setItem('faktur_token', JSON.stringify(data))
            }
            this.password = null
            this.$router.push({ name: 'Profile' })
          }
        } catch (e) {
          if (e.status === 400) {
            if (e.data?.isNotAllowed === true) {
              this.notConfirmed = true
            } else {
              this.invalidCredentials = true
            }
          } else {
            this.handleError(e)
          }
          this.password = ''
          this.$refs.password.focus()
        } finally {
          this.loading = false
        }
      }
    }
  },
  async created() {
    if (this.token) {
      return this.$router.push({ name: 'Profile' })
    }
    const token = localStorage.getItem('faktur_token')
    if (token) {
      this.signIn(JSON.parse(token))
      return this.$router.push({ name: 'Profile' })
    }
  }
}
</script>
