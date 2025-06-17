<template>
  <div class="flex flex-col items-center p-6 bg-white rounded-lg shadow-sm">
    <div
      class="h-32 w-32 rounded-full bg-gradient-to-r from-blue-500 to-violet-600 flex items-center justify-center mb-4 text-white text-4xl font-medium">
      {{ getInitials() }}
    </div>
    <h2 class="text-xl font-bold text-center">
      {{ user.firstName || user.lastName ? `${user.firstName} ${user.lastName}` : user.email }}
    </h2>
    <span class="inline-flex items-center px-3 py-1 rounded-full text-sm mt-2"
      :class="user.role === 'Admin' ? 'bg-violet-100 text-violet-800' : 'bg-blue-100 text-blue-800'">
      {{ user.role === 'Admin' ? 'Администратор' : 'Пользователь' }}
    </span>
  </div>
</template>

<script setup lang="ts">
// Описываем ожидаемые параметры компонента
const props = defineProps<{
  user: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: string;
  }
}>();

// Функция для получения инициалов из имени пользователя
const getInitials = () => {
  if (!props.user) return '';

  const firstName = props.user.firstName || '';
  const lastName = props.user.lastName || '';

  if (!firstName && !lastName) return props.user.email?.charAt(0).toUpperCase() || '?';

  return (firstName.charAt(0) + (lastName ? lastName.charAt(0) : '')).toUpperCase();
};
</script>