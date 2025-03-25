<template>
  <div class="min-h-screen flex flex-col">
    <Header />

    <main class="flex-grow container mx-auto px-4 pt-5 pb-8">
      <router-view />
    </main>

    <Footer />
  </div>
</template>

<script setup lang="ts">
import Header from '../src/components/layouts/Header.vue'
import Footer from '../src/components/layouts/Footer.vue'
</script>

<style>
#app {
  font-family: Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  min-height: 100vh;
  margin: 0;
  padding: 0;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  margin: 0;
  padding: 0;
}
</style>
