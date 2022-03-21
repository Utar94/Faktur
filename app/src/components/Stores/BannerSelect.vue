<template>
  <form-select :id="id" :label="label" :options="options" :placeholder="placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
import { getBanners } from '@/api/banners'

export default {
  props: {
    id: {
      type: String,
      default: 'banner'
    },
    label: {
      type: String,
      default: 'store.banner.label'
    },
    placeholder: {
      type: String,
      default: 'store.banner.placeholder'
    },
    value: {}
  },
  data: () => ({
    banners: []
  }),
  computed: {
    options() {
      return this.banners.map(({ id, name }) => ({
        text: name,
        value: id
      }))
    }
  },
  async created() {
    try {
      const { data } = await getBanners({ deleted: false, sort: 'Name' })
      this.banners = data.items
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
