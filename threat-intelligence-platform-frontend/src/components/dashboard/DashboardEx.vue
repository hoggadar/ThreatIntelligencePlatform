<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import axios from '../../api/axios'; // Импортируем настроенный экземпляр axios

// Тип для уровня серьезности
type Severity = 'high' | 'medium' | 'low' | 'critical';

interface Card {
  title: string;
  badgeText: string;
  badgeColor: string;
  value: number;
  subtitle: string;
  trend: string;
  trendColor: string;
  progress: number;
  progressColor: string;
}

interface ThreatType {
  name: string;
  count: number;
  percentage: number;
  color: string;
}

interface Incident {
  id: string;
  source: string;
  firstSeen: string;
  lastSeen: string;
  type: string;
  value: string;
  tags: string[];
  additionalData: Record<string, any>;
  severity: Severity;
  time?: string;
}

// Состояние для данных
const cards = ref<Card[]>([
  {
    title: 'Общее количество индикаторов',
    badgeText: 'Активно',
    badgeColor: 'bg-red-100 text-red-800',
    value: 0,
    subtitle: 'Всего в системе',
    trend: '+0',
    trendColor: 'text-red-600',
    progress: 0,
    progressColor: 'bg-red-500'
  },
  {
    title: 'Новые индикаторы за сегодня',
    badgeText: 'Новые',
    badgeColor: 'bg-green-100 text-green-800',
    value: 0,
    subtitle: 'Поступило за 24 часа',
    trend: '+0',
    trendColor: 'text-green-600',
    progress: 0,
    progressColor: 'bg-green-500'
  },
  {
    title: 'Индикаторы высокой опасности',
    badgeText: 'Высокая',
    badgeColor: 'bg-yellow-100 text-yellow-800',
    value: 0,
    subtitle: 'Требуют внимания',
    trend: '+0',
    trendColor: 'text-yellow-600',
    progress: 0,
    progressColor: 'bg-yellow-500'
  }
]);

const threatTypes = ref<ThreatType[]>([]);
const recentIncidents = ref<Incident[]>([]);
const loading = ref(false);
const error = ref('');

// Состояние для модального окна
const isModalOpen = ref(false);
const selectedIncident = ref<Incident | null>(null);

const disableBodyScroll = () => {
  document.body.style.overflow = 'hidden';
};

const enableBodyScroll = () => {
  document.body.style.overflow = '';
};

// Функция для открытия модального окна
const openModal = (incident: Incident) => {
  selectedIncident.value = incident;
  isModalOpen.value = true;
  disableBodyScroll();
};

// Функция для закрытия модального окна
const closeModal = () => {
  isModalOpen.value = false;
  selectedIncident.value = null;
  enableBodyScroll();
};

// Предыдущие значения для расчета трендов
const previousValues = ref({
  totalIndicators: 0,
  newIndicatorsToday: 0,
  highSeverityIndicators: 0,
  lastUpdate: new Date()
});

// Функция для получения цвета типа угрозы
const getTypeColor = (type: string): string => {
  const colorMap: Record<string, string> = {
    'ip': 'bg-red-500',
    'domain': 'bg-blue-500',
    'url': 'bg-green-500',
    'md5': 'bg-purple-500',
    'sha256': 'bg-purple-500'
  };
  return colorMap[type] || 'bg-gray-500';
};

// Функция для получения цвета иконки в зависимости от серьезности
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

// Функция для расчета тренда
const calculateTrend = (current: number, previous: number): string => {
  if (previous === 0) return '+0%';
  const change = ((current - previous) / previous) * 100;
  return `${change >= 0 ? '+' : ''}${change.toFixed(1)}%`;
};

// Функция для форматирования времени
const formatTimeAgo = (timestamp: number): string => {
  const now = Date.now();
  const diff = now - timestamp;
  const minutes = Math.floor(diff / 60000);

  if (minutes < 60) {
    return `${minutes}м назад`;
  } else if (minutes < 1440) {
    const hours = Math.floor(minutes / 60);
    return `${hours}ч назад`;
  } else {
    const days = Math.floor(minutes / 1440);
    return `${days}д назад`;
  }
};

// Функция для определения серьезности по типу
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

// Функция для получения статистики по типам
const getIoCCountByType = async (): Promise<Record<string, number>> => {
  try {
    const response = await axios.get('/IoC/CountByType');
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении статистики по типам:', error);
    return {};
  }
};

// Функция для получения последних инцидентов
const getLatestIncidents = async (): Promise<Incident[]> => {
  try {
    const response = await axios.get('/IoC/GetAll', { params: { limit: 10, offset: 0 } });
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении последних инцидентов:', error);
    return [];
  }
};

// Функция для получения общего количества индикаторов
const getIoCCount = async (): Promise<number> => {
  try {
    const response = await axios.get('/IoC/Count');
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении общего количества индикаторов:', error);
    return 0;
  }
};

// Функция для получения всех индикаторов с фильтрацией на клиенте
const getIoCAll = async (): Promise<Incident[]> => {
  try {
    // В данном случае мы получаем все данные, чтобы фильтровать их на клиенте
    // Для больших объемов данных это может быть неэффективно
    const response = await axios.get('/IoC/GetAll', { params: { limit: 10000, offset: 0 } });
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении всех индикаторов:', error);
    return [];
  }
};

// Функция для загрузки данных
const fetchData = async () => {
  try {
    loading.value = true;
    error.value = '';

    // Получаем общее количество индикаторов
    const totalIndicators = await getIoCCount();

    // Получаем все индикаторы для клиентской фильтрации
    const allIndicators = await getIoCAll();

    // Применяем серьезность к каждому индикатору сразу после получения
    const processedIndicators = allIndicators.map(incident => ({
      ...incident,
      severity: getSeverityByType(incident.type)
    }));

    // Рассчитываем новые индикаторы за последние 24 часа
    const twentyFourHoursAgo = new Date();
    twentyFourHoursAgo.setHours(twentyFourHoursAgo.getHours() - 24);
    const newIndicatorsToday = processedIndicators.filter(incident => new Date(incident.firstSeen) > twentyFourHoursAgo).length;

    // Получаем статистику по типам угроз
    const typeStats = await getIoCCountByType();

    // Рассчитываем индикаторы высокой опасности, используя количество IP-адресов из статистики по типам
    const highSeverityIndicators = typeStats['ip'] || 0;

    // Обновляем карточки с новыми значениями
    cards.value[0].value = totalIndicators;
    cards.value[0].trend = `+${totalIndicators - previousValues.value.totalIndicators}`;
    cards.value[0].progress = Math.min((totalIndicators / 100000) * 100, 100); // Примерный расчет прогресса

    cards.value[1].value = newIndicatorsToday;
    cards.value[1].trend = `+${newIndicatorsToday - previousValues.value.newIndicatorsToday}`;
    cards.value[1].progress = Math.min((newIndicatorsToday / 100) * 100, 100); // Примерный расчет прогресса

    cards.value[2].value = highSeverityIndicators;
    cards.value[2].trend = `+${highSeverityIndicators - previousValues.value.highSeverityIndicators}`;
    cards.value[2].progress = Math.min((highSeverityIndicators / 10000) * 100, 100); // Примерный расчет прогресса

    // Обновляем предыдущие значения
    previousValues.value = {
      totalIndicators: totalIndicators,
      newIndicatorsToday: newIndicatorsToday,
      highSeverityIndicators: highSeverityIndicators,
      lastUpdate: new Date()
    };

    // Обновляем типы угроз с сортировкой по убыванию
    threatTypes.value = Object.entries(typeStats)
      .map(([name, count]) => ({
        name,
        count: count as number,
        percentage: Number(((count as number / totalIndicators) * 100).toFixed(1)),
        color: getTypeColor(name)
      }))
      .sort((a, b) => b.count - a.count);

    // Обновляем последние инциденты (оставляем существующую логику)
    recentIncidents.value = processedIndicators
      .map((incident: Incident) => ({
        ...incident,
        time: formatTimeAgo(new Date(incident.lastSeen).getTime()),
        // severity: getSeverityByType(incident.type) // Уже установлено в processedIndicators
      }))
      .sort((a: Incident, b: Incident) =>
        new Date(b.lastSeen).getTime() - new Date(a.lastSeen).getTime()
      )
      .slice(0, 3);

    loading.value = false;
  } catch (err) {
    error.value = 'Не удалось загрузить данные: ' + (err as Error).message;
    loading.value = false;
    console.error(err);
  }
};

// Установка интервала для обновления данных (каждые 5 минут)
let intervalId: number;
onMounted(() => {
  fetchData(); // Загружаем данные сразу при монтировании компонента
  intervalId = window.setInterval(fetchData, 5 * 60 * 1000); // Обновляем каждые 5 минут
});

onBeforeUnmount(() => {
  clearInterval(intervalId);
  enableBodyScroll(); // Убедимся, что прокрутка включена при размонтировании компонента
});
</script>

<template>
  <div class="space-y-6 px-4 py-6">
    <!-- Индикатор загрузки и ошибки -->
    <div v-if="loading" class="text-center py-4">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-4 border-primary-500 border-t-transparent">
      </div>
      <p class="mt-2 text-gray-600">Загрузка данных...</p>
    </div>

    <div v-if="error" class="bg-red-50 border border-red-200 text-red-800 rounded-md p-4 mb-6">
      {{ error }}
      <button @click="fetchData" class="text-blue-600 underline ml-2">Попробовать снова</button>
    </div>

    <!-- Карточки статистики -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="card in cards" :key="card.title" class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between">
          <h3 class="text-lg font-medium text-gray-900">{{ card.title }}</h3>
          <span :class="['inline-flex items-center px-2.5 py-0.5 rounded-full text-sm font-medium', card.badgeColor]">
            {{ card.badgeText }}
          </span>
        </div>
        <p class="mt-4 text-3xl font-bold text-gray-900">{{ card.value }}</p>
        <div class="mt-4">
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500">{{ card.subtitle }}</span>
            <span :class="[card.trendColor, 'font-medium']">{{ card.trend }}</span>
          </div>
          <div class="mt-2 h-2 bg-gray-100 rounded-full">
            <div class="h-2 rounded-full" :class="card.progressColor" :style="{ width: card.progress + '%' }"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Графики и таблицы -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Последние инциденты -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Последние инциденты</h3>
        <div class="space-y-4">
          <div v-for="incident in recentIncidents" :key="incident.id" @click="openModal(incident)"
            class="flex items-center space-x-4 p-4 bg-gray-50 rounded-lg cursor-pointer">
            <div class="flex-shrink-0">
              <div
                :class="['w-10 h-10 rounded-full flex items-center justify-center', getSeverityIcon(incident.severity)?.bg]">
                <svg class="w-6 h-6" :class="getSeverityIcon(incident.severity)?.icon" fill="none" stroke="currentColor"
                  viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 truncate">
                {{ incident.type }} из {{ incident.source }}
              </p>
              <p class="text-sm text-gray-500 overflow-hidden text-ellipsis whitespace-nowrap">
                {{ incident.value }}
              </p>
            </div>
            <div class="flex-shrink-0 text-sm text-gray-500 text-right">
              <span>{{ incident.time }}</span>
              <span class="text-xs block">Последнее обнаружение: </span>
              <span>{{ new Date(incident.lastSeen).toLocaleString()}}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Статистика по типам угроз -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Статистика по типам угроз</h3>
        <div class="space-y-4">
          <div v-for="(type, i) in threatTypes" :key="type.name" class="flex items-center">
            <div class="w-32 flex-shrink-0">
              <span class="text-sm font-medium text-gray-900">{{ type.name }}</span>
            </div>
            <div class="flex-1">
              <div class="h-2 bg-gray-100 rounded-full">
                <div class="h-2 rounded-full" :class="type.color" :style="{ width: type.percentage + '%' }"></div>
              </div>
            </div>
            <div class="w-32 text-right">
              <span class="text-sm font-medium text-gray-900">{{ type.count }} ({{ type.percentage.toFixed(1)
                }}%)</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Модальное окно просмотра деталей инцидента -->
  <div v-if="isModalOpen" class="fixed inset-0 flex items-center justify-center z-50">
    <div class="absolute inset-0 bg-black opacity-50" @click="closeModal"></div>
    <div class="bg-white p-6 rounded-lg shadow-lg z-10 w-full max-w-lg">
      <div class="border-b border-gray-200 pb-4 mb-4">
        <h3 class="text-lg font-semibold text-gray-800">Детали инцидента</h3>
        <button @click="closeModal" class="absolute top-4 right-4 text-gray-500 hover:text-gray-700 transition-colors">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <div v-if="selectedIncident" class="space-y-4 text-sm text-gray-700 max-h-[calc(100vh-12rem)] overflow-y-auto">
        <div class="bg-gray-50 p-4 rounded-lg border border-gray-100">
          <div class="flex items-center mb-3">
            <span class="px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full mr-2"
              :class="getSeverityIcon(selectedIncident.severity)?.bg">
              {{ selectedIncident.type }}
            </span>
            <div class="text-lg font-medium text-gray-900 break-all">{{ selectedIncident.value }}</div>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 pt-3 border-t border-gray-200">
            <div>
              <strong>ID:</strong> {{ selectedIncident.id }}
            </div>
            <div>
              <strong>Источник:</strong> {{ selectedIncident.source }}
            </div>
            <div>
              <strong>Первое обнаружение:</strong> {{ new Date(selectedIncident.firstSeen).toLocaleString() }}
            </div>
            <div>
              <strong>Последнее обнаружение:</strong> {{ new Date(selectedIncident.lastSeen).toLocaleString() }}
            </div>
            <div>
              <strong>Серьезность:</strong> {{ selectedIncident.severity }}
            </div>
            <div>
              <strong>Теги:</strong> {{ selectedIncident.tags.join(', ') || 'Нет тегов' }}
            </div>
          </div>
        </div>

        <div v-if="selectedIncident.additionalData && Object.keys(selectedIncident.additionalData).length"
          class="bg-gray-50 p-4 rounded-lg border border-gray-100">
          <p class="font-medium text-gray-700 mb-2">Дополнительные данные:</p>
          <ul class="list-disc list-inside space-y-1 ml-4">
            <li v-for="(val, key) in selectedIncident.additionalData" :key="key">
              <strong>{{ key }}:</strong> {{ val }}
            </li>
          </ul>
        </div>
      </div>

      <div class="mt-6 flex justify-end">
        <button type="button"
          class="inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-indigo-600 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:text-sm"
          @click="closeModal">
          Закрыть
        </button>
      </div>
    </div>
  </div>
</template>
