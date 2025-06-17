<template>
  <div class="bg-white rounded-lg overflow-hidden">

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
import { useAuth } from '../services/authService';

const router = useRouter();
const auth = useAuth();

const handleLogout = () => {
  auth.logout();
  router.push('/login');
};
</script>