<script setup lang="ts">
const threats = [
  {
    id: 1,
    name: 'Подозрительная активность',
    description: 'Множественные попытки доступа',
    level: 'Критический',
    levelClass: 'bg-red-100 text-red-800',
    status: 'Активно',
    statusClass: 'bg-green-100 text-green-800',
    date: '2024-03-09 14:30',
    iconBg: 'bg-red-100',
    iconColor: 'text-red-600'
  },
  {
    id: 2,
    name: 'Попытка взлома',
    description: 'Брутфорс атака',
    level: 'Высокий',
    levelClass: 'bg-orange-100 text-orange-800',
    status: 'В обработке',
    statusClass: 'bg-yellow-100 text-yellow-800',
    date: '2024-03-09 13:45',
    iconBg: 'bg-orange-100',
    iconColor: 'text-orange-600'
  },
  {
    id: 3,
    name: 'Вредоносное ПО',
    description: 'Обнаружен троян',
    level: 'Средний',
    levelClass: 'bg-yellow-100 text-yellow-800',
    status: 'Закрыто',
    statusClass: 'bg-gray-100 text-gray-800',
    date: '2024-03-09 12:15',
    iconBg: 'bg-yellow-100',
    iconColor: 'text-yellow-600'
  }
]
</script>

<template>
  <div class="space-y-6 px-4 py-6">
    <div class="sm:flex sm:items-center sm:justify-between">
      <h1 class="text-2xl font-bold text-gray-900">Угрозы</h1>
      <button class="btn btn-primary mt-4 sm:mt-0">Добавить угрозу</button>
    </div>

    <div class="flex flex-col sm:flex-row space-y-4 sm:space-y-0 sm:space-x-4">
      <div class="flex-1 relative">
        <input type="text" placeholder="Поиск угроз..."
          class="w-full pl-10 pr-4 py-2 border border-gray-300 text-black rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent" />
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>
      </div>

      <div class="flex-shrink-0">
        <select
          class="w-full sm:w-auto pl-3 pr-10 py-2 border border-gray-300 text-black rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
          <option value="all">Все угрозы</option>
          <option value="critical">Критические</option>
          <option value="high">Высокие</option>
          <option value="medium">Средние</option>
          <option value="low">Низкие</option>
        </select>
      </div>
    </div>

    <div class="bg-white shadow-sm rounded-lg overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Угроза</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Уровень</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Статус</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Дата</th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Действия</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="threat in threats" :key="threat.id">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-start">
                  <div :class="['h-10 w-10 rounded-full flex items-center justify-center', threat.iconBg]">
                    <svg class="h-6 w-6" :class="threat.iconColor" fill="none" stroke="currentColor"
                      viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900">{{ threat.name }}</div>
                    <div class="text-sm text-gray-500">{{ threat.description }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="['inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium', threat.levelClass]">
                  {{ threat.level }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="['inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium', threat.statusClass]">
                  {{ threat.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ threat.date }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button class="text-primary-600 hover:text-primary-900">Подробнее</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="bg-white px-4 py-3 border-t border-gray-200 sm:px-6">
        <div class="flex items-center justify-between">
          <p class="text-sm text-gray-700">
            Показано <span class="font-medium">1</span> - <span class="font-medium">10</span> из <span
              class="font-medium">20</span> результатов
          </p>
          <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
            <button
              class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50">
              Предыдущая
            </button>
            <button
              class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
              1
            </button>
            <button
              class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-primary-50 text-sm font-medium text-primary-600">
              2
            </button>
            <button
              class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
              3
            </button>
            <button
              class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50">
              Следующая
            </button>
          </nav>
        </div>
      </div>
    </div>
  </div>
</template>
