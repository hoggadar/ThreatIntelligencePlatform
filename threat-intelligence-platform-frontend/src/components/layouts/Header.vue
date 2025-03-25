<template>
  <header class="fixed top-0 left-0 right-0 bg-white shadow-sm z-50">
    <nav class="container mx-auto px-4 py-4 flex items-center">
      <div class="w-1/4">
        <router-link to="/" class="text-xl font-bold text-gray-900">
          Threat Intelligence Platform
        </router-link>
      </div>

      <div class="flex-1 hidden md:flex justify-center items-center space-x-8">
        <router-link
          v-for="link in navLinks"
          :key="link.path"
          :to="link.path"
          class="text-gray-600 hover:text-primary-600 transition-colors"
          :class="{ 'text-primary-600': isCurrentRoute(link.path) }"
        >
          {{ link.name }}
        </router-link>
      </div>

      <div class="w-1/4 hidden md:flex justify-end">
        <router-link to="/login" class="btn btn-primary">
          Войти
        </router-link>
      </div>

      <button
        @click="isMenuOpen = !isMenuOpen"
        class="md:hidden ml-auto p-2 text-gray-600 hover:text-primary-600"
        aria-label="Открыть меню"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path
            v-if="!isMenuOpen"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M4 6h16M4 12h16M4 18h16"
          />
          <path
            v-else
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M6 18L18 6M6 6l12 12"
          />
        </svg>
      </button>
    </nav>

    <!-- Мобильное меню -->
    <div v-show="isMenuOpen" class="md:hidden bg-white border-t">
      <div class="container mx-auto px-4 py-2 space-y-2">
        <router-link
          v-for="link in navLinks"
          :key="link.path"
          :to="link.path"
          class="block py-2 text-gray-600 hover:text-primary-600"
          :class="{ 'text-primary-600': isCurrentRoute(link.path) }"
          @click="isMenuOpen = false"
        >
          {{ link.name }}
        </router-link>
        <router-link
          to="/login"
          class="block py-2 text-primary-600 font-medium"
          @click="isMenuOpen = false"
        >
          Войти
        </router-link>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const isMenuOpen = ref(false)

const navLinks = [
  { name: 'Панель управления', path: '/dashboard' },
  { name: 'Угрозы', path: '/threats' },
  { name: 'Аналитика', path: '/analytics' },
]

const isCurrentRoute = (path: string) => {
  return route.path === path
}
</script>
