<template>
  <header class="fixed top-0 left-0 right-0 bg-white shadow-sm z-50">
    <nav class="container mx-auto px-4 py-4 flex items-center">
      <div class="w-1/4">
        <router-link to="/" class="text-xl font-bold text-gray-900">
          Threat Intelligence Platform
        </router-link>
      </div>

      <div class="flex-1 hidden md:flex justify-center items-center space-x-8">
        <template v-for="link in navLinks" :key="link.name">
          <router-link v-if="link.path[0] === '/'" :to="link.path"
            class="text-gray-600 hover:text-primary-600 transition-colors"
            :class="{ 'text-primary-600': isCurrentRoute(link.path) }">
            {{ link.name }}
          </router-link>
          <a v-else href="#footer" class="text-gray-600 hover:text-primary-600 transition-colors cursor-pointer">
            {{ link.name }}
          </a>
        </template>
      </div>

      <div class="w-1/4 hidden md:flex justify-end">
        <template v-if="!authState.isAuthenticated">
          <router-link to="/login" class="btn btn-primary">
            Войти
          </router-link>
        </template>
        <template v-else>
          <button @click="handleProfileClick" class="w-14 h-14 rounded-full bg-gray-200 hover:bg-gray-300 transition-colors flex 
            items-center justify-center focus:outline-none">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-8 h-8 text-gray-600" fill="none" viewBox="0 0 24 24"
              stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round"
                d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </button>
        </template>
      </div>

      <button @click="isMenuOpen = !isMenuOpen" class="md:hidden ml-auto p-2 text-gray-600 hover:text-primary-600"
        aria-label="Открыть меню">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path v-if="!isMenuOpen" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M4 6h16M4 12h16M4 18h16" />
          <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </nav>

    <!-- Мобильное меню -->
    <div v-show="isMenuOpen" class="md:hidden bg-white border-t">
      <div class="container mx-auto px-4 py-2 space-y-2">
        <template v-for="link in navLinks" :key="link.name">
          <router-link v-if="link.path[0] === '/'" :to="link.path"
            class="block py-2 text-gray-600 hover:text-primary-600"
            :class="{ 'text-primary-600': isCurrentRoute(link.path) }" @click="isMenuOpen = false">
            {{ link.name }}
          </router-link>
          <a v-else href="#footer" class="block py-2 text-gray-600 hover:text-primary-600 cursor-pointer">
            {{ link.name }}
          </a>
        </template>
        <template v-if="!authState.isAuthenticated">
          <router-link to="/login" class="block py-2 text-primary-600 font-medium" @click="isMenuOpen = false">
            Войти
          </router-link>
        </template>
        <template v-else>
          <button @click="handleProfileClick" class="w-full flex items-center space-x-2 py-2" :class="{
            'text-gray-600 hover:text-primary-600': !isCurrentRoute('/profile'),
            'text-primary-600': isCurrentRoute('/profile')
          }">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24"
              stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round"
                d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
            <span>Профиль</span>
          </button>
        </template>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, watch, onBeforeUnmount } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth } from '../../services/authService'
import { hasCookie } from '../../utils/cookies'

const route = useRoute()
const router = useRouter()
const isMenuOpen = ref(false)
const auth = useAuth()

const navLinks = [
  { name: 'Панель управления', path: '/dashboard' },
  { name: 'Угрозы', path: '/threats' },
  { name: 'Аналитика', path: '/analytics' },
  { name: 'Контакты', path: '/contacts' },
]

// Создаем реактивное состояние авторизации
const authState = reactive({
  isAuthenticated: false
})

// Функция проверки авторизации, обновляющая состояние
const checkAuthentication = () => {
  // Проверяем наличие токена в cookie
  const hasTokenCookie = hasCookie('auth_token');

  // Резервная проверка флага в localStorage
  let hasLocalStorageFlag = false;
  try {
    hasLocalStorageFlag = localStorage.getItem('auth_token_debug') === 'token_exists';
  } catch (e) {
    // Ошибки localStorage игнорируем
  }

  // Обновляем реактивное состояние
  authState.isAuthenticated = hasTokenCookie || hasLocalStorageFlag;
}

onMounted(() => {
  // Проверяем авторизацию при монтировании
  checkAuthentication();

  // Регистрируем интервал для периодической проверки авторизации с увеличенным интервалом
  const intervalId = setInterval(checkAuthentication, 15000); // Проверка раз в 15 секунд вместо 5

  // Очищаем интервал при размонтировании компонента
  onBeforeUnmount(() => {
    clearInterval(intervalId);
  });
})

// Отслеживаем изменение маршрута для проверки авторизации
watch(() => route.path, checkAuthentication)

const handleProfileClick = () => {
  router.push('/profile');
  isMenuOpen.value = false;
}

const isCurrentRoute = (path: string) => {
  return route.path === path;
}
</script>

<style scoped>
.btn {
  @apply px-4 py-2 rounded-md font-medium transition-colors;
}

.btn-primary {
  @apply bg-primary-600 text-white hover:bg-primary-700;
}
</style>
