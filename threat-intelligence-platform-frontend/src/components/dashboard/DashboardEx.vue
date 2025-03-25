<script setup lang="ts">
const cards = [
  {
    title: 'Активные угрозы',
    badgeText: 'Критические',
    badgeColor: 'bg-red-100 text-red-800',
    value: 24,
    subtitle: 'За последние 24 часа',
    trend: '+12%',
    trendColor: 'text-red-600',
    progress: 75,
    progressColor: 'bg-red-500'
  },
  {
    title: 'Обработано сегодня',
    badgeText: 'Выполнено',
    badgeColor: 'bg-green-100 text-green-800',
    value: 156,
    subtitle: 'Среднее время обработки',
    trend: '15 мин',
    trendColor: 'text-green-600',
    progress: 90,
    progressColor: 'bg-green-500'
  },
  {
    title: 'Критические угрозы',
    badgeText: 'В процессе',
    badgeColor: 'bg-yellow-100 text-yellow-800',
    value: 3,
    subtitle: 'Требуют внимания',
    trend: '2 новых',
    trendColor: 'text-yellow-600',
    progress: 30,
    progressColor: 'bg-yellow-500'
  }
]

const threatTypes = [
  { name: 'Malware', percentage: 45, color: 'bg-red-500' },
  { name: 'Phishing', percentage: 30, color: 'bg-yellow-500' },
  { name: 'DDoS', percentage: 15, color: 'bg-blue-500' },
  { name: 'Other', percentage: 10, color: 'bg-gray-500' }
]
</script>
 
<template>
  <div class="space-y-6 px-4 py-6">
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

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Последние инциденты</h3>
        <div class="space-y-4">
          <div v-for="i in 3" :key="i" class="flex items-center space-x-4 p-4 bg-gray-50 rounded-lg">
            <div class="flex-shrink-0">
              <div class="w-10 h-10 rounded-full bg-red-100 flex items-center justify-center">
                <svg class="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 truncate">
                Попытка несанкционированного доступа
              </p>
              <p class="text-sm text-gray-500">
                IP: 192.168.1.{{ i * 100 }}
              </p>
            </div>
            <div class="inline-flex items-center text-sm text-gray-500">
              {{ i * 5 }}м назад
            </div>
          </div>
        </div>
      </div>

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
            <div class="w-16 text-right">
              <span class="text-sm font-medium text-gray-900">{{ type.percentage }}%</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
