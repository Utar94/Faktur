<template>
  <validation-provider :name="$t(label).toLowerCase()" :rules="allRules" :vid="id" v-slot="validationContext" slim>
    <b-form-group
      :label="required ? '' : $t(label)"
      :label-for="id"
      :invalid-feedback="validationContext.errors[0]"
      :state="getValidationState(validationContext)"
    >
      <template #label v-if="required"><span class="text-danger">*</span> {{ $t(label) }}</template>
      <b-input-group :append="append">
        <b-form-input
          :disabled="disabled"
          :id="id"
          :max="maxValue"
          :maxlength="maxLength"
          :min="minValue"
          :minlength="minLength"
          :placeholder="$t(placeholder)"
          :ref="id"
          :state="getValidationState(validationContext)"
          :step="step"
          :type="type"
          :value="value"
          @input="$emit('input', $event)"
        />
      </b-input-group>
      <slot />
    </b-form-group>
  </validation-provider>
</template>

<script>
import { v4 as uuidv4 } from 'uuid'

export default {
  props: {
    append: {
      type: String,
      default: ''
    },
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
    maxLength: {
      type: Number,
      required: false
    },
    maxValue: {
      type: Number,
      required: false
    },
    minLength: {
      type: Number,
      required: false
    },
    minValue: {
      type: Number,
      required: false
    },
    placeholder: {
      type: String,
      default: ''
    },
    required: {
      type: Boolean,
      default: false
    },
    rules: {
      type: Object,
      default: null
    },
    step: {
      type: Number,
      required: false
    },
    type: {
      type: String,
      default: 'text'
    },
    value: {}
  },
  computed: {
    allRules() {
      const rules = { ...this.rules }
      if (typeof this.maxLength !== 'undefined') {
        rules.max = this.maxLength
      }
      if (typeof this.maxValue !== 'undefined') {
        rules.max_value = this.maxValue
      }
      if (typeof this.minLength !== 'undefined') {
        rules.min = this.minLength
      }
      if (typeof this.minValue !== 'undefined') {
        rules.min_value = this.minValue
      }
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
