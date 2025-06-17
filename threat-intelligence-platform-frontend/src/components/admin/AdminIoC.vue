<template>
  <div>
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-xl font-semibold">Управление индикаторами компрометации</h2>
      <div class="text-gray-600 text-sm">
        Всего индикаторов: <span class="font-medium">{{ totalCount }}</span>
      </div>
    </div>

    <!-- Статистика -->
    <div class="mb-6 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4">
      <!-- Статистика по типам -->
      <div class="bg-white rounded-lg shadow p-4">
        <h3 class="text-sm font-medium text-gray-700 mb-2">Распределение по типам</h3>
        <div class="space-y-2">
          <div v-for="(count, type) in sortedTypeStats" :key="type" class="flex justify-between">
            <div class="flex items-center">
              <span class="inline-block w-3 h-3 rounded-full mr-2" :class="getTypeColorClass(type)"></span>
              <span class="text-sm">{{ type }}</span>
            </div>
            <span class="text-sm font-medium">{{ count }}</span>
          </div>
        </div>
      </div>

      <!-- Статистика по источникам -->
      <div class="bg-white rounded-lg shadow p-4 sm:col-span-1 lg:col-span-2">
        <h3 class="text-sm font-medium text-gray-700 mb-2">Источники данных</h3>
        <div class="space-y-2">
          <div v-for="(count, source) in sortedSourceStats" :key="source" class="flex justify-between">
            <span class="text-sm truncate max-w-[65%]">{{ source }}</span>
            <span class="text-sm font-medium">{{ count }}</span>
          </div>
        </div>
      </div>

      <!-- Фильтры -->
      <div class="bg-white rounded-lg shadow p-4 sm:col-span-2">
        <h3 class="text-sm font-medium text-gray-700 mb-2">Фильтры</h3>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label for="ioc-type-filter" class="block text-xs text-gray-600 mb-1">Тип</label>
            <select id="ioc-type-filter" v-model="filters.type"
              class="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Все типы</option>
              <option v-for="type in availableTypes" :key="type" :value="type">{{ type }}</option>
            </select>
          </div>
          <div>
            <label for="ioc-source-filter" class="block text-xs text-gray-600 mb-1">Источник</label>
            <select id="ioc-source-filter" v-model="filters.source"
              class="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Все источники</option>
              <option v-for="source in availableSources" :key="source" :value="source">{{ source }}</option>
            </select>
          </div>
        </div>

        <div class="flex justify-end mt-3">
          <button @click="applyFilters" class="px-3 py-1 bg-blue-600 text-white text-sm rounded hover:bg-blue-700">
            Применить фильтры
          </button>
        </div>
      </div>
    </div>

    <!-- Строка поиска -->
    <div class="flex justify-between items-center mb-2">
      <div>
        <span class="mr-2 text-gray-700 text-sm">Показать</span>
        <select v-model="paginationLimit" @change="onLimitChange" class="border rounded px-2 py-1 text-sm">
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

    <!-- Таблица IoC -->
    <div class="border rounded-md overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-2 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider w-10">#</th>
            <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider">Тип</th>
            <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider">Значение</th>
            <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider hidden md:table-cell">
              Источник</th>
            <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider hidden lg:table-cell">
              Теги</th>
            <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="(ioc, index) in iocs" :key="ioc.id" class="hover:bg-gray-50">
            <td class="px-2 py-4 whitespace-nowrap text-center text-sm text-gray-600 font-medium w-10">
              {{ calculateIoCIndex(index) }}
            </td>
            <td class="px-4 py-4 whitespace-nowrap">
              <span class="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full"
                :class="getTypeClass(ioc.type)">
                {{ ioc.type }}
              </span>
            </td>
            <td class="px-4 py-4">
              <div class="text-sm text-gray-900 truncate max-w-xs">{{ ioc.value }}</div>
            </td>
            <td class="px-4 py-4 whitespace-nowrap hidden md:table-cell">
              <div class="text-sm text-gray-900">{{ ioc.source }}</div>
            </td>
            <td class="px-4 py-4 whitespace-nowrap hidden lg:table-cell">
              <div class="flex flex-wrap gap-1">
                <span v-for="tag in ioc.tags" :key="tag" class="px-1.5 py-0.5 text-xs rounded-md"
                  :class="tag ? 'bg-gray-100 text-gray-800' : 'bg-transparent'">
                  {{ tag || '—' }}
                </span>
                <span v-if="!ioc.tags || ioc.tags.length === 0 || (ioc.tags.length === 1 && !ioc.tags[0])"
                  class="text-gray-400 text-xs">Нет тегов</span>
              </div>
            </td>
            <td class="px-4 py-4 whitespace-nowrap text-sm">
              <div class="flex gap-2">
                <button @click="viewIoCDetails(ioc)" class="text-blue-600 hover:text-blue-900 relative group">
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
                <button @click="confirmDeleteIoC(ioc)" class="text-red-600 hover:text-red-900 relative group">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                  <span
                    class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white text-xs px-2 py-1 rounded opacity-0 group-hover:opacity-100 whitespace-nowrap transition-opacity duration-200">
                    Удалить индикатор
                  </span>
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="iocs.length === 0">
            <td colspan="6" class="px-4 py-4 text-center text-sm text-gray-500">
              {{ loading ? 'Загрузка индикаторов...' : 'Индикаторы не найдены' }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Пагинация и надпись -->
    <div v-if="totalCount > 0"
      class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 mt-4">
      <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
        <div>
          <p class="text-sm text-gray-700">
            Показаны записи
            <span class="font-medium">{{ totalCount > 0 ? paginationOffset + 1 : 0 }}</span> -
            <span class="font-medium">{{ totalCount > 0 ? Math.min(paginationOffset + iocs.length, totalCount) : 0
              }}</span>
            из <span class="font-medium">{{ totalCount }}</span>
          </p>
        </div>
        <div v-if="hasPagination">
          <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
            <button @click="prevPage" :disabled="paginationOffset === 0"
              class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
              :class="paginationOffset === 0 ? 'opacity-50 cursor-not-allowed' : ''">
              <span class="sr-only">Previous</span>
              <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fill-rule="evenodd"
                  d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z"
                  clip-rule="evenodd" />
              </svg>
            </button>

            <!-- Кнопки с номерами страниц -->
            <template v-for="page in paginationItems" :key="page">
              <button v-if="typeof page === 'number'" @click="goToPage(page)" :class="[
                isCurrentPage(page) ? 'bg-blue-50 text-blue-600' : 'text-gray-900',
                'relative inline-flex items-center px-4 py-2 text-sm font-semibold ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0'
              ]">
                {{ page }}
              </button>
              <span v-else
                class="relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-700 ring-1 ring-inset ring-gray-300 focus:outline-offset-0">
                ...
              </span>
            </template>

            <button @click="nextPage" :disabled="currentPage >= Math.ceil(totalCount / paginationLimit)"
              class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
              :class="currentPage >= Math.ceil(totalCount / paginationLimit) ? 'opacity-50 cursor-not-allowed' : ''">
              <span class="sr-only">Next</span>
              <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fill-rule="evenodd"
                  d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z"
                  clip-rule="evenodd" />
              </svg>
            </button>
          </nav>
        </div>
      </div>
    </div>

    <!-- Модальное окно просмотра деталей IoC -->
    <div v-if="showDetailsModal" class="fixed inset-0 flex items-center justify-center z-50">
      <div class="absolute inset-0 bg-black opacity-50" @click="closeDetailsModal"></div>
      <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-lg overflow-hidden">
        <div class="border-b border-gray-200">
          <div class="flex justify-between items-center mb-3">
            <h3 class="text-lg font-semibold text-gray-800">Детали индикатора</h3>
            <button @click="closeDetailsModal" class="text-gray-500 hover:text-gray-700 transition-colors">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>
        </div>

        <div v-if="selectedIoC" class="p-4 space-y-4 max-h-[calc(100vh-12rem)] overflow-y-auto">
          <div class="bg-gray-50 p-4 rounded-lg border border-gray-100">
            <div class="flex items-center mb-3">
              <div
                :class="['w-10 h-10 rounded-full flex items-center justify-center mr-2', getSeverityIcon(selectedIoC.severity || 'low')?.bg]">
                <svg class="w-6 h-6" :class="getSeverityIcon(selectedIoC.severity || 'low')?.icon" fill="none"
                  stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              </div>
              <span class="px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full mr-2"
                :class="getTypeClass(selectedIoC.type)">
                {{ selectedIoC.type }}
              </span>
              <div class="text-lg font-medium text-gray-900 break-all">{{ selectedIoC.value }}</div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 pt-3 border-t border-gray-200">
              <div>
                <strong>ID:</strong> {{ selectedIoC.id }}
              </div>
              <div>
                <strong>Источник:</strong> {{ selectedIoC.source }}
              </div>
              <div>
                <strong>Первое обнаружение:</strong> {{ new Date(selectedIoC.firstSeen).toLocaleString() }}
              </div>
              <div>
                <strong>Последнее обнаружение:</strong> {{ new Date(selectedIoC.lastSeen).toLocaleString() }}
              </div>
              <div>
                <strong>Серьезность:</strong> {{ selectedIoC.severity }}
              </div>
              <div>
                <strong>Теги:</strong> {{ selectedIoC.tags && selectedIoC.tags.length > 0 && selectedIoC.tags[0] !== ''
                  ?
                  selectedIoC.tags.join(', ') : 'Нет тегов' }}
              </div>
            </div>
          </div>

          <div v-if="selectedIoC.additionalData && Object.keys(selectedIoC.additionalData).length"
            class="bg-gray-50 p-4 rounded-lg border border-gray-100">
            <h4 class="text-xs uppercase tracking-wider text-gray-600 font-semibold mb-2">Дополнительные данные</h4>
            <pre class="text-sm text-gray-700 whitespace-pre-wrap">{{ formatAdditionalData(selectedIoC.additionalData) }}
        </pre>
          </div>
        </div>

        <div class="bg-gray-50 px-6 py-3 rounded-b-lg mt-4">
          <div class="flex justify-center">
            <button @click="closeDetailsModal"
              class="px-3 py-1.5 text-sm text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 transition-colors">
              Закрыть
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Модальное окно подтверждения удаления -->
    <div v-if="showDeleteConfirm" class="fixed inset-0 flex items-center justify-center z-50">
      <div class="absolute inset-0 bg-black opacity-50" @click="closeDeleteConfirm"></div>
      <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-md">
        <h3 class="text-lg font-semibold mb-4">Подтверждение удаления</h3>
        <p class="text-gray-600 mb-6">
          Вы действительно хотите удалить индикатор <strong>{{ iocToDelete?.value }}</strong> ({{ iocToDelete?.type }})?
          <br>Это действие нельзя отменить.
        </p>

        <div class="flex justify-end gap-2">
          <button @click="closeDeleteConfirm" class="px-4 py-2 text-gray-700 bg-gray-200 rounded hover:bg-gray-300">
            Отмена
          </button>
          <button @click="deleteSelectedIoC" class="px-4 py-2 text-white bg-red-600 rounded hover:bg-red-700"
            :disabled="deleting">
            {{ deleting ? 'Удаление...' : 'Удалить' }}
          </button>
        </div>

        <div v-if="deleteError" class="mt-3 text-sm text-red-600">
          {{ deleteError }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue';
import {
  type IoCDTO,
  type IoCFilter,
  getAllIoCs,
  getIoCCount,
  getIoCCountByType,
  getIoCCountBySource,
  getFilteredIoCCount,
  getIoCCountSpecificType,
  getIoCCountSpecificSource,
  getIoCCountBySourceAndType
} from '../../services/adminIoCService';

type Severity = 'high' | 'medium' | 'low' | 'critical';

interface IoC extends IoCDTO {
  severity?: Severity;
}

// Состояние данных
const iocs = ref<IoCDTO[]>([]);
const totalCount = ref(0);
const currentPage = ref(1);
const paginationLimit = ref(10);
const paginationOffset = computed(() => (currentPage.value - 1) * paginationLimit.value);
const searchQuery = ref('');
const filters = ref<IoCFilter>({
  type: '',
  source: '',
});
const typeStats = ref<Record<string, number>>({});
const sourceStats = ref<Record<string, number>>({});
const availableTypes = ref<string[]>([]);
const availableSources = ref<string[]>([]);

// Состояние загрузки и ошибок
const loading = ref(false);
const error = ref('');

// Состояние модальных окон
const showDetailsModal = ref(false);
const selectedIoC = ref<IoC | null>(null);
const showDeleteConfirm = ref(false);
const iocToDelete = ref<IoCDTO | null>(null);
const deleting = ref(false);
const deleteError = ref<string | null>(null);

// Переменная для хранения таймера
const searchTimeout = ref<ReturnType<typeof setTimeout> | null>(null);

// Добавляю обработку ошибок API
const apiError = ref<string | null>(null);

// Управление скроллом
const disableBodyScroll = () => {
  document.body.classList.add('overflow-hidden');
};

const enableBodyScroll = () => {
  document.body.classList.remove('overflow-hidden');
};

// Элементы пагинации
const paginationItems = computed(() => {
  const current = currentPage.value;
  const total = Math.ceil(totalCount.value / paginationLimit.value);
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

const hasPagination = computed(() => totalCount.value > paginationLimit.value);

const sortedTypeStats = computed(() => {
  return Object.entries(typeStats.value)
    .sort(([, a], [, b]) => b - a)
    .reduce((obj, [key, value]) => ({ ...obj, [key]: value }), {});
});

const sortedSourceStats = computed(() => {
  return Object.entries(sourceStats.value)
    .sort(([, a], [, b]) => b - a)
    .reduce((obj, [key, value]) => ({ ...obj, [key]: value }), {});
});

// Загрузка данных
const fetchIoCs = async () => {
  try {
    loading.value = true;
    // console.log('Fetching IoCs with filters:', {
    //   search: searchQuery.value,
    //   type: filters.value.type,
    //   source: filters.value.source,
    //   limit: paginationLimit.value,
    //   offset: paginationOffset.value
    // });

    // Получаем общее количество в зависимости от выбранных фильтров
    if (filters.value.type && !filters.value.source) {
      totalCount.value = await getIoCCountSpecificType(filters.value.type);
    } else if (filters.value.source && !filters.value.type) {
      totalCount.value = await getIoCCountSpecificSource(filters.value.source);
    } else if (filters.value.source && filters.value.type) {
      const sourceTypeCounts = await getIoCCountBySourceAndType(filters.value.source);
      totalCount.value = sourceTypeCounts[filters.value.type] || 0;
    } else {
      totalCount.value = await getFilteredIoCCount(searchQuery.value, filters.value.type, filters.value.source);
    }

    // console.log('Total count after filtering:', totalCount.value);

    // Получаем данные
    const response = await getAllIoCs(
      paginationLimit.value,
      paginationOffset.value,
      searchQuery.value,
      filters.value.type,
      filters.value.source
    );

    // console.log('Received data:', response);

    // Проверяем формат ответа и применяем фильтры
    let filteredData = Array.isArray(response) ? response : (response?.items || []);

    // Применяем фильтры к полученным данным
    if (filters.value.type) {
      filteredData = filteredData.filter(ioc => ioc.type === filters.value.type);
    }
    if (filters.value.source) {
      filteredData = filteredData.filter(ioc => ioc.source === filters.value.source);
    }

    iocs.value = filteredData;

  } catch (error: any) {
    // console.error('Error fetching IoCs:', error);
    apiError.value = error.message || 'Ошибка при загрузке данных';
    iocs.value = [];
  } finally {
    loading.value = false;
  }
};

// Загрузка статистики по типам
const fetchTypeStats = async () => {
  try {
    typeStats.value = await getIoCCountByType();
  } catch (error: any) {
    // console.error('Ошибка при загрузке статистики по типам:', error);
  }
};

// Загрузка статистики по источникам
const fetchSourceStats = async () => {
  try {
    sourceStats.value = await getIoCCountBySource();
  } catch (error: any) {
    // console.error('Ошибка при загрузке статистики по источникам:', error);
  }
};

// Получение цвета для типа индикатора
const getTypeClass = (type: string): string => {
  const typeMap: Record<string, string> = {
    'ip': 'bg-red-100 text-red-800',
    'domain': 'bg-blue-100 text-blue-800',
    'url': 'bg-green-100 text-green-800',
    'md5': 'bg-purple-100 text-purple-800',
    'sha256': 'bg-purple-100 text-purple-800'
  };

  return typeMap[type] || 'bg-gray-100 text-gray-800';
};

const getTypeColorClass = (type: string): string => {
  const typeMap: Record<string, string> = {
    'ip': 'bg-red-500',
    'domain': 'bg-blue-500',
    'url': 'bg-green-500',
    'md5': 'bg-purple-500',
    'sha256': 'bg-purple-500'
  };

  return typeMap[type] || 'bg-gray-500';
};

// Функция для расчета номера индикатора с учетом пагинации
const calculateIoCIndex = (index: number): number => {
  if (totalCount.value === 0) return 0;
  return paginationOffset.value + index + 1;
};

// Функция для форматирования даты и времени
const formatDate = (dateString: string | Date): string => {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) {
    return 'Неизвестно';
  }
  return date.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit', year: 'numeric' }) + ', ' +
    date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
};

// Форматирование дополнительных данных для отображения
const formatAdditionalData = (data: Record<string, any>): string => {
  return JSON.stringify(data, null, 2);
};

const applyFilters = () => {
  // console.log('Applying filters:', filters.value);
  currentPage.value = 1;
  fetchIoCs();
};

const prevPage = () => {
  if (paginationOffset.value >= paginationLimit.value) {
    currentPage.value -= 1;
    fetchIoCs();
  }
};

const nextPage = () => {
  if (currentPage.value < Math.ceil(totalCount.value / paginationLimit.value)) {
    currentPage.value += 1;
    fetchIoCs();
  }
};

const goToPage = (page: number) => {
  currentPage.value = page;
  fetchIoCs();
};

const isCurrentPage = (page: number): boolean => {
  return page === currentPage.value;
};

// Функции для работы с модальными окнами
const viewIoCDetails = (ioc: IoC) => {
  selectedIoC.value = {
    ...ioc,
    severity: getSeverityByType(ioc.type)
  };
  showDetailsModal.value = true;
  disableBodyScroll();
};

const closeDetailsModal = () => {
  showDetailsModal.value = false;
  selectedIoC.value = null;
  enableBodyScroll();
};

const confirmDeleteIoC = (ioc: IoCDTO) => {
  iocToDelete.value = ioc;
  showDeleteConfirm.value = true;
  disableBodyScroll();
};

const closeDeleteConfirm = () => {
  showDeleteConfirm.value = false;
  iocToDelete.value = null;
  enableBodyScroll();
};

const deleteSelectedIoC = async () => {
  if (!iocToDelete.value) return;

  deleting.value = true;
  deleteError.value = null;

  try {
    // В API пока нет метода для удаления
    // await deleteIoC(iocToDelete.value.id);

    // Удаляем из локального списка
    iocs.value = iocs.value.filter(ioc => ioc.id !== iocToDelete.value!.id);

    // Уменьшаем общее количество
    totalCount.value--;

    // Перезагружаем данные если текущая страница пустая
    if (iocs.value.length === 0 && paginationOffset.value > 0) {
      currentPage.value = Math.max(1, Math.ceil(paginationOffset.value / paginationLimit.value));
      fetchIoCs();
    }

    closeDeleteConfirm();
  } catch (error: any) {
    deleteError.value = error.message || 'Ошибка при удалении индикатора';
  } finally {
    deleting.value = false;
  }
};

const onLimitChange = () => {
  currentPage.value = 1;
  fetchIoCs();
};

const clearSearch = () => {
  searchQuery.value = '';
  currentPage.value = 1;
  fetchIoCs();
};

// Инициализация компонента
onMounted(async () => {
  // console.log('Component mounted');
  // Загружаем статистику параллельно
  await Promise.all([
    fetchIoCs(),
    fetchTypeStats(),
    fetchSourceStats()
  ]);
});

// На всякий случай включаем скролл, если компонент удаляется
onBeforeUnmount(() => {
  enableBodyScroll();
  // Очищаем таймер поиска при удалении компонента
  if (searchTimeout.value) clearTimeout(searchTimeout.value);
});

// debounce-поиск
watch(searchQuery, (val) => {
  if (searchTimeout.value) clearTimeout(searchTimeout.value);
  searchTimeout.value = setTimeout(() => {
    currentPage.value = 1;
    fetchIoCs();
  }, 400);
});

// Добавляем наблюдатели за изменением фильтров
watch(filters, () => {
  // console.log('Filters changed:', filters.value);
}, { deep: true });

const getSeverityIcon = (severity: Severity) => {
  return {
    high: {
      bg: 'bg-red-100',
      icon: 'text-red-600'
    },
    medium: {
      bg: 'bg-yellow-100',
      icon: 'text-yellow-600'
    },
    low: {
      bg: 'bg-blue-100',
      icon: 'text-blue-600'
    },
    critical: {
      bg: 'bg-red-200',
      icon: 'text-red-800'
    }
  }[severity];
};

const getSeverityByType = (type: string): Severity => {
  const severityMap: Record<string, Severity> = {
    'ip': 'critical',
    'domain': 'medium',
    'url': 'medium',
    'md5': 'low',
    'sha256': 'low'
  };
  return severityMap[type] || 'low';
};
</script>

<style>
body.overflow-hidden {
  overflow: hidden;
}

/* Для адаптивности таблицы на мобильных устройствах */
@media (max-width: 640px) {
  .min-w-full {
    min-width: auto;
  }
}
</style>