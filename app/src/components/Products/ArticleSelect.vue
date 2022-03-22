<template>
  <form-select
    :disabled="disabled"
    :id="id"
    :label="label"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import { getArticles } from '@/api/articles'

export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'article'
    },
    label: {
      type: String,
      default: 'product.article.label'
    },
    placeholder: {
      type: String,
      default: 'product.article.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  data: () => ({
    articles: []
  }),
  computed: {
    options() {
      return this.articles.map(({ id, name }) => ({
        text: name,
        value: id
      }))
    }
  },
  async created() {
    try {
      const { data } = await getArticles({ deleted: false, sort: 'Name' })
      this.articles = data.items
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
