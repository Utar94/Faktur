<template>
  <b-container>
    <h1 v-t="'user.confirm.title'" />
    <b-alert variant="success" v-model="success">
      <p>
        <strong v-t="'success'" />
        {{ $t('user.confirm.success') }}
      </p>
      <icon-button icon="sign-in-alt" text="signIn.title" :to="{ name: 'SignIn' }" variant="primary" />
    </b-alert>
  </b-container>
</template>

<script>
import { mapActions } from 'vuex'
import { confirm } from '@/api/identity'

export default {
  data: () => ({
    success: false
  }),
  methods: {
    ...mapActions(['translate'])
  },
  async created() {
    const id = this.$route.query.id
    const token = this.$route.query.token
    if (!id || !token) {
      return this.$router.push({ name: 'SignIn' })
    }
    const locale = this.$route.query.locale
    if (locale) {
      this.translate(locale)
    }
    try {
      await confirm({ id, token })
      this.success = true
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
