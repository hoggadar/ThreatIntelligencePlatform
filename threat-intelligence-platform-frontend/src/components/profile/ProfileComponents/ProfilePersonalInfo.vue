<template>
  <div class="w-full">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-xl font-semibold text-gray-800">Личная информация</h2>
      <button v-if="!isEditing" @click="startEditing"
        class="flex items-center gap-2 text-blue-600 hover:text-blue-800 font-medium text-sm">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
        </svg>
        Редактировать
      </button>
    </div>

    <div v-if="successMessage" class="mb-6 p-3 bg-green-50 border border-green-200 text-green-700 rounded-lg">
      <div class="flex items-center">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
          stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
        </svg>
        {{ successMessage }}
      </div>
    </div>

    <div v-if="errorMessage" class="mb-6 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg">
      <div class="flex items-center">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
          stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        {{ errorMessage }}
      </div>
    </div>

    <!-- Режим просмотра профиля -->
    <div v-if="!isEditing" class="space-y-6 w-full">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 w-full">
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Имя</label>
          <div class="p-3 bg-gray-50 rounded-lg border border-gray-200 text-gray-800">
            {{ user.firstName || 'Не указано' }}
          </div>
        </div>
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Фамилия</label>
          <div class="p-3 bg-gray-50 rounded-lg border border-gray-200 text-gray-800">
            {{ user.lastName || 'Не указано' }}
          </div>
        </div>
      </div>
      <div class="space-y-2">
        <label class="block text-sm text-gray-700">Email</label>
        <div class="p-3 bg-gray-50 rounded-lg border border-gray-200 text-gray-800">
          {{ user.email }}
        </div>
      </div>
    </div>

    <!-- Режим редактирования профиля -->
    <div v-else class="space-y-6 w-full">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Имя</label>
          <input v-model="editedUser.firstName" type="text"
            class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2"
            :class="firstNameError ? 'border-red-300 focus:ring-red-500' : 'border-gray-300 focus:ring-blue-500'"
            placeholder="Введите имя" />
          <p v-if="firstNameError" class="text-sm text-red-600 mt-1">{{ firstNameError }}</p>
        </div>
        <div class="space-y-2">
          <label class="block text-sm text-gray-700">Фамилия</label>
          <input v-model="editedUser.lastName" type="text"
            class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2"
            :class="lastNameError ? 'border-red-300 focus:ring-red-500' : 'border-gray-300 focus:ring-blue-500'"
            placeholder="Введите фамилию" />
          <p v-if="lastNameError" class="text-sm text-red-600 mt-1">{{ lastNameError }}</p>
        </div>
      </div>
      <div class="space-y-2">
        <label class="block text-sm text-gray-700">Email</label>
        <input v-model="editedUser.email" type="email"
          class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2"
          :class="emailError ? 'border-red-300 focus:ring-red-500' : 'border-gray-300 focus:ring-blue-500'"
          placeholder="Введите email" />
        <p v-if="emailError" class="text-sm text-red-600 mt-1">{{ emailError }}</p>
      </div>

      <div class="flex gap-3 pt-4">
        <button @click="saveProfile"
          class="py-2 px-4 text-white rounded transition focus:outline-none focus:ring-2 focus:ring-blue-500"
          :class="hasFormErrors ? 'bg-gray-400 cursor-not-allowed' : 'bg-blue-600 hover:bg-blue-700'"
          :disabled="hasFormErrors">
          Сохранить
        </button>
        <button @click="cancelEditing"
          class="py-2 px-4 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 transition focus:outline-none focus:ring-2 focus:ring-gray-400">
          Отмена
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { updateUserViaApi } from '../../../services/userService';

// Определяем входные параметры
const props = defineProps<{
  user: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: string;
  }
}>();

// Определяем события
const emit = defineEmits<{
  'user-updated': [user: any];
}>();

const isEditing = ref(false);
const editedUser = ref({ firstName: '', lastName: '', email: '' });
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

// Состояния валидации полей
const firstNameError = ref<string | null>(null);
const lastNameError = ref<string | null>(null);
const emailError = ref<string | null>(null);

// Наблюдаем за изменениями в полях формы для мгновенной валидации
watch(() => editedUser.value.firstName, (newVal) => {
  if (newVal.length > 20) {
    firstNameError.value = 'Имя не должно превышать 20 букв';
  } else {
    firstNameError.value = null;
  }
});

watch(() => editedUser.value.lastName, (newVal) => {
  if (newVal.length > 20) {
    lastNameError.value = 'Фамилия не должна превышать 20 букв';
  } else {
    lastNameError.value = null;
  }
});

watch(() => editedUser.value.email, (newVal) => {
  if (!isValidEmail(newVal) && newVal.length > 0) {
    emailError.value = 'Введите корректный email адрес';
  } else {
    emailError.value = null;
  }
});

// Проверка наличия ошибок в форме
const hasFormErrors = computed(() => {
  return !!(firstNameError.value || lastNameError.value || emailError.value);
});

/**
 * Проверяет корректность email адреса
 */
const isValidEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

/**
 * Валидирует данные формы перед сохранением
 */
const validateForm = (): string | null => {
  // Сначала проверяем каждое поле отдельно для обновления состояний ошибок
  if (editedUser.value.firstName.length > 20) {
    firstNameError.value = 'Имя не должно превышать 20 букв';
    return firstNameError.value;
  }

  if (editedUser.value.lastName.length > 20) {
    lastNameError.value = 'Фамилия не должна превышать 20 букв';
    return lastNameError.value;
  }

  if (!isValidEmail(editedUser.value.email)) {
    emailError.value = 'Введите корректный email адрес';
    return emailError.value;
  }

  return null;
};

const startEditing = () => {
  isEditing.value = true;
  errorMessage.value = null;
  successMessage.value = null;
  if (props.user) {
    editedUser.value = {
      firstName: props.user.firstName,
      lastName: props.user.lastName,
      email: props.user.email
    };
  }
};

const cancelEditing = () => {
  isEditing.value = false;
  errorMessage.value = null;
};

const saveProfile = async () => {
  if (!props.user) return;

  // Очищаем предыдущие сообщения об ошибках
  errorMessage.value = null;

  // Валидация данных перед отправкой
  const validationError = validateForm();
  if (validationError) {
    errorMessage.value = validationError;
    return;
  }

  try {
    // Используем API-метод обновления пользователя
    const updatedUser = await updateUserViaApi(props.user.id, editedUser.value);

    // Уведомляем родительский компонент об обновленных данных
    emit('user-updated', updatedUser);

    successMessage.value = 'Профиль успешно обновлен';
    isEditing.value = false;
  } catch (error: any) {
    errorMessage.value = error.message || 'Ошибка при обновлении профиля';
  }
};
</script>