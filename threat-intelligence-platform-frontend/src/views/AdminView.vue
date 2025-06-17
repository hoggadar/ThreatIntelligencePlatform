<template>
  <div class="space-y-6 px-4 py-6">
    <h1 class="text-2xl font-bold text-gray-800 space-y-6 pt-6">Панель администратора</h1>

    <div v-if="error" class="bg-red-50 border border-red-200 text-red-800 rounded-md p-4 mb-6">
      {{ error }}
      <button @click="tryAgain" class="text-blue-600 underline ml-2">Попробовать снова</button>
    </div>

    <div class="flex flex-wrap gap-4 mb-6">
      <button @click="activeTab = 'users'" class="px-4 py-2 rounded-md"
        :class="activeTab === 'users' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'">
        Управление пользователями
      </button>
      <button @click="activeTab = 'ioc'" class="px-4 py-2 rounded-md"
        :class="activeTab === 'ioc' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'">
        Управление IoC
      </button>
    </div>

    <div v-if="activeTab === 'users' && !error">
      <AdminUsers />
    </div>
    <div v-else-if="activeTab === 'ioc' && !error">
      <AdminIoC />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '../services/authService';
import AdminUsers from '../components/admin/AdminUsers.vue';
import AdminIoC from '../components/admin/AdminIoC.vue';

const router = useRouter();
const auth = useAuth();
const activeTab = ref('users');
const error = ref('');

const checkAdminAccess = async () => {
  try {
    const userData = await auth.getCurrentUser();
    if (userData.role !== 'Admin') {
      router.push('/profile');
    }
    error.value = '';
  } catch (e: any) {
    error.value = e.message || 'Ошибка доступа: требуются права администратора';
    router.push('/login');
  }
};

const tryAgain = () => {
  checkAdminAccess();
};

onMounted(() => {
  checkAdminAccess();
});
</script>