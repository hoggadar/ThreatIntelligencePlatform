<template>
  <div class="bg-white rounded-lg overflow-hidden">
    <button @click="$emit('tab-change', 'personal')"
      class="w-full h-12 px-4 text-left flex items-center gap-3 border-l-0"
      :class="activeTab === 'personal' ? 'border-l-4 border-l-blue-500 bg-blue-50' : 'hover:bg-gray-50'">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-gray-600" fill="none" viewBox="0 0 24 24"
        stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
      </svg>
      <span>Личная информация</span>
    </button>
    <button @click="$emit('tab-change', 'security')"
      class="w-full h-12 px-4 text-left flex items-center gap-3 border-l-0"
      :class="activeTab === 'security' ? 'border-l-4 border-l-blue-500 bg-blue-50' : 'hover:bg-gray-50'">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-gray-600" fill="none" viewBox="0 0 24 24"
        stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
      </svg>
      <span>Безопасность</span>
    </button>
    <button @click="handleLogout"
      class="w-full h-12 px-4 text-left flex items-center gap-3 text-red-600 hover:bg-red-50">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
      </svg>
      <span>Выйти</span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuth } from '../../../services/authService';

// Определяем входные параметры
const props = defineProps<{
  activeTab: string;
}>();

// Определяем события
const emit = defineEmits<{
  'tab-change': [tab: string];
}>();

const router = useRouter();
const auth = useAuth();

const handleLogout = () => {
  auth.logout();
  router.push('/login');
};
</script>