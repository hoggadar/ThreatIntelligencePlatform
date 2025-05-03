<template>
  <div class="space-y-6 px-4 py-6">
    <div class="min-h-screen bg-gray-50 flex items-start justify-center py-10 px-4">
      <div class="w-full max-w-4xl bg-white rounded-3xl shadow-lg p-8 md:p-10 space-y-12">
        <div class="text-center">
          <h1 class="text-4xl font-bold text-gray-900">Контакты</h1>
          <p class="mt-3 text-lg text-gray-600">Свяжитесь с нами удобным для вас способом</p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="bg-gray-50 rounded-2xl p-6 flex items-center space-x-4">
            <div class="bg-blue-100 rounded-full p-3">
              <svg class="w-7 h-7 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
              </svg>
            </div>
            <div>
              <p class="text-lg font-semibold text-gray-900">8-017-017-17-17</p>
              <p class="text-gray-600">Телефон</p>
            </div>
          </div>

          <div class="bg-gray-50 rounded-2xl p-6 flex items-center space-x-4">
            <div class="bg-blue-100 rounded-full p-3">
              <svg class="w-7 h-7 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
            </div>
            <div>
              <p class="text-lg font-semibold text-gray-900">support@tip.com</p>
              <p class="text-gray-600">Email</p>
            </div>
          </div>

          <div class="bg-gray-50 rounded-2xl p-6 flex items-center space-x-4">
            <div class="bg-blue-100 rounded-full p-3">
              <svg class="w-7 h-7 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </div>
            <div>
              <p class="text-lg font-semibold text-gray-900">г. Новосибирск</p>
              <p class="text-gray-600">ул. Русская, 35</p>
            </div>
          </div>
        </div>

        <div class="bg-gray-50 rounded-2xl p-8">
          <h2 class="text-2xl font-bold text-gray-900 mb-6">Мы в социальных сетях</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <a href="https://github.com/hoggadar/ThreatIntelligencePlatform" target="_blank"
              class="flex items-center space-x-4 bg-white rounded-xl p-4 hover:shadow-md transition duration-300">
              <img src="/github.png" alt="GitHub" class="w-10 h-10 object-contain" />
              <div>
                <p class="font-semibold text-gray-900">GitHub</p>
                <p class="text-sm text-gray-600">Следите за обновлениями</p>
              </div>
            </a>

            <a href="https://t.me/Tamik017" target="_blank"
              class="flex items-center space-x-4 bg-white rounded-xl p-4 hover:shadow-md transition duration-300">
              <img src="/tg.png" alt="Telegram" class="w-10 h-10 object-contain" />
              <div>
                <p class="font-semibold text-gray-900">Telegram</p>
                <p class="text-sm text-gray-600">Оперативная поддержка</p>
              </div>
            </a>

            <a href="#" target="_blank"
              class="flex items-center space-x-4 bg-white rounded-xl p-4 hover:shadow-md transition duration-300">
              <img src="/linkedin.png" alt="LinkedIn" class="w-10 h-10 object-contain" />
              <div>
                <p class="font-semibold text-gray-900">LinkedIn</p>
                <p class="text-sm text-gray-600">Профессиональная сеть</p>
              </div>
            </a>

            <a href="#" target="_blank"
              class="flex items-center space-x-4 bg-white rounded-xl p-4 hover:shadow-md transition duration-300">
              <img src="/x.jpg" alt="Twitter" class="w-10 h-10 object-contain" />
              <div>
                <p class="font-semibold text-gray-900">Twitter</p>
                <p class="text-sm text-gray-600">Новости и обновления</p>
              </div>
            </a>

            <a href="#" target="_blank"
              class="flex items-center space-x-4 bg-white rounded-xl p-4 hover:shadow-md transition duration-300">
              <img src="/ds.png" alt="Discord" class="w-10 h-10 object-contain" />
              <div>
                <p class="font-semibold text-gray-900">Discord</p>
                <p class="text-sm text-gray-600">Сообщество разработчиков</p>
              </div>
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>