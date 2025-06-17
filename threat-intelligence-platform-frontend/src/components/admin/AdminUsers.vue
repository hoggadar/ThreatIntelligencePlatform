<template>
  <div>
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-xl font-semibold">Управление пользователями</h2>
      <button @click="openCreateUserModal"
        class="px-3 py-2 bg-green-600 text-white rounded hover:bg-green-700 flex items-center gap-1">
        Создать пользователя
      </button>
    </div>

    <!-- Строка поиска -->
    <div class="flex justify-between items-center mb-2">
      <div>
        <span class="mr-2 text-gray-700 text-sm">Показать</span>
        <select v-model.number="pagination.pageSize" @change="onUserLimitChange"
          class="border rounded px-2 py-1 text-sm">
          <option v-for="opt in [10, 25, 50, 100]" :key="opt" :value="opt">{{ opt }}</option>
        </select>
        <span class="ml-2 text-gray-700 text-sm">записей</span>
      </div>
      <div class="flex items-center justify-center">
        <span class="mr-2 text-gray-700 text-sm">Поиск:</span>
        <div class="relative">
          <input v-model="searchQuery" type="text" class="border rounded px-2 py-1 text-sm text-center pr-7"
            placeholder="Поиск..." />
          <button v-if="searchQuery" @click="clearSearch"
            class="absolute right-1 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-700">
            ×
          </button>
        </div>
      </div>
    </div>

    <!-- Таблица пользователей -->
    <div class="border rounded-md overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-2 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider w-10">#</th>
            <th class="px-6 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider">Имя</th>
            <th class="px-6 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider hidden sm:table-cell">
              Email</th>
            <th class="px-6 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider hidden md:table-cell">
              Роль</th>
            <th class="px-6 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="(user, index) in filteredUsers" :key="user.id" class="hover:bg-gray-50">
            <td class="px-2 py-4 whitespace-nowrap text-center text-sm text-gray-600 font-medium w-10">
              {{ calculateUserNumber(index) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap ">
              <div class="flex items-center">
                <div class="ml-2">
                  <div class="text-sm font-medium text-gray-900">
                    {{ `${user.firstName || ''} ${user.lastName || ''}` }}
                    <span v-if="!user.firstName && !user.lastName" class="text-gray-500 italic">Не указано</span>
                  </div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap hidden sm:table-cell">
              <div class="text-sm text-gray-900">{{ user.email }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap hidden md:table-cell">
              <span v-for="role in user.roles" :key="role"
                class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full mr-1"
                :class="role === 'Admin' ? 'bg-purple-100 text-purple-800' : 'bg-blue-100 text-blue-800'">
                {{ role }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
              <div class="flex justify-end gap-2">
                <button @click="viewUserDetails(user)" class="text-gray-600 hover:text-gray-900 relative group">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  <span
                    class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white text-xs px-2 py-1 rounded opacity-0 group-hover:opacity-100 whitespace-nowrap transition-opacity duration-200">
                    Просмотреть детали
                  </span>
                </button>
                <button @click="editUser(user)" class="text-blue-600 hover:text-blue-900 relative group">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                  <span
                    class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white text-xs px-2 py-1 rounded opacity-0 group-hover:opacity-100 whitespace-nowrap transition-opacity duration-200">
                    Редактировать пользователя
                  </span>
                </button>
                <button @click="openRoleManager(user)" class="text-green-600 hover:text-green-900 relative group">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                  </svg>
                  <span
                    class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white text-xs px-2 py-1 rounded opacity-0 group-hover:opacity-100 whitespace-nowrap transition-opacity duration-200">
                    Управление ролью
                  </span>
                </button>
                <button @click="confirmDeleteUser(user)" class="text-red-600 hover:text-red-900 relative group">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                  <span
                    class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white text-xs px-2 py-1 rounded opacity-0 group-hover:opacity-100 whitespace-nowrap transition-opacity duration-200">
                    Удалить пользователя
                  </span>
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="filteredUsers.length === 0">
            <td :colspan="columnsCount" class="px-6 py-4 text-center text-sm text-gray-500 w-full">
              {{ loading ? 'Загрузка пользователей...' : 'Пользователи не найдены' }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Пагинация -->
    <div v-if="hasPagination" class="flex justify-center mt-4">
      <div class="flex space-x-1">
        <button @click="changePage(pagination.pageIndex - 1)" :disabled="pagination.pageIndex === 1"
          :class="{ 'bg-gray-200 text-gray-500': pagination.pageIndex === 1, 'bg-gray-100 hover:bg-gray-200': pagination.pageIndex !== 1 }"
          class="px-3 py-1 rounded">
          &laquo;
        </button>

        <template v-for="page in paginationPages" :key="page">
          <button @click="changePage(page)"
            :class="page === pagination.pageIndex ? 'bg-blue-600 text-white' : 'bg-gray-100 hover:bg-gray-200'"
            class="px-3 py-1 rounded">
            {{ page }}
          </button>
        </template>

        <button @click="changePage(pagination.pageIndex + 1)" :disabled="pagination.pageIndex === totalPages"
          :class="{ 'bg-gray-200 text-gray-500': pagination.pageIndex === totalPages, 'bg-gray-100 hover:bg-gray-200': pagination.pageIndex !== totalPages }"
          class="px-3 py-1 rounded">
          &raquo;
        </button>
      </div>
    </div>

    <!-- Модальное окно создания/редактирования пользователя -->
    <div v-if="showModal" class="fixed inset-0 flex items-center justify-center z-50">
      <div class="absolute inset-0 bg-black opacity-50" @click="closeModal"></div>
      <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-lg">
        <h3 class="text-lg font-semibold mb-4">{{ editingUser ? 'Редактировать пользователя' : 'Создать пользователя' }}
        </h3>

        <div class="mb-4 grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Имя</label>
            <input v-model="userForm.firstName" type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Имя пользователя" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Фамилия</label>
            <input v-model="userForm.lastName" type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Фамилия пользователя" />
          </div>
        </div>

        <div class="mb-4">
          <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
          <input v-model="userForm.email" type="email"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Email пользователя" />
        </div>

        <div class="mb-4" v-if="!editingUser">
          <label class="block text-sm font-medium text-gray-700 mb-1">Пароль</label>
          <input v-model="userForm.password" type="password"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Пароль пользователя" />
        </div>

        <div class="flex justify-end gap-2 mt-6">
          <button @click="closeModal" class="px-4 py-2 text-gray-700 bg-gray-200 rounded hover:bg-gray-300">
            Отмена
          </button>
          <button @click="saveUser" class="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700"
            :disabled="saving">
            {{ saving ? 'Сохранение...' : 'Сохранить' }}
          </button>
        </div>

        <div v-if="formError" class="mt-3 text-sm text-red-600">
          {{ formError }}
        </div>
      </div>
    </div>

    <!-- Модальное окно управления ролями -->
    <div v-if="showRoleModal" class="fixed inset-0 flex items-center justify-center z-50">
      <div class="absolute inset-0 bg-black opacity-50" @click="closeRoleModal"></div>
      <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-md">
        <h3 class="text-lg font-semibold mb-4">Управление ролями</h3>
        <p class="text-gray-600 mb-4 text-center">
          Пользователь: <strong>{{ roleManagement.user?.firstName }} {{ roleManagement.user?.lastName }}</strong> ({{
            roleManagement.user?.email }})
        </p>

        <div class="mb-4">
          <p class="text-sm text-gray-600 mb-2 text-center">Выберите роль для пользователя:</p>
          <div class="space-y-3">
            <label class="flex items-center p-3 border rounded-md"
              :class="roleManagement.selectedRole === 'Admin' ? 'border-purple-500 bg-purple-50' : 'border-gray-300 hover:bg-gray-50'">
              <input type="radio" name="role" value="Admin" v-model="roleManagement.selectedRole" class="mr-2 h-4 w-4"
                :disabled="roleManagement.processing" />
              <div class="w-full">
                <span class="font-medium">Администратор</span>
                <p class="text-sm text-gray-500 mt-1 text-center">Полный доступ к управлению системой</p>
              </div>
            </label>
            <label class="flex items-center p-3 border rounded-md"
              :class="roleManagement.selectedRole === 'User' ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:bg-gray-50'">
              <input type="radio" name="role" value="User" v-model="roleManagement.selectedRole" class="mr-2 h-4 w-4"
                :disabled="roleManagement.processing" />
              <div class="w-full">
                <span class="font-medium">Пользователь</span>
                <p class="text-sm text-gray-500 mt-1 text-center">Стандартные права доступа</p>
              </div>
            </label>
          </div>
        </div>

        <div class="flex justify-end gap-2 mt-6">
          <button @click="closeRoleModal" class="px-4 py-2 text-gray-700 bg-gray-200 rounded hover:bg-gray-300">
            Отмена
          </button>
          <button @click="saveUserRole" class="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700"
            :disabled="roleManagement.processing">
            {{ roleManagement.processing ? 'Сохранение...' : 'Сохранить' }}
          </button>
        </div>

        <div v-if="roleManagement.error" class="mt-3 text-sm text-red-600">
          {{ roleManagement.error }}
        </div>
      </div>
    </div>

    <!-- Модальное окно подтверждения удаления -->
    <div v-if="showDeleteConfirm" class="fixed inset-0 flex items-center justify-center z-50">
      <div class="absolute inset-0 bg-black opacity-50" @click="closeDeleteConfirm"></div>
      <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-md">
        <h3 class="text-lg font-semibold mb-4">Подтверждение удаления</h3>
        <p class="text-gray-600 mb-6">Вы действительно хотите удалить пользователя <strong>{{ userToDelete?.email
            }}</strong>? Это действие нельзя отменить.</p>

        <div class="flex justify-end gap-2">
          <button @click="closeDeleteConfirm" class="px-4 py-2 text-gray-700 bg-gray-200 rounded hover:bg-gray-300">
            Отмена
          </button>
          <button @click="deleteSelectedUser" class="px-4 py-2 text-white bg-red-600 rounded hover:bg-red-700"
            :disabled="deleting">
            {{ deleting ? 'Удаление...' : 'Удалить' }}
          </button>
        </div>

        <div v-if="deleteError" class="mt-3 text-sm text-red-600">
          {{ deleteError }}
        </div>
      </div>
    </div>

    <!-- Модальное окно просмотра деталей пользователя -->
    <UserDetailsModal :userId="selectedUserId" :show="showUserDetails" @update:show="showUserDetails = $event"
      @close="showUserDetails = false" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, onBeforeUnmount } from 'vue';
import {
  UserDTO,
  UserListResponse,
  getAllUsers,
  getUserById,
  createUser as createApiUser,
  updateUser as updateApiUser,
  deleteUser as deleteApiUser,
  getUserRoles,
  addToRole,
  removeFromRole,
  getAllRoles,
  RoleDTO
} from '../../services/adminUserService';
import UserDetailsModal from './UserDetailsModal.vue';

// Состояние пользователей
const users = ref<UserDTO[]>([]);
const loading = ref(true);
const searchQuery = ref('');
let searchTimeout: ReturnType<typeof setTimeout> | null = null;

// Состояние пагинации
const pagination = ref<{
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}>({
  pageIndex: 1,
  pageSize: 10,
  totalPages: 1,
  totalCount: 0
});

// Состояние модалки для создания/редактирования
const showModal = ref(false);
const editingUser = ref<UserDTO | null>(null);
const userForm = ref<{
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}>({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
});
const saving = ref(false);
const formError = ref<string | null>(null);

// Состояние модалки для управления ролями
const showRoleModal = ref(false);
const roleManagement = ref({
  user: null as UserDTO | null,
  roles: [] as string[],
  selectedRole: '',
  availableRoles: [] as RoleDTO[],
  processing: false,
  error: null as string | null,
});

// Состояние модалки для удаления
const showDeleteConfirm = ref(false);
const userToDelete = ref<UserDTO | null>(null);
const deleting = ref(false);
const deleteError = ref<string | null>(null);

// Добавим состояние для просмотра деталей пользователя
const showUserDetails = ref(false);
const selectedUserId = ref('');

// Управление скроллом body при открытии модальных окон
const disableBodyScroll = () => {
  document.body.classList.add('overflow-hidden');
};

const enableBodyScroll = () => {
  document.body.classList.remove('overflow-hidden');
};

// Фильтрованный список пользователей
const filteredUsers = computed(() => {
  const query = searchQuery.value.toLowerCase().trim();
  if (!query) return users.value;

  return users.value.filter(user =>
    (user.firstName?.toLowerCase().includes(query) || false) ||
    (user.lastName?.toLowerCase().includes(query) || false) ||
    user.email.toLowerCase().includes(query) ||
    (user.roles && user.roles.some(role => role.toLowerCase().includes(query)))
  );
});

// Генерация страниц для пагинации
const totalPages = computed(() => {
  // Если totalCount == 0, но есть пользователи, используем длину массива
  const count = pagination.value.totalCount > 0 ? pagination.value.totalCount : users.value.length;
  return count > 0 ? Math.ceil(count / pagination.value.pageSize) : 1;
});

const hasPagination = computed(() => {
  const count = pagination.value.totalCount > 0 ? pagination.value.totalCount : users.value.length;
  return count > 0 && totalPages.value > 1;
});

const paginationPages = computed(() => {
  const total = totalPages.value;
  const current = pagination.value.pageIndex;
  const items: (number | string)[] = [];

  if (total <= 5) {
    for (let i = 1; i <= total; i++) items.push(i);
  } else if (current <= 3) {
    items.push(1, 2, 3, 4, 5, '...', total);
  } else if (current >= total - 2) {
    items.push(1, '...', total - 4, total - 3, total - 2, total - 1, total);
  } else {
    items.push(1, '...', current - 1, current, current + 1, '...', total);
  }
  return items;
});

// Загрузка пользователей
const fetchUsers = async () => {
  loading.value = true;
  try {
    const response = await getAllUsers(pagination.value.pageIndex, pagination.value.pageSize);
    users.value = response.items;
    pagination.value = {
      pageIndex: response.pageIndex,
      totalPages: response.totalPages,
      totalCount: response.totalCount,
      pageSize: response.pageSize
    };
  } catch (error: any) {
    console.error('Ошибка при загрузке пользователей:', error);
  } finally {
    loading.value = false;
  }
};

// Изменение страницы
const changePage = (page: number | string) => {
  // Пропустить переход на '...'
  if (typeof page === 'string') return;

  if (page < 1 || page > pagination.value.totalPages || page === pagination.value.pageIndex) {
    return;
  }
  pagination.value.pageIndex = page;
};

// Следим за изменением страницы и обновляем данные
watch(() => pagination.value.pageIndex, () => {
  fetchUsers();
});

// Функции для работы с модальными окнами
const openCreateUserModal = () => {
  disableBodyScroll();
  editingUser.value = null;
  userForm.value = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
  };
  formError.value = null;
  showModal.value = true;
};

const editUser = (user: UserDTO) => {
  disableBodyScroll();
  editingUser.value = user;
  userForm.value = {
    firstName: user.firstName || '',
    lastName: user.lastName || '',
    email: user.email,
    password: '', // при редактировании пароль не меняем
  };
  formError.value = null;
  showModal.value = true;
};

const closeModal = () => {
  enableBodyScroll();
  showModal.value = false;
  editingUser.value = null;
  formError.value = null;
};

const openRoleManager = async (user: UserDTO) => {
  disableBodyScroll();
  roleManagement.value = {
    user,
    roles: user.roles || [],
    selectedRole: (user.roles && user.roles.length > 0) ? user.roles[0] : '',
    availableRoles: [],
    processing: false,
    error: null,
  };
  showRoleModal.value = true;

  // Загружаем доступные роли
  try {
    const rolesResponse = await getAllRoles();
    roleManagement.value.availableRoles = rolesResponse.items;
  } catch (error) {
    // Если не удалось загрузить роли, используем базовые Admin и User
    roleManagement.value.availableRoles = [
      { id: '1', name: 'Admin', description: 'Администратор системы' },
      { id: '2', name: 'User', description: 'Обычный пользователь' }
    ];
  }
};

const closeRoleModal = () => {
  enableBodyScroll();
  showRoleModal.value = false;
  roleManagement.value = {
    user: null,
    roles: [],
    selectedRole: '',
    availableRoles: [],
    processing: false,
    error: null,
  };
};

const confirmDeleteUser = (user: UserDTO) => {
  disableBodyScroll();
  userToDelete.value = user;
  deleteError.value = null;
  showDeleteConfirm.value = true;
};

const closeDeleteConfirm = () => {
  enableBodyScroll();
  showDeleteConfirm.value = false;
  userToDelete.value = null;
  deleteError.value = null;
};

const viewUserDetails = (user: UserDTO) => {
  selectedUserId.value = user.id;
  showUserDetails.value = true;
  // Скролл блокируется в компоненте UserDetailsModal
};

// Сохранение пользователя (создание или редактирование)
const saveUser = async () => {
  saving.value = true;
  formError.value = null;

  try {
    // Валидация
    if (!userForm.value.email) {
      throw new Error('Email обязателен');
    }

    if (!editingUser.value && !userForm.value.password) {
      throw new Error('Пароль обязателен при создании пользователя');
    }

    if (editingUser.value) {
      // Обновление существующего пользователя
      const updatedUser = await updateApiUser(editingUser.value.id, {
        firstName: userForm.value.firstName,
        lastName: userForm.value.lastName,
        email: userForm.value.email,
      });

      // Обновляем список
      const index = users.value.findIndex(u => u.id === updatedUser.id);
      if (index !== -1) {
        users.value[index] = updatedUser;
      }
    } else {
      // Создание нового пользователя
      const newUser = await createApiUser({
        firstName: userForm.value.firstName,
        lastName: userForm.value.lastName,
        email: userForm.value.email,
        password: userForm.value.password,
      });

      // Добавляем в список, если пользователь должен быть на текущей странице
      if (users.value.length < pagination.value.pageSize) {
        users.value.push(newUser);
      } else {
        // Если список полный, перезагружаем данные
        await fetchUsers();
      }
    }

    closeModal();
  } catch (error: any) {
    formError.value = error.message || 'Ошибка при сохранении пользователя';
  } finally {
    saving.value = false;
  }
};

// Сохранение роли пользователя
const saveUserRole = async () => {
  if (!roleManagement.value.user || !roleManagement.value.selectedRole) return;

  roleManagement.value.processing = true;
  roleManagement.value.error = null;

  try {
    const userId = roleManagement.value.user.id;
    const newRole = roleManagement.value.selectedRole;
    const currentRoles = roleManagement.value.user.roles || [];

    // Удаляем все текущие роли
    for (const role of currentRoles) {
      await removeFromRole(userId, role);
    }

    // Добавляем новую роль
    await addToRole(userId, newRole);

    // Обновляем пользователя в списке
    const userIndex = users.value.findIndex(u => u.id === userId);
    if (userIndex !== -1) {
      users.value[userIndex].roles = [newRole];

      // Обновляем и локальное представление пользователя
      if (roleManagement.value.user) {
        roleManagement.value.user.roles = [newRole];
      }
    }

    // Закрываем модальное окно
    closeRoleModal();
  } catch (error: any) {
    roleManagement.value.error = error.message || 'Ошибка при изменении роли';
  } finally {
    roleManagement.value.processing = false;
  }
};

// Удаление пользователя
const deleteSelectedUser = async () => {
  if (!userToDelete.value) return;

  deleting.value = true;
  deleteError.value = null;

  try {
    await deleteApiUser(userToDelete.value.id);

    // Удаляем пользователя из списка
    users.value = users.value.filter(u => u.id !== userToDelete.value!.id);

    // Если после удаления список пуст, но есть предыдущие страницы, загружаем предыдущую
    if (users.value.length === 0 && pagination.value.pageIndex > 1) {
      pagination.value.pageIndex--;
      await fetchUsers();
    } else if (pagination.value.totalCount > 0) {
      // Обновляем общее количество
      pagination.value.totalCount--;

      // Пересчитываем общее количество страниц
      pagination.value.totalPages = Math.ceil(pagination.value.totalCount / pagination.value.pageSize);

      // Если текущая страница больше общего количества, переходим на последнюю
      if (pagination.value.pageIndex > pagination.value.totalPages && pagination.value.totalPages > 0) {
        pagination.value.pageIndex = pagination.value.totalPages;
        await fetchUsers();
      }
    }

    closeDeleteConfirm();
  } catch (error: any) {
    deleteError.value = error.message || 'Ошибка при удалении пользователя';
  } finally {
    deleting.value = false;
  }
};

// Функция для расчета номера пользователя с учетом пагинации
const calculateUserNumber = (index: number): number => {
  return (pagination.value.pageIndex - 1) * pagination.value.pageSize + index + 1;
};

const onUserLimitChange = () => {
  pagination.value.pageIndex = 1;
  pagination.value.pageSize = Number(pagination.value.pageSize);
  fetchUsers();
};

// Загрузка данных при монтировании компонента
onMounted(() => {
  fetchUsers();
});

// На всякий случай включаем скролл, если компонент удаляется
onBeforeUnmount(() => {
  enableBodyScroll();
});

// debounce-поиск
watch(searchQuery, (val) => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pagination.value.pageIndex = 1;
    fetchUsers();
  }, 400);
});

const clearSearch = () => {
  searchQuery.value = '';
  pagination.value.pageIndex = 1;
  fetchUsers();
};

const columnsCount = 5;
</script>

<style>
body.overflow-hidden {
  overflow: hidden;
}
</style>