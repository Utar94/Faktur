<template>
  <b-container>
    <template v-if="banner">
      <h1 v-text="banner.name" />
      <audit-info :entity="banner" />
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <name-field v-model="name" />
          <description-field v-model="description" />
          <div>
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <icon-button class="mx-1" icon="ban" text="actions.cancel" :to="{ name: 'Banners' }" />
          </div>
        </b-form>
      </validation-observer>
    </template>
  </b-container>
</template>

<script>
import { getBanner, updateBanner } from '@/api/banners'

export default {
  data: () => ({
    banner: null,
    description: null,
    loading: false,
    name: null
  }),
  computed: {
    hasChanges() {
      return this.name !== this.banner.name || (this.description ?? '') !== (this.banner.description ?? '')
    }
  },
  methods: {
    setModel(banner) {
      this.banner = banner
      this.description = banner.description
      this.name = banner.name
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateBanner(this.banner.id, {
              description: this.description,
              name: this.name
            })
            this.setModel(data)
            this.$refs.form.reset()
            this.toast('success', 'banner.saved')
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
      const { data } = await getBanner(this.$route.params.id)
      this.setModel(data)
    } catch (e) {
      const { status } = e
      if (status === 404) {
        return this.$router.push({ name: 'NotFound' })
      }
      this.handleError(e)
    }
  }
}
</script>
