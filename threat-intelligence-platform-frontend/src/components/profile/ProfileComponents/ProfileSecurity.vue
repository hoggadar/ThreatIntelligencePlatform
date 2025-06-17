<template>
  <div class="w-full">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-xl font-semibold text-gray-800">Безопасность</h2>
      <button v-if="!showPasswordForm" @click="showPasswordForm = true"
        class="flex items-center gap-2 text-blue-600 hover:text-blue-800 font-medium text-sm">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
        </svg>
        Изменить пароль
      </button>
    </div>

    <div v-if="passwordSuccess" class="mb-6 p-3 bg-green-50 border border-green-200 text-green-700 rounded-lg">
      <div class="flex items-center">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
          stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
        </svg>
        {{ passwordSuccess }}
      </div>
    </div>

    <!-- Форма смены пароля -->
    <div v-if="showPasswordForm" class="space-y-6 w-full border border-gray-200 rounded-lg p-6 bg-gray-50">
      <h3 class="text-lg font-medium text-gray-800">Смена пароля</h3>

      <div v-if="passwordError" class="p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg">
        <div class="flex items-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          {{ passwordError }}
        </div>
      </div>

      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Текущий пароль</label>
          <input v-model="passwordForm.currentPassword" type="password"
            class="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Введите текущий пароль" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Новый пароль</label>
          <input v-model="passwordForm.newPassword" type="password"
            class="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Введите новый пароль" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Подтверждение пароля</label>
          <input v-model="passwordForm.confirmPassword" type="password"
            class="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Повторите новый пароль" />
        </div>
      </div>

      <div class="flex gap-3 pt-2">
        <button @click="changePassword"
          class="py-2 px-4 bg-blue-600 text-white rounded hover:bg-blue-700 transition focus:outline-none focus:ring-2 focus:ring-blue-500">
          Сменить пароль
        </button>
        <button @click="showPasswordForm = false"
          class="py-2 px-4 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 transition focus:outline-none focus:ring-2 focus:ring-gray-400">
          Отмена
        </button>
      </div>
    </div>

    <!-- Информация о безопасности -->
    <div v-else class="space-y-6 w-full">
      <div class="space-y-2">
        <div class="p-3 bg-gray-50 rounded-lg border border-gray-200">
          <div>
            <h4 class="font-medium text-gray-700">Пароль</h4>
            <p class="text-sm text-gray-500">Последнее обновление пароля: неизвестно</p>
          </div>
        </div>
      </div>

      <div class="space-y-2">
        <div class="p-3 bg-gray-50 rounded-lg border border-gray-200">
          <h4 class="font-medium text-gray-700 mb-1">Двухфакторная аутентификация</h4>
          <p class="text-sm text-gray-500 mb-2">
            Повысьте безопасность аккаунта, добавив дополнительный слой защиты.
          </p>
          <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
            Не активировано
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

// Определяем входные параметры
const props = defineProps<{
  userId: string;
}>();

// Определяем события
const emit = defineEmits<{
  'password-changed': [success: boolean];
}>();

const showPasswordForm = ref(false);
const passwordForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
});
const passwordError = ref<string | null>(null);
const passwordSuccess = ref<string | null>(null);

const changePassword = async () => {
  passwordError.value = null;

  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    passwordError.value = 'Пароли не совпадают';
    return;
  }

  if (passwordForm.value.newPassword.length < 6) {
    passwordError.value = 'Пароль должен содержать не менее 6 символов';
    return;
  }

  try {
    // Здесь должен быть запрос к API для смены пароля
    // await changePasswordApi(props.userId, passwordForm.value);

    // Временная эмуляция успешного ответа
    passwordSuccess.value = 'Пароль успешно обновлен';
    passwordForm.value = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    };

    // Уведомляем родительский компонент об успешной смене пароля
    emit('password-changed', true);

    setTimeout(() => {
      passwordSuccess.value = null;
      showPasswordForm.value = false;
    }, 2000);
  } catch (error: any) {
    passwordError.value = error.message || 'Ошибка при смене пароля';
  }
};
</script>