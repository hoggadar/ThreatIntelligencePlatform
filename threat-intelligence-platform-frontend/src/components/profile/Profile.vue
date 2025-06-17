<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '../../services/authService';
import { getUserByIdFromApi, getUserByEmail } from '../../services/userService';
import { jwtDecode } from 'jwt-decode';

import ProfileHeader from './ProfileComponents/ProfileHeader.vue';
import ProfileNavigation from './ProfileComponents/ProfileNavigation.vue';
import ProfilePersonalInfo from './ProfileComponents/ProfilePersonalInfo.vue';
import ProfileSecurity from './ProfileComponents/ProfileSecurity.vue';

const router = useRouter();
const auth = useAuth();
const user = ref<{ id: string, firstName: string, lastName: string, email: string, role: string } | null>(null);
const activeTab = ref('personal');
const isLoadingProfile = ref(true);
const error = ref('');

interface DecodedJwtPayload {
  [key: string]: any;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"?: string;
  exp?: number;
}

// Функция для проверки токена
const validateToken = (token: string): boolean => {
  try {
    const decodedToken = jwtDecode<DecodedJwtPayload>(token);

    // Проверяем срок действия токена
    if (decodedToken.exp) {
      const currentTime = Date.now() / 1000;
      if (decodedToken.exp < currentTime) {
        return false;
      }
    }

    // Проверяем наличие ID пользователя
    if (!decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]) {
      return false;
    }

    return true;
  } catch (e) {
    return false;
  }
}

onMounted(async () => {
  try {
    isLoadingProfile.value = true;
    error.value = '';

    // Получаем токен из cookie с помощью функции getToken из authService
    const token = auth.getToken();

    if (!token) {
      error.value = "Нет токена авторизации";
      router.push('/login');
      return;
    }

    // Проверяем валидность токена
    if (!validateToken(token)) {
      error.value = "Невалидный или истекший токен";
      // Удаляем невалидный токен
      auth.logout();
      router.push('/login');
      return;
    }

    // Декодируем JWT и получаем userId и email
    const decodedToken = jwtDecode<DecodedJwtPayload>(token);
    const userId = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    const email = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
    const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    if (!userId) {
      error.value = "ID пользователя не найден в токене";
      return;
    }

    // Сначала пытаемся получить данные по ID (работает для администраторов)
    let userDataLoaded = false;

    try {
      const userData = await getUserByIdFromApi(userId);

      user.value = {
        id: userData.id,
        firstName: userData.firstName || '',
        lastName: userData.lastName || '',
        email: userData.email,
        role: userData.role || 'User'
      };
      userDataLoaded = true;
    } catch (idError: any) {
      // Если есть email в токене, пробуем получить по email
      if (email) {
        try {
          const userDataByEmail = await getUserByEmail(email);

          user.value = {
            id: userDataByEmail.id,
            firstName: userDataByEmail.firstName || '',
            lastName: userDataByEmail.lastName || '',
            email: userDataByEmail.email,
            role: userDataByEmail.role || 'User'
          };
          userDataLoaded = true;
        } catch (emailError: any) {
          // Не пробрасываем эту ошибку дальше
        }
      }
    }

    // Если не удалось загрузить данные ни по ID, ни по email, используем данные из токена
    if (!userDataLoaded && email) {
      user.value = {
        id: userId,
        firstName: "Пользователь",
        lastName: "",
        email: email,
        role: role || "User"
      };
      userDataLoaded = true;
    }

    // Если все способы не сработали, показываем ошибку
    if (!userDataLoaded) {
      error.value = "Не удалось загрузить данные профиля. Пожалуйста, попробуйте войти снова.";
    }
  } catch (e: any) {
    error.value = "Произошла неизвестная ошибка при загрузке профиля.";
  } finally {
    isLoadingProfile.value = false;
  }
});

const handleTabChange = (tab: string) => {
  activeTab.value = tab;
};

const navigateToAdmin = () => {
  router.push('/admin');
};

const handleUserUpdated = (updatedUser: any) => {
  if (!user.value) return;

  user.value = {
    ...user.value,
    firstName: updatedUser.firstName,
    lastName: updatedUser.lastName,
    email: updatedUser.email
  };
};
</script>

<template>
  <div class="min-h-screen bg-gray-50 px-4 py-10">
    <!-- Скелетон загрузки -->
    <div v-if="isLoadingProfile" class="max-w-6xl mx-auto bg-white rounded-xl shadow-md p-6">
      <div class="flex flex-col md:flex-row gap-6 animate-pulse">
        <div class="md:w-64 space-y-4">
          <div class="h-36 bg-gray-200 rounded-lg"></div>
          <div class="h-10 bg-gray-200 rounded"></div>
          <div class="h-10 bg-gray-200 rounded"></div>
        </div>
        <div class="flex-1 min-w-[600px] space-y-6">
          <div class="h-8 bg-gray-200 rounded w-1/3"></div>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="h-20 bg-gray-200 rounded"></div>
            <div class="h-20 bg-gray-200 rounded"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Сообщение об ошибке -->
    <div v-else-if="error" class="max-w-6xl mx-auto bg-white rounded-xl shadow-md p-6">
      <div class="flex items-center">
        <div class="bg-red-100 rounded-full p-2 mr-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <div>
          <h3 class="text-lg font-medium text-red-800">Ошибка загрузки профиля</h3>
          <p class="text-sm text-red-700">{{ error }}</p>
          <button @click="router.push('/login')"
            class="mt-3 py-1 px-4 bg-red-600 hover:bg-red-700 text-white text-sm font-medium rounded-md transition">
            Вернуться на страницу входа
          </button>
        </div>
      </div>
    </div>

    <!-- Содержимое профиля -->
    <div v-else-if="user" class="max-w-6xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Ваш профиль</h1>
        <button v-if="user.role === 'Admin'" @click="navigateToAdmin"
          class="flex items-center gap-2 py-2 px-4 bg-violet-600 hover:bg-violet-700 text-white text-sm font-medium rounded-md transition">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          Панель администратора
        </button>
      </div>

      <div class="flex flex-col md:flex-row gap-8">
        <!-- Левая колонка -->
        <div class="md:w-64 space-y-6">
          <!-- Аватар и информация пользователя -->
          <ProfileHeader :user="user" />

          <!-- Навигационное меню -->
          <ProfileNavigation :active-tab="activeTab" @tab-change="handleTabChange" />
        </div>

        <!-- Правая колонка - основное содержимое с фиксированной шириной -->
        <div class="flex-1 min-w-[600px]">
          <div class="bg-white rounded-lg shadow-sm p-6 min-h-[450px] w-full">
            <!-- Личная информация -->
            <ProfilePersonalInfo v-if="activeTab === 'personal'" :user="user" @user-updated="handleUserUpdated" />

            <!-- Безопасность -->
            <ProfileSecurity v-if="activeTab === 'security'" :userId="user.id" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {

  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: .5;
  }
}
</style>
