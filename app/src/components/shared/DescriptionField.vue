<template>
  <form-textarea :id="id" label="description.label" placeholder="description.placeholder" :required="required" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'description'
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  }
}
</script>
