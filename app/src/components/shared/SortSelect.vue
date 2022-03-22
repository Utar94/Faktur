<template>
  <b-form-group :label="$t(label)" :label-for="id">
    <b-form-select :id="id" :options="options" :value="value" @input="$emit('input', $event)">
      <template #first>
        <b-form-select-option :value="null">{{ $t(placeholder) }}</b-form-select-option>
      </template>
    </b-form-select>
    <b-form-checkbox :checked="desc" @input="$emit('desc', $event)">{{ $t('desc') }}</b-form-checkbox>
  </b-form-group>
</template>

<script>
export default {
  props: {
    desc: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'sort'
    },
    label: {
      type: String,
      default: 'sort.label'
    },
    options: {
      type: Array,
      default: () => []
    },
    placeholder: {
      type: String,
      default: 'sort.placeholder'
    },
    value: {}
  }
}
</script>
