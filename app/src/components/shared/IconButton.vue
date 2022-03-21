<template>
  <b-button :disabled="disabled" :size="size" :to="to" :type="type" :variant="variant" @click="$emit('click', $event)">
    <b-spinner v-if="loading" small />
    <font-awesome-icon v-else :icon="icon" />
    {{ $t(text) }}
  </b-button>
</template>

<script>
export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    icon: {
      type: String,
      required: true
    },
    loading: {
      type: Boolean,
      default: false
    },
    size: {
      type: String,
      default: ''
    },
    text: {
      type: String,
      default: ''
    },
    to: {},
    type: {
      type: String,
      default: 'button'
    },
    variant: {
      type: String,
      default: ''
    }
  }
}
</script>
