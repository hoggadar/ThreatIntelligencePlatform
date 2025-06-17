<template>
  <div class="min-h-screen flex flex-col">
    <Header />

    <main class="flex-grow container mx-auto px-4 pt-5 pb-8">
      <FormStyles />
      <router-view />
    </main>

    <Footer />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import Header from '../src/components/layouts/Header.vue'
import Footer from '../src/components/layouts/Footer.vue'
import FormStyles from './components/common/FormStyles.vue'
import { useAuth } from './services/authService'
import { testCookieSupport } from './utils/cookies'

// Проверка авторизации при загрузке приложения
onMounted(() => {
  // Тестируем поддержку куки в браузере
  testCookieSupport();

  // Используем сервис авторизации для обновления состояния
  const auth = useAuth();
  auth.checkAuth();
});
</script>

<style>
:root {
  font-family: Inter, system-ui, Avenir, Helvetica, Arial, sans-serif;
  line-height: 1.5;
  font-weight: 400;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app {
  min-height: 100vh;
  margin: 0;
  padding: 0;
}

body {
  @apply bg-gray-50;
  margin: 0;
  padding: 0;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}
</style>
