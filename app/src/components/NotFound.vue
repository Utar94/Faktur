<template>
  <b-container>
    <b-row class="vh-100 text-center" align-v="center">
      <b-col>
        <b-jumbotron header="404" :lead="$t('notFound.lead')">
          <p v-t="'notFound.help'" />
          <icon-button icon="home" text="notFound.link" :to="{ name: 'Home' }" variant="primary" />
        </b-jumbotron>
      </b-col>
    </b-row>
  </b-container>
</template>

<style scoped>
.container {
  text-align: center;
}
</style>
