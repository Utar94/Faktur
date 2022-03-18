<template>
  <b-container>
    <h1 v-t="'user.title'" />
    <template v-if="profile">
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row" v-t="'user.createdAt'" />
            <td v-text="$d(new Date(profile.createdAt), 'medium')" />
          </tr>
          <tr v-if="profile.confirmedAt">
            <th scope="row" v-t="'user.confirmedAt'" />
            <td v-text="$d(new Date(profile.confirmedAt), 'medium')" />
          </tr>
          <tr v-if="profile.updatedAt">
            <th scope="row" v-t="'user.updatedAt'" />
            <td v-text="$d(new Date(profile.updatedAt), 'medium')" />
          </tr>
          <tr v-if="profile.fullName">
            <th scope="row" v-t="'user.fullName'" />
            <td v-text="profile.fullName" />
          </tr>
          <tr v-if="profile.email">
            <th scope="row" v-t="'user.email.label'" />
            <td v-text="profile.email" />
          </tr>
        </tbody>
      </table>
      <h2 v-t="'user.editProfile'" />
      <validation-observer ref="profileForm">
        <b-form @submit.prevent="submitProfile">
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
          <locale-select v-model="locale" />
          <picture-field v-model="picture" />
          <icon-submit :disabled="loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
        </b-form>
      </validation-observer>
      <h2 v-t="'user.password.label'" />
      <b-alert dismissible variant="warning" v-model="passwordMismatch">
        <strong v-t="'user.password.changeFailed'" />
        {{ $t('user.password.mismatch') }}
      </b-alert>
      <p v-if="profile.passwordChangedAt">{{ $t('user.password.changedAt', { date: $d(new Date(profile.passwordChangedAt), 'medium') }) }}</p>
      <validation-observer ref="passwordForm">
        <b-form @submit.prevent="submitPassword">
          <password-field
            id="current"
            label="user.password.current.label"
            placeholder="user.password.current.placeholder"
            ref="current"
            required
            v-model="current"
          />
          <b-row>
            <password-field
              class="col"
              id="password"
              label="user.password.label"
              :min-length="6"
              placeholder="user.password.placeholder"
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
          <icon-submit :disabled="loading" icon="key" :loading="loading" text="user.changePassword" variant="primary" />
        </b-form>
      </validation-observer>
      <div class="my-3" />
    </template>
  </b-container>
</template>

<script>
import { changePassword, getProfile, saveProfile } from '@/api/identity'
import LocaleSelect from './Profile/LocaleSelect.vue'
import PictureField from './Profile/PictureField.vue'

export default {
  components: {
    LocaleSelect,
    PictureField
  },
  data: () => ({
    confirmation: null,
    current: null,
    firstName: null,
    lastName: null,
    locale: null,
    loading: false,
    password: null,
    passwordMismatch: false,
    picture: null,
    profile: null
  }),
  methods: {
    setModel(model) {
      this.profile = model
      this.firstName = model?.firstName
      this.lastName = model?.lastName
      this.locale = model?.locale
      this.picture = model?.picture
    },
    async submitPassword() {
      if (!this.loading) {
        this.loading = true
        this.passwordMismatch = false
        try {
          if (await this.$refs.passwordForm.validate()) {
            await changePassword({
              current: this.current,
              password: this.password
            })
            const { data } = await getProfile()
            this.profile = data
            this.current = null
            this.password = null
            this.confirmation = null
            this.$refs.passwordForm.reset()
            this.toast('success', 'user.password.success')
          }
        } catch (e) {
          if (e.status === 400 && e.data?.succeeded === false && e.data.errors.length === 1 && e.data.errors[0].code === 'PasswordMismatch') {
            this.passwordMismatch = true
          } else {
            this.handleError(e)
          }
          this.current = ''
          this.password = ''
          this.confirmation = ''
          this.$refs.current.focus()
        } finally {
          this.loading = false
        }
      }
    },
    async submitProfile() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.profileForm.validate()) {
            const { data } = await saveProfile({
              firstName: this.firstName,
              lastName: this.lastName,
              locale: this.locale,
              picture: this.picture || null
            })
            this.setModel(data)
            this.$refs.profileForm.reset()
            this.toast('success', 'user.profileUpdated')
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  async created() {
    try {
      const { data } = await getProfile()
      this.setModel(data)
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
