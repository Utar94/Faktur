<template>
  <div>
    <b-navbar toggleable="lg" type="dark" variant="dark">
      <b-navbar-brand :to="{ name: 'Home' }">
        <img src="@/assets/logo.png" alt="Faktur Logo" width="32" />
        Faktur
      </b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item :to="{ name: 'Articles' }">
            <font-awesome-icon icon="carrot" />
            {{ $t('articles.title') }}
          </b-nav-item>
          <b-nav-item :to="{ name: 'Banners' }">
            <font-awesome-icon icon="flag" />
            {{ $t('banners.title') }}
          </b-nav-item>
          <b-nav-item :to="{ name: 'Stores' }">
            <font-awesome-icon icon="store" />
            {{ $t('stores.title') }}
          </b-nav-item>
          <b-nav-item :to="{ name: 'Products' }">
            <font-awesome-icon icon="shopping-cart" />
            {{ $t('products.title') }}
          </b-nav-item>
        </b-navbar-nav>

        <b-navbar-nav class="ml-auto">
          <!-- <b-nav-form>
            <b-input-group>
              <b-form-input size="sm" :placeholder="$t('actions.search')" />
              <b-input-group-append>
                <icon-button class="my-2 my-sm-0" icon="search" size="sm" />
              </b-input-group-append>
            </b-input-group>
          </b-nav-form> -->

          <b-nav-item-dropdown :text="localeName" right>
            <b-dropdown-item v-for="locale in otherLocales" :key="locale.value" :active="locale.value === $i18n.locale" @click="translate(locale.value)">
              {{ locale.text }}
            </b-dropdown-item>
          </b-nav-item-dropdown>

          <template v-if="token">
            <b-nav-item-dropdown v-if="token" right>
              <template #button-content>
                <img v-if="picture" :src="picture" alt="Avatar" class="rounded-circle" width="24" height="24" />
                <v-gravatar v-else class="rounded-circle" :email="email" :size="24" />
              </template>
              <b-dropdown-item :to="{ name: 'Profile' }">
                <font-awesome-icon icon="user" />
                {{ name }}
              </b-dropdown-item>
              <b-dropdown-item @click="doSignOut">
                <font-awesome-icon icon="sign-out-alt" />
                {{ $t('user.signOut') }}
              </b-dropdown-item>
            </b-nav-item-dropdown>
          </template>
          <template v-else>
            <b-nav-item :to="{ name: 'SignIn' }">
              <font-awesome-icon icon="sign-in-alt" />
              {{ $t('signIn.title') }}
            </b-nav-item>
            <b-nav-item :to="{ name: 'SignUp' }">
              <font-awesome-icon icon="user" />
              {{ $t('signUp.title') }}
            </b-nav-item>
          </template>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
  </div>
</template>

<script>
import jwt from 'jsonwebtoken'
import { localize } from 'vee-validate'
import { mapActions, mapState } from 'vuex'
import { signOut } from '@/api/identity'
import locales from '@/i18n/locales.json'

export default {
  data: () => ({
    loading: false
  }),
  computed: {
    ...mapState(['locale', 'token']),
    email() {
      return this.token ? jwt.decode(this.token.access_token).email : null
    },
    localeName() {
      return locales[this.$i18n.locale]
    },
    name() {
      return (this.token ? jwt.decode(this.token.access_token).name : null) ?? this.$i18n.t('user.title')
    },
    otherLocales() {
      return this.$i18n.availableLocales
        .map(value => ({
          text: locales[value],
          value
        }))
        .filter(({ text }) => typeof text === 'string' && text.length > 0 && text !== this.localeName)
        .sort((a, b) => (a.text > b.text ? 1 : a.text < b.text ? -1 : 0))
    },
    picture() {
      return this.token ? jwt.decode(this.token.access_token).picture : null
    }
  },
  methods: {
    ...mapActions(['translate']),
    async doSignOut() {
      if (!this.loading) {
        this.loading = true
        try {
          const { refresh_token } = this.token
          await signOut({ refresh_token })
          this.$store.dispatch('signOut')
          localStorage.removeItem('logitar_token')
          this.$router.push({ name: 'SignIn' })
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    locale: {
      immediate: true,
      handler(locale) {
        if (locale) {
          this.$i18n.locale = locale
          localize(locale)
        }
      }
    }
  }
}
</script>
