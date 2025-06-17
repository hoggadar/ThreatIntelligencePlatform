<template>
  <div v-if="show" class="fixed inset-0 flex items-center justify-center z-50">
    <div class="absolute inset-0 bg-black opacity-50" @click="close"></div>
    <div class="bg-white rounded-lg shadow-lg z-10 w-full max-w-md overflow-hidden">
      <div class="border-b border-gray-200">
        <div class="flex justify-between items-center p-4">
          <h3 class="text-lg font-semibold text-gray-800">Информация о пользователе</h3>
          <button @click="close" class="text-gray-500 hover:text-gray-700 transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
      </div>

      <!-- Загрузка -->
      <div v-if="loading" class="flex justify-center items-center p-8">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>

      <!-- Ошибка -->
      <div v-else-if="error" class="p-4">
        <div class="bg-red-50 border border-red-200 text-red-700 rounded-lg p-3 mb-2">
          <div class="flex items-start">
            <svg class="w-5 h-5 text-red-500 mr-2 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <div>
              <p class="font-medium">Ошибка загрузки данных</p>
              <p class="text-sm mt-1">{{ error }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Информация о пользователе -->
      <div v-else-if="user" class="p-4 text-center">
        <!-- Основная информация -->
        <div class="mb-4 bg-gray-50 p-4 rounded-lg border border-gray-100">
          <div class="flex flex-col items-center mb-3">
            <div class="bg-blue-100 rounded-full p-2 mb-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-blue-600" viewBox="0 0 20 20"
                fill="currentColor">
                <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd" />
              </svg>
            </div>
            <div>
              <h4 class="text-base font-medium text-gray-900">{{ user.firstName || '' }} {{ user.lastName || '' }}</h4>
              <p class="text-blue-600 text-sm">{{ user.email }}</p>
            </div>
          </div>

          <div class="border-t border-gray-200 pt-3 mx-auto">
            <div>
              <p class="text-xs text-gray-600">ID пользователя</p>
              <p class="font-medium text-gray-900 break-all text-xs mt-1">{{ user.id }}</p>
            </div>
          </div>
        </div>

        <!-- Роли и права -->
        <div class="mb-4">
          <h4 class="text-xs uppercase tracking-wider text-gray-600 font-semibold mb-2">Роли пользователя</h4>
          <div class="flex flex-wrap justify-center gap-2">
            <span v-for="role in user.roles" :key="role" class="px-2 py-0.5 rounded-full text-xs font-medium"
              :class="getRoleClass(role)">
              {{ role }}
            </span>
            <span v-if="!user.roles?.length" class="text-gray-500 italic text-xs">Нет назначенных ролей</span>
          </div>
        </div>

        <!-- Детали пользователя -->
        <div class="mb-4">
          <h4 class="text-xs uppercase tracking-wider text-gray-600 font-semibold mb-2">Дополнительная информация</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-3 max-w-md mx-auto">
            <div class="bg-gray-50 p-2 rounded-md">
              <p class="text-xs text-gray-500">Имя</p>
              <p class="font-medium text-sm">{{ user.firstName || 'Не указано' }}</p>
            </div>
            <div class="bg-gray-50 p-2 rounded-md">
              <p class="text-xs text-gray-500">Фамилия</p>
              <p class="font-medium text-sm">{{ user.lastName || 'Не указано' }}</p>
            </div>
          </div>
        </div>

        <!-- Детали ролей -->
        <div v-if="user.roles && user.roles.length > 0" class="mb-4">
          <h4 class="text-xs uppercase tracking-wider text-gray-600 font-semibold mb-2">Детали ролей</h4>
          <div class="space-y-2 max-w-md mx-auto">
            <div v-if="user.roles.includes('Admin')" class="bg-gray-50 p-3 rounded-md">
              <div class="flex items-center justify-center mb-1">
                <span class="px-2 py-0.5 text-xs rounded-full font-medium bg-purple-100 text-purple-800">
                  Admin
                </span>
              </div>
              <p class="text-xs text-gray-600">Администратор системы с полным доступом ко всем функциям и настройкам
                платформы</p>
            </div>
            <div v-if="user.roles.includes('User')" class="bg-gray-50 p-3 rounded-md">
              <div class="flex items-center justify-center mb-1">
                <span class="px-2 py-0.5 text-xs rounded-full font-medium bg-blue-100 text-blue-800">
                  User
                </span>
              </div>
              <p class="text-xs text-gray-600">Стандартный пользователь системы с ограниченным доступом к функциям
                платформы</p>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-gray-50 px-4 py-3 rounded-b-lg">
        <div class="flex justify-center">
          <button @click="close"
            class="px-3 py-1.5 text-sm text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 transition-colors">
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onBeforeUnmount } from 'vue';
import { getUserById, UserDTO } from '../../services/adminUserService';

const props = defineProps<{
  userId: string;
  show: boolean;
}>();

const emit = defineEmits<{
  'update:show': [show: boolean];
  close: [];
}>();

const user = ref<UserDTO | null>(null);
const loading = ref(false);
const error = ref<string | null>(null);

const disableBodyScroll = () => {
  document.body.classList.add('overflow-hidden');
};

const enableBodyScroll = () => {
  document.body.classList.remove('overflow-hidden');
};

const close = () => {
  enableBodyScroll();
  emit('update:show', false);
  emit('close');
};

const getRoleClass = (role: string) => {
  switch (role) {
    case 'Admin':
      return 'bg-purple-100 text-purple-800';
    case 'User':
      return 'bg-blue-100 text-blue-800';
    default:
      return 'bg-gray-100 text-gray-800';
  }
};

const fetchUserDetails = async () => {
  if (!props.userId) return;

  loading.value = true;
  error.value = null;

  try {
    user.value = await getUserById(props.userId);
  } catch (err: any) {
    error.value = err.message || 'Не удалось загрузить информацию о пользователе';
  } finally {
    loading.value = false;
  }
};

// Загружаем данные пользователя при открытии модального окна
watch(() => props.show, (newVal) => {
  if (newVal) {
    disableBodyScroll();
    fetchUserDetails();
  } else {
    enableBodyScroll();
  }
});

// Также загружаем данные при изменении userId (если модальное окно открыто)
watch(() => props.userId, (newVal) => {
  if (props.show && newVal) {
    fetchUserDetails();
  }
});

// На всякий случай включаем скролл, если компонент удаляется
onBeforeUnmount(() => {
  enableBodyScroll();
});
</script>

<style>
body.overflow-hidden {
  overflow: hidden;
}
</style>