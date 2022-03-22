<template>
  <b-form-group :label="$t(label)" :label-for="id">
    <b-form-select :id="id" :value="value" @input="$emit('input', $event)">
      <b-form-select-option v-for="(option, index) in options" :key="index" :value="option">{{ option }}</b-form-select-option>
    </b-form-select>
  </b-form-group>
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'count'
    },
    label: {
      type: String,
      default: 'count'
    },
    options: {
      type: Array,
      default: () => [10, 25, 50, 100]
    },
    value: {}
  }
}
</script>
