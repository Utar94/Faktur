<template>
  <b-form-group :label="$t(label)" label-for="id">
    <b-form-input :id="id" :lazy="lazy" :placeholder="$t(placeholder)" :value="value" @input="$emit('input', $event)" />
  </b-form-group>
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'search'
    },
    label: {
      type: String,
      default: 'search.label'
    },
    lazy: {
      type: Boolean,
      default: true
    },
    placeholder: {
      type: String,
      default: 'search.placeholder'
    },
    value: {}
  }
}
</script>
