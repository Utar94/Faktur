<template>
  <b-container>
    <h1 v-t="'articles.title'" />
    <div class="my-2">
      <icon-button class="mx-1" :disabled="loading" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" icon="plus" text="actions.create" variant="success" v-b-modal.createArticle />
      <create-article-modal id="createArticle" @created="onCreate" />
    </div>
    <b-form>
      <b-row>
        <search-field class="col" v-model="search" />
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
    </b-form>
    <template v-if="articles.length">
      <table id="results" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'name.label'" />
            <th scope="col" v-t="'article.gtin.label'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="article in articles" :key="article.id">
            <td>
              <router-link :to="{ name: 'Article', params: { id: article.id } }" v-text="article.name" />
            </td>
            <td>{{ article.gtin || $t('none') }}</td>
            <td>{{ $d(new Date(article.updatedAt || article.createdAt), 'medium') }}</td>
            <td>
              <icon-button class="mx-1" icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`deleteArticle_${article.id}`" />
              <delete-modal
                confirm="articles.delete.confirm"
                :disabled="loading"
                :displayName="article.name"
                :id="`deleteArticle_${article.id}`"
                :loading="loading"
                title="articles.delete.title"
                @ok="onDelete(article, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="results" />
    </template>
    <p v-else v-t="'articles.none'" />
  </b-container>
</template>

<script>
import CreateArticleModal from './CreateArticleModal.vue'
import { getArticles, setArticleDeleted } from '@/api/articles'

export default {
  components: {
    CreateArticleModal
  },
  data: () => ({
    count: 10,
    desc: false,
    articles: [],
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
      return Object.entries(this.$i18n.t('articles.sort'))
        .map(([key, value]) => ({
          text: value,
          value: key
        }))
        .sort((a, b) => (a.text < b.text ? -1 : a.text > b.text ? 1 : 0))
    }
  },
  methods: {
    onCreate({ id }) {
      this.$router.push({ name: 'Article', params: { id } })
    },
    async onDelete({ id }, callback) {
      let deleted = false
      if (!this.loading) {
        this.loading = true
        try {
          await setArticleDeleted(id)
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
          const { data } = await getArticles(params ?? this.params)
          this.articles = data.items
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
