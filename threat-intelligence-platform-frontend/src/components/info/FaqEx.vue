<!-- ИСПРАВИТЬ ПРЫГАЮЩИЕ БЛОКИ -->
<template>
  <div class="max-w-3xl mx-auto space-y-8 px-4 py-6">
    <div class="text-center">
      <h1 class="text-4xl font-bold text-gray-900">Часто задаваемые вопросы</h1>
      <p class="text-gray-700 text-lg mt-4">
        Ответы на популярные вопросы о платформе
        <span class="font-semibold text-blue-600">Threat Intelligence Platform</span>
      </p>
    </div>

    <div class="space-y-4">
      <div v-for="(faq, index) in faqs" :key="index"
        class="w-full bg-white rounded-2xl overflow-hidden transition-colors duration-200 shadow-md">
        <button @click="faq.isOpen = !faq.isOpen" class="w-full px-6 py-4 flex items-center justify-between 
        text-left focus:outline-none">
          <h2 class="text-lg font-semibold text-gray-800">{{ faq.question }}</h2>
          <svg class="w-5 h-5 text-gray-600 transition-transform duration-200" :class="{ 'rotate-180': faq.isOpen }"
            xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
          </svg>
        </button>

        <div :style="{maxHeight: faq.isOpen ? '500px' : '0'}" class="overflow-hidden transition-all duration-300">
          <div class="px-6 pb-4 pt-4 text-gray-600 text-left">
            <p class="break-words overflow-x-hidden">{{ faq.answer }}</p>
            <div v-if="faq.links" class="mt-3 flex flex-wrap gap-3">
              <router-link v-for="link in faq.links" :key="link.to" :to="link.to"
                class="text-blue-600 hover:text-blue-800 text-sm font-medium">
                {{ link.text }} →
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="mt-8 pt-6 border-t border-gray-200">
      <p class="text-center text-gray-700">
        Не нашли ответ на свой вопрос?
        <router-link to="/contacts" class="text-blue-600 hover:text-blue-800 font-medium">
          Свяжитесь с нами
        </router-link>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const faqs = ref([
  {
    question: 'Что такое TIP?',
    answer:
      'Threat Intelligence Platform (TIP) — это современная платформа для управления и анализа угроз в области информационной безопасности. Она помогает организациям эффективно собирать, анализировать и реагировать на потенциальные угрозы безопасности.',
    isOpen: false,
    links: [{ to: '/about', text: 'Подробнее о платформе' }]
  },
  {
    question: 'Как зарегистрироваться на платформе?',
    answer:
      'Для регистрации перейдите на страницу регистрации и заполните необходимую форму. После заполнения вам нужно будет подтвердить email и согласиться с условиями использования и политикой конфиденциальности.',
    isOpen: false,
    links: [
      { to: '/register', text: 'Регистрация' },
      { to: '/privacy', text: 'Политика конфиденциальности' }
    ]
  },
  {
    question: 'Как работает импорт индикаторов компрометации (IOC)?',
    answer:
      'Платформа поддерживает несколько способов импорта IOC: через веб-интерфейс, API или автоматическую загрузку из внешних источников. Все импортированные данные автоматически нормализуются и проходят проверку на достоверность.',
    isOpen: false,
    links: [{ to: '/documentation', text: 'Документация по API' }]
  },
  {
    question: 'Как получить техническую поддержку?',
    answer:
      'Мы предоставляем несколько каналов связи с технической поддержкой: email, телефон и мессенджеры. Время ответа зависит от выбранного способа связи.',
    isOpen: false,
    links: [{ to: '/contacts', text: 'Контакты поддержки' }]
  },
  {
    question: 'Где найти документацию по использованию платформы?',
    answer:
      'Полная документация доступна в разделе "Документация". Там вы найдете руководства пользователя, API-документацию и примеры использования всех функций платформы.',
    isOpen: false,
    links: [{ to: '/documentation', text: 'Перейти к документации' }]
  }
])
</script>
