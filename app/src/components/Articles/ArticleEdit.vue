<template>
  <b-container>
    <template v-if="article">
      <h1 v-text="article.name" />
      <audit-info :entity="article" />
      <validation-observer ref="form">
        <b-form @submit.prevent="submit">
          <name-field v-model="name" />
          <form-field
            id="gtin"
            label="article.gtin.label"
            :max-value="99999999999999"
            :min-value="0"
            placeholder="article.gtin.placeholder"
            type="number"
            v-model.number="gtin"
          />
          <description-field v-model="description" />
          <div>
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <icon-button class="mx-1" icon="ban" text="actions.cancel" :to="{ name: 'Articles' }" />
          </div>
        </b-form>
      </validation-observer>
    </template>
  </b-container>
</template>

<script>
import { getArticle, updateArticle } from '@/api/articles'

export default {
  data: () => ({
    article: null,
    description: null,
    gtin: null,
    loading: false,
    name: null
  }),
  computed: {
    hasChanges() {
      return this.name !== this.article.name || (this.description ?? '') !== (this.article.description ?? '') || this.gtin !== this.article.gtin
    }
  },
  methods: {
    setModel(article) {
      this.article = article
      this.description = article.description
      this.gtin = article.gtin
      this.name = article.name
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateArticle(this.article.id, {
              description: this.description,
              gtin: this.gtin,
              name: this.name
            })
            this.setModel(data)
            this.$refs.form.reset()
            this.toast('success', 'article.saved')
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
      const { data } = await getArticle(this.$route.params.id)
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
