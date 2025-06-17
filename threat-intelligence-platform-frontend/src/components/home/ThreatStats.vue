<template>
  <div class="bg-gray-50 py-16">
    <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <div class="max-w-4xl mx-auto text-center mb-12">
        <h2 class="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">
          Глобальная статистика киберугроз
        </h2>
        <p class="mt-3 text-xl text-gray-500 sm:mt-4">
          Наша платформа ежедневно анализирует данные для выявления и предотвращения новейших векторов атак
        </p>
        <div class="flex items-center justify-center mt-6">
          <span
            class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-indigo-100 text-indigo-800">
            <span class="w-2 h-2 rounded-full bg-indigo-500 mr-2 animate-pulse"></span>
            Данные обновляются в реальном времени
          </span>
        </div>
      </div>

      <!-- Основные статистики с анимированным счётчиком -->
      <dl class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        <div v-for="(stat, index) in stats" :key="index"
          class="bg-white overflow-hidden shadow rounded-lg p-6 border-t-4 transition-all hover:shadow-lg"
          :class="stat.colorClass">
          <dt class="text-sm font-medium text-gray-500 truncate">{{ stat.name }}</dt>
          <dd class="mt-1 text-3xl font-bold" :class="stat.textColorClass">
            {{ stat.prefix }}{{ formattedValues[index] }}{{ stat.suffix }}
          </dd>
          <div class="mt-3">
            <span :class="`text-xs ${stat.trendClass} font-medium`">{{ stat.trend }} </span>
            <span class="text-xs text-gray-500">{{ stat.period }}</span>
          </div>
        </div>
      </dl>

      <!-- График распределения типов атак -->
      <div class="mt-16 bg-white shadow rounded-lg p-6">
        <div class="flex flex-col md:flex-row items-start justify-between mb-6">
          <div>
            <h3 class="text-lg font-medium text-gray-900">Распределение типов атак</h3>
            <p class="text-sm text-gray-500">Данные за последние 30 дней</p>
          </div>
          <div class="mt-4 md:mt-0 flex space-x-2">
            <button v-for="(option, idx) in viewOptions" :key="idx" @click="activeView = option.id"
              class="px-3 py-1 text-sm rounded-md transition-colors" :class="activeView === option.id
                ? 'bg-indigo-100 text-indigo-700'
                : 'bg-gray-100 text-gray-500 hover:bg-gray-200'">
              {{ option.name }}
            </button>
          </div>
        </div>

        <!-- Графики -->
        <div class="grid grid-cols-1 lg:grid-cols-5 gap-4">
          <!-- Левая колонка: карта -->
          <div class="lg:col-span-3 flex flex-col items-center pt-10">
            <div v-if="activeView === 'distribution'"
              class="w-full max-w-[700px] aspect-[2/1] rounded-xl border-2 border-indigo-300 shadow-inner bg-white overflow-hidden flex flex-col items-center">
              <v-chart :option="pieOption" autoresize style="width:100%;height:100%" />
            </div>
            <div v-if="activeView === 'timeline'"
              class="w-full max-w-[700px] aspect-[2/1] rounded-xl border-2 border-indigo-300 shadow-inner bg-white overflow-hidden flex flex-col items-center">
              <v-chart :option="lineOption" autoresize style="width:100%;height:100%" />
            </div>
            <div v-if="activeView === 'geo'"
              class="w-full max-w-[700px] aspect-[16/6] min-h-[350px] rounded-xl border-2 border-indigo-300 shadow-inner bg-white overflow-hidden flex flex-col items-center">
              <v-chart :option="geoOption" style="width:100%;height:100%" />
              <div class="mt-2 text-xs text-gray-500 text-center w-full">География атак (по городам)</div>
            </div>
          </div>
          <!-- Правая колонка: текст -->
          <div class="lg:col-span-2 grid grid-cols-1 gap-4 mt-6 lg:mt-0">
            <div class="bg-gray-50 p-4 rounded-lg">
              <h4 class="text-sm font-medium text-gray-700">Ключевые данные:</h4>
              <ul class="mt-2 text-sm text-gray-600 space-y-2 text-left">
                <li v-for="(fact, idx) in keyFacts" :key="idx" class="flex items-center">
                  <svg class="h-4 w-4 text-indigo-600 mr-2 min-w-[16px] min-h-[16px]" fill="none" stroke="currentColor"
                    viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  {{ fact }}
                </li>
              </ul>
            </div>

            <!-- <div class="bg-indigo-50 p-4 rounded-lg border-l-4 border-indigo-500">
              <div class="flex items-center justify-between">
                <h4 class="text-sm font-medium text-indigo-900">API обновлений</h4>
                <div class="flex items-center">
                  <span class="inline-block w-2 h-2 rounded-full bg-green-500 mr-1 animate-pulse"></span>
                  <span class="text-xs text-indigo-700">{{ updateStatus }}</span>
                </div>
              </div>
              <p class="mt-2 text-sm text-indigo-700">
                {{ updateInfo }}
              </p>
              <div class="mt-3 text-xs text-indigo-900 font-mono bg-indigo-100 p-2 rounded overflow-x-auto">
                GET /api/v1/threats/stats?period=daily
              </div>
            </div>

            <div class="bg-white border border-gray-200 p-4 rounded-lg hover:shadow-md transition-shadow">
              <div class="flex justify-between items-center">
                <h4 class="text-sm font-medium text-gray-700">Последнее обновление</h4>
                <span class="text-xs text-gray-500">{{ lastUpdate }}</span>
              </div>
              <div class="mt-2 space-y-2">
                <div v-for="(update, idx) in recentUpdates" :key="idx" class="flex items-start justify-between">
                  <div class="flex items-center">
                    <span :class="`inline-block w-2 h-2 rounded-full mt-1.5 mr-2 ${update.color}`"></span>
                    <p class="text-xs font-medium text-gray-700">{{ update.title }}</p>
                  </div>
                  <span class="text-xs text-gray-500 ml-2 whitespace-nowrap">{{ update.time }}</span>
                </div>
              </div>
            </div> -->
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue';
import * as echarts from 'echarts/core';
import { use } from 'echarts/core';
import VChart from 'vue-echarts';
import { PieChart, LineChart, ScatterChart, MapChart } from 'echarts/charts';
import { TitleComponent, TooltipComponent, LegendComponent, GeoComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';

use([
  PieChart, LineChart, ScatterChart, MapChart,
  TitleComponent, TooltipComponent, LegendComponent, GeoComponent, CanvasRenderer
]);

// Статистические данные
const stats = [
  {
    name: 'Обнаружено угроз',
    value: 2843917,
    increment: 10,
    format: 'number',
    prefix: '',
    suffix: '',
    trend: '+14.2%',
    period: 'За последний месяц',
    colorClass: 'border-indigo-600',
    textColorClass: 'text-indigo-600',
    trendClass: 'text-red-600'
  },
  {
    name: 'Критические уязвимости',
    value: 8392,
    increment: 2,
    format: 'number',
    prefix: '',
    suffix: '',
    trend: '+5.7%',
    period: 'За последний месяц',
    colorClass: 'border-red-600',
    textColorClass: 'text-red-600',
    trendClass: 'text-red-600'
  },
  {
    name: 'Предотвращено атак',
    value: 1407329,
    increment: 20,
    format: 'number',
    prefix: '',
    suffix: '',
    trend: '+18.3%',
    period: 'За последний месяц',
    colorClass: 'border-green-600',
    textColorClass: 'text-green-600',
    trendClass: 'text-green-600'
  },
  {
    name: 'Средний ущерб',
    value: 3.92,
    increment: 0.001,
    format: 'currency',
    prefix: '$',
    suffix: 'M',
    trend: '+8.1%',
    period: 'За последний год',
    colorClass: 'border-yellow-600',
    textColorClass: 'text-yellow-600',
    trendClass: 'text-red-600'
  }
];

// Типы атак с анимированными значениями
const attackTypes = ref([
  { name: 'Malware', percentage: 42, colorClass: 'bg-red-500', gradientClass: 'bg-gradient-to-t from-red-600 to-red-400' },
  { name: 'Phishing', percentage: 36, colorClass: 'bg-blue-500', gradientClass: 'bg-gradient-to-t from-blue-600 to-blue-400' },
  { name: 'DDoS', percentage: 12, colorClass: 'bg-yellow-500', gradientClass: 'bg-gradient-to-t from-yellow-600 to-yellow-400' },
  { name: 'SQL Injection', percentage: 8, colorClass: 'bg-green-500', gradientClass: 'bg-gradient-to-t from-green-600 to-green-400' },
  { name: 'Прочие', percentage: 2, colorClass: 'bg-purple-500', gradientClass: 'bg-gradient-to-t from-purple-600 to-purple-400' }
]);

// Ключевые факты
const keyFacts = [
  '78% атак нацелены на малый и средний бизнес',
  '94% вредоносных программ доставляются по электронной почте',
  'DDoS-атаки выросли на 87% по сравнению с прошлым годом',
  'Среднее время обнаружения утечки: 197 дней',
  'Стоимость утечки данных выросла на 12% за год'
];

// Переключение вариантов отображения
const viewOptions = [
  { id: 'distribution', name: 'Распределение' },
  { id: 'timeline', name: 'Временная шкала' },
  { id: 'geo', name: 'География атак' }
];
const activeView = ref('distribution');

// Статус обновления
const updateStatus = ref('Онлайн');
const updateInfo = ref('Данные обновляются каждые 60 секунд автоматически');

// Последние обновления
const lastUpdate = ref('12:45:32');
const recentUpdates = [
  { title: 'Обнаружена новая угроза: XSS-атака', time: '12:45', color: 'bg-red-500' },
  { title: 'Блокирована попытка брутфорс атаки', time: '12:30', color: 'bg-yellow-500' },
  { title: 'Обновлены правила фильтрации', time: '12:15', color: 'bg-green-500' }
];

// Форматирование значений с анимацией
const displayValues = ref(stats.map(stat => 0));
const formattedValues = computed(() => {
  return displayValues.value.map((value, index) => {
    if (stats[index].format === 'currency') {
      return value.toFixed(2);
    }
    return new Intl.NumberFormat().format(Math.floor(value));
  });
});

let animationInterval: number;
let updateInterval: number;
let apiSimulationInterval: number;

// Загрузка и регистрация карты мира
onMounted(async () => {
  if (!echarts.getMap('world')) {
    try {
      const res = await fetch('/maps/world.json');
      const worldJson = await res.json();
      echarts.registerMap('world', worldJson);
    } catch (e) {
      // eslint-disable-next-line no-console
      console.error('Не удалось загрузить карту мира для ECharts', e);
    }
  }

  // Анимация увеличения счетчиков
  const duration = 2000; // 2 секунды
  const steps = 20;
  const stepDuration = duration / steps;

  displayValues.value = stats.map(stat => 0);

  let step = 0;
  animationInterval = window.setInterval(() => {
    step++;

    displayValues.value = displayValues.value.map((val, i) => {
      const target = stats[i].value;
      return val + (target - val) / (steps - step + 1);
    });

    if (step >= steps) {
      clearInterval(animationInterval);
      displayValues.value = stats.map(stat => stat.value);
    }
  }, stepDuration);

  // Имитация обновления данных через API
  apiSimulationInterval = window.setInterval(() => {
    updateStatus.value = 'Получение данных...';
    // Имитация задержки API
    setTimeout(() => {
      // Обновление статистики
      stats.forEach((stat, idx) => {
        stat.value += stat.increment * (Math.random() > 0.5 ? 1 : -1) * Math.random() * 10;
        displayValues.value[idx] = stat.value;
      });

      // Обновление распределения типов атак
      attackTypes.value.forEach(type => {
        type.percentage += Math.random() > 0.5 ? 1 : -1;
        if (type.percentage < 1) type.percentage = 1;
        if (type.percentage > 50) type.percentage = 50;
      });

      // Нормализация процентов, чтобы сумма была 100%
      const total = attackTypes.value.reduce((sum, type) => sum + type.percentage, 0);
      attackTypes.value.forEach(type => {
        type.percentage = Math.round(type.percentage / total * 100);
      });

      // Обновление последнего обновления
      const now = new Date();
      lastUpdate.value = `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}:${now.getSeconds().toString().padStart(2, '0')}`;

      // Иногда добавляем новое обновление
      if (Math.random() > 0.7) {
        const updates = [
          { title: 'Обнаружена попытка SQL-инъекции', color: 'bg-red-500' },
          { title: 'Заблокирован фишинговый сайт', color: 'bg-yellow-500' },
          { title: 'Обновлены сигнатуры вредоносного ПО', color: 'bg-green-500' },
          { title: 'Выявлена уязвимость в CMS', color: 'bg-red-500' },
          { title: 'Предотвращена MITM-атака', color: 'bg-yellow-500' }
        ];

        const randomUpdate = updates[Math.floor(Math.random() * updates.length)];
        recentUpdates.unshift({
          title: randomUpdate.title,
          time: `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}`,
          color: randomUpdate.color
        });

        // Ограничиваем список обновлений
        if (recentUpdates.length > 3) {
          recentUpdates.pop();
        }
      }

      updateStatus.value = 'Онлайн';
    }, 800);
  }, 3600000); // Обновление раз в час

  // Имитация обновления текста
  updateInterval = window.setInterval(() => {
    const messages = [
      'Данные обновляются каждые 60 секунд автоматически',
      'Выполняется мониторинг 24/7/365',
      'Подключено к глобальной сети мониторинга угроз',
      'Активно 87 источников данных о киберугрозах'
    ];

    updateInfo.value = messages[Math.floor(Math.random() * messages.length)];
  }, 10000); // Обновление каждые 10 секунд
});

onBeforeUnmount(() => {
  if (animationInterval) clearInterval(animationInterval);
  if (updateInterval) clearInterval(updateInterval);
  if (apiSimulationInterval) clearInterval(apiSimulationInterval);
});

// Опции для круговой диаграммы
const pieOption = ref({
  tooltip: {
    trigger: 'item',
    formatter: '{b}: {d}%'
  },
  legend: {
    orient: 'vertical',
    left: 'left',
    textStyle: {
      color: '#3730a3',
      fontWeight: 'bold',
      fontSize: 14
    }
  },
  series: [
    {
      name: 'Тип атаки',
      type: 'pie',
      radius: ['50%', '80%'],
      avoidLabelOverlap: false,
      itemStyle: {
        borderRadius: 8,
        borderColor: '#fff',
        borderWidth: 2
      },
      label: {
        show: true,
        position: 'outside',
        formatter: '{b}: {d}%'
      },
      emphasis: {
        label: {
          show: true,
          fontSize: 18,
          fontWeight: 'bold'
        }
      },
      labelLine: {
        show: true
      },
      data: attackTypes.value.map(type => ({ value: type.percentage, name: type.name }))
    }
  ]
});

// Обновление данных диаграммы при изменении attackTypes
watch(attackTypes, (newTypes) => {
  pieOption.value.series[0].data = newTypes.map(type => ({ value: type.percentage, name: type.name }));
}, { deep: true });

// Линейный график (временная шкала)
const lineOption = ref({
  tooltip: { trigger: 'axis' },
  legend: { data: ['Обнаружено', 'Предотвращено'] },
  grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
  xAxis: {
    type: 'category',
    boundaryGap: false,
    data: ['1 мая', '5 мая', '10 мая', '15 мая', '20 мая', '25 мая', '31 мая']
  },
  yAxis: { type: 'value' },
  series: [
    {
      name: 'Обнаружено',
      type: 'line',
      data: [120, 132, 101, 134, 90, 230, 210],
      smooth: true,
      lineStyle: { color: '#6366f1', width: 3 },
      areaStyle: { color: 'rgba(99,102,241,0.1)' }
    },
    {
      name: 'Предотвращено',
      type: 'line',
      data: [220, 182, 191, 234, 290, 330, 310],
      smooth: true,
      lineStyle: { color: '#dc2626', width: 3 },
      areaStyle: { color: 'rgba(220,38,38,0.08)' }
    }
  ]
});

// География атак (scatter на карте мира)
const geoOption = ref({
  tooltip: {
    trigger: 'item',
    formatter: function (params: any) {
      return params.data ? `${params.data.name}<br/>Атак: ${params.data.value[2]}` : '';
    }
  },
  geo: {
    map: 'world',
    roam: false,
    zoom: 1.3,
    minZoom: 1.3,
    maxZoom: 1.3,
    label: { show: false },
    itemStyle: {
      areaColor: '#e0e7ef',
      borderColor: '#6366f1',
      borderWidth: 1
    },
    emphasis: {
      itemStyle: {
        areaColor: '#a5b4fc'
      }
    }
  },
  series: [
    {
      name: 'Атаки',
      type: 'scatter',
      coordinateSystem: 'geo',
      data: [
        { name: 'Москва', value: [37.6173, 55.7558, 120] },
        { name: 'Лондон', value: [-0.1276, 51.5074, 80] },
        { name: 'Нью-Йорк', value: [-74.006, 40.7128, 95] },
        { name: 'Пекин', value: [116.4074, 39.9042, 110] },
        { name: 'Берлин', value: [13.405, 52.52, 60] },
        { name: 'Токио', value: [139.6917, 35.6895, 70] }
      ],
      symbolSize: function (val: number[]) { return Math.sqrt(val[2]) * 2; },
      encode: { tooltip: 2 },
      itemStyle: {
        color: '#ef4444',
        shadowBlur: 10,
        shadowColor: 'rgba(239,68,68,0.3)'
      }
    }
  ]
});
</script>

<style scoped>
.path-animation {
  stroke-dasharray: 1000;
  stroke-dashoffset: 1000;
  animation: dash 4s linear forwards;
}

.line-animation {
  stroke-dasharray: 100;
  stroke-dashoffset: 100;
  animation: dash 2s linear forwards;
}

@keyframes dash {
  to {
    stroke-dashoffset: 0;
  }
}
</style>