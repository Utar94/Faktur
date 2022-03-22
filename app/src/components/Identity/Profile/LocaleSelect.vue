<template>
  <form-select
    id="locale"
    label="user.locale.label"
    :options="options"
    placeholder="user.locale.placeholder"
    required
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import locales from '@/i18n/locales.json'

export default {
  props: {
    value: {}
  },
  computed: {
    options() {
      return this.$i18n.availableLocales
        .map(value => ({
          text: locales[value],
          value
        }))
        .filter(({ text }) => typeof text === 'string' && text.length > 0)
        .sort((a, b) => (a.text > b.text ? 1 : a.text < b.text ? -1 : 0))
    }
  }
}
</script>
