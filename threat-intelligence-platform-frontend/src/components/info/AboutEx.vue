<template>
  <section class="container mx-auto px-4 py-6 max-w-3xl">
    <div class="bg-white rounded-3xl shadow-lg p-8 md:p-10 space-y-8">
      <h1 class="text-4xl font-bold text-gray-900 text-center">О платформе</h1>

      <p class="text-gray-700 text-lg text-left">
        <span class="font-semibold text-blue-600">Threat Intelligence Platform</span> — это современное веб-приложение
        для сбора, анализа и визуализации индикаторов компрометации (IoC), созданное для повышения кибербезопасности
        организаций.
      </p>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div>
          <h2 class="text-xl font-semibold text-gray-800 mb-2">Цели проекта</h2>
          <ul class="list-disc list-inside text-gray-700 space-y-1 text-left ">
            <li>Сбор индикаторов угроз из различных источников</li>
            <li>Нормализация и структурирование данных</li>
            <li>Аналитические инструменты для оценки угроз</li>
            <li>Визуализация угроз и IoC</li>
          </ul>
        </div>
        <div>
          <h2 class="text-xl font-semibold text-gray-800 mb-2">Ключевые функции</h2>
          <ul class="list-disc list-inside text-gray-700 space-y-1 text-left">
            <li>Панель управления с обзором состояния системы</li>
            <li>Модуль угроз с фильтрацией и поиском IoC</li>
            <li>Интерактивная аналитика и графики</li>
            <li>Контактная информация и справочные ресурсы</li>
          </ul>
        </div>
      </div>

      <div class="bg-gray-50 rounded-2xl p-6 flex flex-col md:flex-row gap-6">
        <div class="w-full md:w-1/2">
          <h2 class="text-xl font-semibold text-gray-800 mb-2">Разработка</h2>
          <p class="text-gray-700">
            Платформа разработана в рамках выпускной квалификационной работы.
          </p>
          <div class="mt-6">
            <span class="font-semibold text-gray-700">Стек технологий:</span>
            <div class="inline-flex flex-wrap gap-2 mt-1 items-center">
              <span class="bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs font-semibold">Vue 3</span>
              <span class="bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs font-semibold">TailwindCSS</span>
              <span class="bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs font-semibold">Go</span>
              <span class="bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs font-semibold">Docker</span>
            </div>
          </div>
        </div>

        <div class="w-full md:w-1/2 flex flex-col items-center text-center">
          <h2 class="text-xl font-semibold text-gray-800 mb-2">Контакты</h2>
          <p class="text-gray-700">
            По вопросам сотрудничества и поддержки:<br>
            <a href="mailto:support@tip.com" class="text-blue-500 hover:underline font-medium">support@tip.com</a>
          </p>
          <router-link to="/contacts"
            class="mt-3 inline-block bg-blue-600 text-white px-5 py-2 rounded-xl font-semibold shadow hover:bg-blue-700 transition">
            Все контакты
          </router-link>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>
