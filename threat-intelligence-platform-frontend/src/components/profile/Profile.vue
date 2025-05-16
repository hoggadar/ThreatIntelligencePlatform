<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '../../services/authService'

const router = useRouter();
const auth = useAuth();
const user = ref<{ firstName: string, lastName: string, email: string } | null>(null);

onMounted(async () => {
  try {
    const userData = await auth.getCurrentUser();
    user.value = {
      firstName: userData.firstName ?? '',
      lastName: userData.lastName ?? '',
      email: userData.email
    };
  } catch (e) {
      console.error('Ошибка получения профиля:', e);
      router.push('/login');
    }
});

const handleLogout = () => {
  auth.logout();
  router.push('/login');
}
</script>

<template>
  <div class="flex justify-center items-center min-h-[70vh] px-4 py-6">
    <div class="w-full min-w-xl p-8 bg-white rounded-2xl shadow-lg" v-if="user">
      <h2 class="text-2xl font-bold text-center text-gray-800 mb-2">Добро пожаловать!</h2>
      <p class="text-sm text-center text-gray-500 mb-6">
        Ты вошел как <!-- {{ user.firstName }} {{ user.lastName }} --> 
                    <p class="text-lg text-black">{{ user.email }}</p>
      </p>
      <button @click="handleLogout"
        class="w-full py-2 px-4 text-white bg-red-600 hover:bg-red-700 font-medium rounded transition">
        Выйти
      </button>
    </div>
    <div v-else class="text-center text-gray-500">
      Загрузка профиля...
    </div>
  </div>
</template>

<style scoped></style>
