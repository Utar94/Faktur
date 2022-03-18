<template>
  <b-container>
    <div class="my-2">
      <icon-button class="mx-1" :disabled="loading" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" icon="plus" text="actions.create" variant="success" v-b-modal.createDepartment />
      <create-department-modal id="createDepartment" :store-id="storeId" @created="refresh()" />
    </div>
    <b-form>
      <b-row>
        <search-field class="col" v-model="search" />
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
    </b-form>
    <template v-if="departments.length">
      <table id="results" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'name.label'" />
            <th scope="col" v-t="'number.label'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="department in departments" :key="department.id">
            <td>
              <router-link :to="{ name: 'Department', params: { id: department.id, storeId: department.storeId } }" v-text="department.name" />
            </td>
            <td>{{ department.number || $t('none') }}</td>
            <td>{{ $d(new Date(department.updatedAt || department.createdAt), 'medium') }}</td>
            <td>
              <icon-button class="mx-1" icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`deleteDepartment_${department.id}`" />
              <delete-modal
                confirm="departments.delete.confirm"
                :disabled="loading"
                :displayName="department.name"
                :id="`deleteDepartment_${department.id}`"
                :loading="loading"
                title="departments.delete.title"
                @ok="onDelete(department, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="results" />
    </template>
    <p v-else v-t="'departments.none'" />
  </b-container>
</template>

<script>
import CreateDepartmentModal from './CreateDepartmentModal.vue'
import { getDepartments, setDepartmentDeleted } from '@/api/departments'

export default {
  components: {
    CreateDepartmentModal
  },
  props: {
    storeId: {
      type: Number,
      required: true
    }
  },
  data: () => ({
    count: 10,
    desc: false,
    departments: [],
    loading: false,
    page: 1,
    search: null,
    sort: 'Name',
    total: 0
  }),
  computed: {
    params() {
      return {
        deleted: false,
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return Object.entries(this.$i18n.t('departments.sort'))
        .map(([key, value]) => ({
          text: value,
          value: key
        }))
        .sort((a, b) => (a.text < b.text ? -1 : a.text > b.text ? 1 : 0))
    }
  },
  methods: {
    async onDelete({ id }, callback) {
      let deleted = false
      if (!this.loading) {
        this.loading = true
        try {
          await setDepartmentDeleted(id)
          deleted = true
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (deleted) {
          await this.refresh()
        }
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getDepartments(this.storeId, params ?? this.params)
          this.departments = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(params) {
        await this.refresh(params)
      }
    }
  }
}
</script>
