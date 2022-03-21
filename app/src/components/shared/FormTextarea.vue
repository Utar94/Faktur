<template>
  <validation-provider :name="$t(label).toLowerCase()" :rules="allRules" :vid="id" v-slot="validationContext" slim>
    <b-form-group :label="required ? '' : $t(label)" :label-for="id" :invalid-feedback="validationContext.errors[0]">
      <template #label v-if="required"><span class="text-danger">*</span> {{ $t(label) }}</template>
      <b-form-textarea
        :disabled="disabled"
        :id="id"
        :placeholder="$t(placeholder)"
        :ref="id"
        :rows="rows"
        :state="getValidationState(validationContext)"
        :value="value"
        @input="$emit('input', $event)"
      />
      <slot />
    </b-form-group>
  </validation-provider>
</template>

<script>
import { v4 as uuidv4 } from 'uuid'

export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: () => uuidv4()
    },
    label: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: ''
    },
    required: {
      type: Boolean,
      default: false
    },
    rows: {
      type: Number,
      default: 10
    },
    rules: {
      type: Object,
      default: null
    },
    value: {}
  },
  computed: {
    allRules() {
      const rules = { ...this.rules }
      if (typeof this.required !== 'undefined') {
        rules.required = this.required
      }
      return rules
    }
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
