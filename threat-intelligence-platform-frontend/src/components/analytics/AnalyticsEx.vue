<script setup lang="ts">
import { ref, computed, onMounted, watch, defineExpose } from 'vue';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { PieChart, BarChart, LineChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components';
import VChart from 'vue-echarts';
import { analyticsApi, type IoC, type ThreatLevel } from '../../api/analytics';
import { useRouter } from 'vue-router';
import { useAuth } from '../../services/authService';

// Register the required ECharts components
use([
  CanvasRenderer,
  PieChart,
  BarChart,
  LineChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
]);

const router = useRouter();
const auth = useAuth();

const isLoading = ref(true);
const error = ref('');
const totalCount = ref(0);
const countByType = ref<Record<string, number>>({});
const countBySource = ref<Record<string, number>>({});
const recentIoCs = ref<IoC[]>([]);

// Удаляем IoCExtended, так как IoC теперь содержит threatLevel
const selectedIoC = ref<IoC | null>(null);

// Пагинация
const pageSizeOptions = [10, 25, 50, 100];
const pageSize = ref(10);
const currentPage = ref(1);

const totalPages = computed(() => Math.ceil(recentIoCs.value.length / pageSize.value));
const startIndex = computed(() => (currentPage.value - 1) * pageSize.value);
const endIndex = computed(() => Math.min(startIndex.value + pageSize.value, recentIoCs.value.length));

const paginatedIoCs = computed(() => {
  const start = startIndex.value;
  const end = endIndex.value;
  return recentIoCs.value.slice(start, end);
});

const displayedPages = computed(() => {
  const pages = [];
  const maxPages = 5;
  let start = Math.max(1, currentPage.value - Math.floor(maxPages / 2));
  let end = Math.min(totalPages.value, start + maxPages - 1);

  if (end - start + 1 < maxPages) {
    start = Math.max(1, end - maxPages + 1);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

const handlePageSizeChange = () => {
  currentPage.value = 1;
};

// Функции для работы с модальным окном
const openDetails = (ioc: IoC) => {
  selectedIoC.value = ioc;
  if (typeof document !== 'undefined') {
    document.body.classList.add('overflow-hidden');
  }
};

const closeDetails = () => {
  selectedIoC.value = null;
  if (typeof document !== 'undefined') {
    document.body.classList.remove('overflow-hidden');
  }
};

const getThreatLevelText = (level?: ThreatLevel) => {
  switch (level) {
    case 'critical':
      return 'Критический';
    case 'high':
      return 'Высокий';
    case 'medium':
      return 'Средний';
    case 'low':
      return 'Низкий';
    case 'informational':
      return 'Информационный';
    default:
      return 'Неизвестно';
  }
};

const getTypeClass = (type: string) => {
  switch (type.toLowerCase()) {
    case 'ip':
      return 'bg-red-100 text-red-800';
    case 'domain':
      return 'bg-blue-100 text-blue-800';
    case 'url':
      return 'bg-green-100 text-green-800';
    case 'hash':
      return 'bg-purple-100 text-purple-800';
    default:
      return 'bg-gray-100 text-gray-800';
  }
};

const getSeverityIcon = (level: ThreatLevel) => {
  switch (level) {
    case 'critical':
      return { bg: 'bg-red-500', icon: 'text-white' };
    case 'high':
      return { bg: 'bg-orange-500', icon: 'text-white' };
    case 'medium':
      return { bg: 'bg-yellow-500', icon: 'text-white' };
    case 'low':
      return { bg: 'bg-green-500', icon: 'text-white' };
    case 'informational':
      return { bg: 'bg-blue-500', icon: 'text-white' };
    default:
      return { bg: 'bg-gray-500', icon: 'text-white' };
  }
};

// Подсчет тегов из всех IoC
const tagCounts = computed(() => {
  const counts: Record<string, number> = {};
  recentIoCs.value.forEach(ioc => {
    ioc.tags.forEach(tag => {
      if (tag) {
        counts[tag] = (counts[tag] || 0) + 1;
      }
    });
  });
  return counts;
});

// Общее количество тегов
const totalTagsCount = computed(() => {
  return Object.values(tagCounts.value).reduce((sum, count) => sum + count, 0);
});

// Подготовка данных для графика по времени
const timeData = computed(() => {
  const dates = recentIoCs.value.map(ioc => new Date(ioc.firstSeen));
  const sortedDates = dates.sort((a, b) => a.getTime() - b.getTime());
  return sortedDates.map((date, index) => [date, index + 1]);
});

const typeChartOption = computed(() => ({
  tooltip: {
    trigger: 'item',
    formatter: '{b}: {c} ({d}%)'
  },
  legend: {
    orient: 'vertical',
    right: '10%',
    top: 'center',
    padding: [0, 0, 0, 100],
  },
  series: [
    {
      type: 'pie',
      radius: ['40%', '70%'],
      center: ['40%', '50%'],
      minAngle: 15,
      avoidLabelOverlap: false,
      itemStyle: {
        borderRadius: 10,
        borderColor: '#fff',
        borderWidth: 2
      },
      label: {
        show: false,
        position: 'center'
      },
      emphasis: {
        label: {
          show: true,
          fontSize: '20',
          fontWeight: 'bold'
        }
      },
      labelLine: {
        show: false
      },
      data: Object.entries(countByType.value)
        .sort(([, a], [, b]) => b - a)
        .map(([name, value]) => ({
          name,
          value,
          itemStyle: {
            color: getTypeColor(name)
          }
        }))
    }
  ]
}));

const getTypeColor = (type: string) => {
  const colors: Record<string, string> = {
    'sha256': '#6384C6',
    'ip': '#8DC674',
    'md5': '#F7E04C',
    'domain': '#F07C6F',
    'url': '#7FCBE5',
    'default': '#CCCCCC'
  };
  return colors[type.toLowerCase()] || colors.default;
};

const sourceChartOption = computed(() => {
  const sortedSources = Object.entries(countBySource.value)
    .sort(([, a], [, b]) => a - b); // Сортировка по возрастанию значений

  return {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: sortedSources.map(([source]) => source),
      axisLabel: {
        interval: 0, // Показать все метки
        rotate: 30,  // Повернуть метки на 30 градусов, если они длинные
      },
      axisTick: {
        alignWithLabel: true
      }
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        type: 'bar',
        barWidth: '60%',
        data: sortedSources.map(([, count]) => count),
        itemStyle: {
          color: '#3B82F6'
        }
      }
    ]
  };
});

const tagsChartOption = computed(() => {
  const sortedTags = Object.entries(tagCounts.value)
    .sort(([, a], [, b]) => b - a)
    .slice(0, 10);

  return {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'value'
    },
    yAxis: {
      type: 'category',
      data: sortedTags.map(([tag]) => tag)
    },
    series: [
      {
        type: 'bar',
        data: sortedTags.map(([, count]) => count),
        itemStyle: {
          color: '#10B981'
        }
      }
    ]
  };
});

const timeChartOption = computed(() => ({
  tooltip: {
    trigger: 'axis'
  },
  xAxis: {
    type: 'time',
    boundaryGap: false
  },
  yAxis: {
    type: 'value',
    name: 'Количество'
  },
  series: [
    {
      type: 'line',
      smooth: true,
      data: timeData.value,
      areaStyle: {
        opacity: 0.1
      },
      itemStyle: {
        color: '#8B5CF6'
      }
    }
  ]
}));

const loadData = async () => {
  isLoading.value = true;
  error.value = '';
  try {
    if (!auth.isAuthenticated.value) {
      router.push('/login');
      return;
    }
    const [
      totalCountRes,
      countByTypeRes,
      countBySourceRes,
      allIoCsRes
    ] = await Promise.all([
      analyticsApi.getTotalCount(),
      analyticsApi.getCountByType(),
      analyticsApi.getCountBySource(),
      analyticsApi.getAllIoCs(100), // Fetch 100 IoCs for charts
    ]);

    totalCount.value = totalCountRes;
    countByType.value = countByTypeRes;
    countBySource.value = countBySourceRes;
    recentIoCs.value = allIoCsRes.map(ioc => ({
      ...ioc,
      // ThreatLevel и description теперь должны быть в самом IoC, но на всякий случай переопределим их здесь
      threatLevel: ioc.threatLevel || getThreatLevelFromTags(ioc.tags),
      description: ioc.description || ioc.additionalData?.description || ''
    }));
  } catch (err: any) {
    console.error('Ошибка при загрузке данных аналитики:', err);
    if (err.message.includes('401')) {
      error.value = 'Сессия истекла. Пожалуйста, войдите снова.';
      auth.logout();
      router.push('/login');
    } else {
      error.value = err.message || 'Не удалось загрузить данные аналитики.';
    }
  } finally {
    isLoading.value = false;
  }
};

const getThreatLevelFromTags = (tags: string[]): ThreatLevel => {
  if (tags.includes('critical')) return 'critical';
  if (tags.includes('high')) return 'high';
  if (tags.includes('medium')) return 'medium';
  if (tags.includes('low')) return 'low';
  return 'informational';
};

onMounted(() => {
  loadData();
});

watch([currentPage, pageSize], loadData);

defineExpose({
  getThreatLevelText,
  getTypeClass,
  getSeverityIcon,
  openDetails,
  closeDetails
});
</script>

<template>
  <div class="space-y-6 px-4 py-6">
    <h1 class="text-3xl font-bold text-gray-900 mb-6">Аналитика IoC</h1>

    <!-- Карточки с общей статистикой -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
      <div class="bg-white rounded-lg shadow p-6 text-center">
        <h2 class="text-lg font-semibold text-gray-700">Всего IoC</h2>
        <p class="text-4xl font-bold text-red-600 mt-2">{{ totalCount.toLocaleString('ru-RU') }}</p>
      </div>

      <!-- Новая карточка для общего количества тегов -->
      <div class="bg-white rounded-lg shadow p-6 text-center">
        <h2 class="text-lg font-semibold text-gray-700">Всего тегов</h2>
        <p class="text-4xl font-bold text-blue-600 mt-2">{{ totalTagsCount.toLocaleString('ru-RU') }}</p>
      </div>

      <div class="bg-white rounded-lg shadow p-6 text-center">
        <h2 class="text-lg font-semibold text-gray-700">Уникальных типов</h2>
        <p class="text-4xl font-bold text-yellow-300 mt-2">{{ Object.keys(countByType).length }}</p>
      </div>

      <div class="bg-white rounded-lg shadow p-6 text-center">
        <h2 class="text-lg font-semibold text-gray-700">Источников данных</h2>
        <p class="text-4xl font-bold text-purple-600 mt-2">{{ Object.keys(countBySource).length }}</p>
      </div>
    </div>

    <!-- Состояние загрузки -->
    <div v-if="isLoading" class="flex justify-center items-center h-64">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>

    <!-- Состояние ошибки -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-6 mb-8">
      <div class="flex items-center">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd"
              d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
              clip-rule="evenodd" />
          </svg>
        </div>
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">Ошибка загрузки данных</h3>
          <div class="mt-2 text-sm text-red-700">
            <p>{{ error }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Основной контент -->
    <template v-else>
      <!-- Графики -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4">Распределение по типам</h3>
          <v-chart class="chart" :option="typeChartOption" autoresize />
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4">Распределение по источникам</h3>
          <v-chart class="chart" :option="sourceChartOption" autoresize />
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4">Топ тегов</h3>
          <v-chart class="chart" :option="tagsChartOption" autoresize />
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4">Распределение по времени</h3>
          <v-chart class="chart" :option="timeChartOption" autoresize />
        </div>
      </div>

      <!-- Таблица последних IoC -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold">Последние IoC</h3>
          <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-2">
              <label for="pageSize" class="text-sm text-gray-600">Записей на странице:</label>
              <select id="pageSize" v-model="pageSize"
                class="rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                @change="handlePageSizeChange">
                <option v-for="size in pageSizeOptions" :key="size" :value="size">
                  {{ size }}
                </option>
              </select>
            </div>
          </div>
        </div>

        <div class="border rounded-md overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-2 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider w-10">#</th>
                <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider w-1/6">Тип</th>
                <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider w-1/4">Значение</th>
                <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider w-1/6">Источник</th>
                <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider w-1/4">Теги</th>
                <th class="px-4 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider w-16">Действия</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="(ioc, index) in paginatedIoCs" :key="ioc.id" class="hover:bg-gray-50">
                <td class="px-2 py-4 whitespace-nowrap text-center text-sm text-gray-600 font-medium w-10">
                  {{ startIndex + index + 1 }}
                </td>
                <td class="px-4 py-4 whitespace-nowrap w-1/6">
                  <span class="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full"
                    :class="getTypeClass(ioc.type)">
                    {{ ioc.type }}
                  </span>
                </td>
                <td class="px-4 py-4 w-1/4">
                  <div class="text-sm text-gray-900 truncate max-w-md">{{ ioc.value }}</div>
                </td>
                <td class="px-4 py-4 whitespace-nowrap w-1/6">
                  <div class="text-sm text-gray-900">{{ ioc.source }}</div>
                </td>
                <td class="px-4 py-4 w-1/4">
                  <div class="flex flex-wrap gap-1">
                    <span v-for="tag in ioc.tags" :key="tag" class="px-1.5 py-0.5 text-xs rounded-md"
                      :class="tag ? 'bg-gray-100 text-gray-800' : 'bg-transparent'">
                      {{ tag || '—' }}
                    </span>
                    <span v-if="!ioc.tags || ioc.tags.length === 0 || (ioc.tags.length === 1 && !ioc.tags[0])"
                      class="text-gray-400 text-xs">Нет тегов</span>
                  </div>
                </td>
                <td class="px-4 py-4 whitespace-nowrap text-sm w-16">
                  <div class="flex gap-2">
                    <button @click="openDetails(ioc)" class="text-blue-600 hover:text-blue-900 relative group">
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
                  </div>
                </td>
              </tr>
              <tr v-if="paginatedIoCs.length === 0">
                <td colspan="6" class="px-4 py-4 text-center text-sm text-gray-500">
                  {{ isLoading ? 'Загрузка угроз...' : 'Угрозы не найдены' }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Пагинация -->
        <div class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 mt-4">
          <div class="flex flex-1 justify-between sm:hidden">
            <button @click="currentPage--" :disabled="currentPage === 1"
              class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
              :class="{ 'opacity-50 cursor-not-allowed': currentPage === 1 }">
              Назад
            </button>
            <button @click="currentPage++" :disabled="currentPage === totalPages"
              class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
              :class="{ 'opacity-50 cursor-not-allowed': currentPage === totalPages }">
              Вперед
            </button>
          </div>
          <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
            <div>
              <p class="text-sm text-gray-700">
                Показано
                <span class="font-medium">{{ startIndex + 1 }}</span>
                -
                <span class="font-medium">{{ endIndex }}</span>
                из
                <span class="font-medium">{{ totalCount }}</span>
                результатов
              </p>
            </div>
            <div>
              <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                <button @click="currentPage--" :disabled="currentPage === 1"
                  class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
                  :class="{ 'opacity-50 cursor-not-allowed': currentPage === 1 }">
                  <span class="sr-only">Предыдущая</span>
                  <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd"
                      d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z"
                      clip-rule="evenodd" />
                  </svg>
                </button>
                <button v-for="page in displayedPages" :key="page" @click="currentPage = page" :class="[
                  'relative inline-flex items-center px-4 py-2 text-sm font-semibold',
                  currentPage === page
                    ? 'z-10 bg-blue-600 text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600'
                    : 'text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0'
                ]">
                  {{ page }}
                </button>
                <button @click="currentPage++" :disabled="currentPage === totalPages"
                  class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
                  :class="{ 'opacity-50 cursor-not-allowed': currentPage === totalPages }">
                  <span class="sr-only">Следующая</span>
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
      </div>
    </template>

    <!-- Модальное окно просмотра деталей IoC -->
    <div v-if="selectedIoC" class="fixed inset-0 flex items-center justify-center z-50">
      <div class="absolute inset-0 bg-black opacity-50" @click="closeDetails"></div>
      <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-lg overflow-hidden">
        <div class="border-b border-gray-200">
          <div class="flex justify-between items-center mb-3">
            <h3 class="text-lg font-semibold text-gray-800">Детали угрозы</h3>
            <button @click="closeDetails" class="text-gray-500 hover:text-gray-700 transition-colors">
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
                :class="['w-10 h-10 rounded-full flex items-center justify-center mr-2', getSeverityIcon(selectedIoC.threatLevel || 'low')?.bg]">
                <svg class="w-6 h-6" :class="getSeverityIcon(selectedIoC.threatLevel || 'low')?.icon" fill="none"
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
                <strong>Серьезность:</strong>
                <span :class="['px-2.5 py-0.5 rounded-full text-xs font-medium',
                  {
                    'bg-red-100 text-red-800': selectedIoC.threatLevel === 'critical',
                    'bg-orange-100 text-orange-800': selectedIoC.threatLevel === 'high',
                    'bg-yellow-100 text-yellow-800': selectedIoC.threatLevel === 'medium',
                    'bg-green-100 text-green-800': selectedIoC.threatLevel === 'low'
                  }]">
                  {{ getThreatLevelText(selectedIoC.threatLevel) }}
                </span>
              </div>
              <div>
                <strong>Теги:</strong>
                <div class="flex flex-wrap gap-1">
                  <span v-for="tag in selectedIoC.tags" :key="tag" class="px-1.5 py-0.5 text-xs rounded-md"
                    :class="tag ? 'bg-gray-100 text-gray-800' : 'bg-transparent'">
                    {{ tag || '—' }}
                  </span>
                  <span
                    v-if="!selectedIoC.tags || selectedIoC.tags.length === 0 || (selectedIoC.tags.length === 1 && !selectedIoC.tags[0])"
                    class="text-gray-400 text-xs">Нет тегов</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="px-6 py-4 border-t border-gray-200 flex justify-end">
          <button @click="closeDetails"
            class="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.chart {
  height: 400px;
}
</style>
