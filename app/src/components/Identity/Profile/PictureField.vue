<template>
  <validation-provider :rules="{ max: 2048, url: true }" v-slot="validationContext" slim>
    <b-form-group
      :label="$t('user.picture.label')"
      :invalid-feedback="validationContext.errors[0]"
      :state="getValidationState(validationContext)"
      label-for="picture"
    >
      <b-input-group>
        <b-form-input
          id="picture"
          :placeholder="$t('user.picture.placeholder')"
          :state="getValidationState(validationContext)"
          :value="value"
          @input="$emit('input', $event)"
        />
        <b-input-group-append>
          <a :class="{ btn: true, 'btn-info': true, disabled: !value || getValidationState(validationContext) === false }" :href="value" target="_blank">
            <font-awesome-icon icon="eye" />
            {{ $t('actions.view') }}
          </a>
        </b-input-group-append>
      </b-input-group>
    </b-form-group>
  </validation-provider>
</template>

<script>
export default {
  props: {
    value: {}
  }
}
</script>
