<template>
  <b-row>
    <b-col>
      <b-form v-show="editing.left" @submit.prevent="save('left')">
        <b-input-group>
          <b-form-input id="left-header" :placeholder="$t('receipt.headerPlaceholder')" v-model="headers.left" />
          <template #append>
            <icon-submit :disabled="!headers.left" icon="save" variant="primary" />
          </template>
        </b-input-group>
      </b-form>
      <h3 v-show="!editing.left" class="editable text-center" v-text="value.left" @click="toggle('left')" />
    </b-col>
    <b-col>
      <h3 class="text-center" v-t="'receipt.shared'" />
    </b-col>
    <b-col>
      <b-form v-show="editing.right" @submit.prevent="save('right')">
        <b-input-group>
          <b-form-input id="right-header" :placeholder="$t('receipt.headerPlaceholder')" v-model="headers.right" />
          <template #append>
            <icon-submit :disabled="!headers.right" icon="save" variant="primary" />
          </template>
        </b-input-group>
      </b-form>
      <h3 v-show="!editing.right" class="editable text-center" v-text="value.right" @click="toggle('right')" />
    </b-col>
  </b-row>
</template>

<script>
export default {
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  data: () => ({
    editing: {
      left: false,
      right: false
    },
    headers: {
      left: '',
      right: ''
    }
  }),
  methods: {
    save(key) {
      const headers = { ...this.headers }
      headers[key] = this.headers[key]
      this.$emit('input', headers)
      this.toggle(key)
    },
    toggle(key) {
      this.editing[key] = !this.editing[key]
    }
  },
  created() {
    for (const [key, value] of Object.entries(this.value)) {
      this.editing[key] = !value
    }
  },
  watch: {
    value: {
      deep: true,
      immediate: true,
      handler(headers) {
        this.headers.left = headers.left
        this.headers.right = headers.right
      }
    }
  }
}
</script>

<style scoped>
.editable:hover {
  cursor: pointer;
  text-decoration: underline;
}
</style>
