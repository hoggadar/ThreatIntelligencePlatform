<template>
  <section class="container mx-auto px-4 py-6 max-w-3xl ">
    <div class="bg-white rounded-3xl shadow-lg p-8 md:p-10 space-y-8 privacy-content-box ">
      <div class="">
        <h1 class="text-4xl font-bold text-gray-900 text-center">Политика конфиденциальности</h1>
        <p class="text-gray-700 text-lg mt-4">
          Настоящая политика конфиденциальности описывает, какие данные собираются при использовании платформы
          <span class="font-semibold text-blue-600">Threat Intelligence Platform</span>, как они обрабатываются и
          защищаются.
        </p>
      </div>

      <div class="space-y-8">
        <div class="bg-gray-50 rounded-2xl p-6">
          <h2 class="text-xl font-semibold text-gray-800 mb-4">1. Сбор информации</h2>
          <ul class="list-disc list-inside text-gray-700 space-y-2">
            <li>Данные, предоставленные пользователем при регистрации (имя, email и др.)</li>
            <li>Лог-файлы и техническая информация (IP-адрес, браузер, устройства)</li>
            <li>Аналитические данные для улучшения функциональности платформы</li>
            <li>Данные о взаимодействии с платформой и предпочтениях пользователя</li>
          </ul>
        </div>

        <div class="bg-gray-50 rounded-2xl p-6">
          <h2 class="text-xl font-semibold text-gray-800 mb-4">2. Использование информации</h2>
          <p class="text-gray-700 mb-2">Собранная информация используется для:</p>
          <ul class="list-disc list-inside text-gray-700 space-y-2">
            <li>Обеспечения доступа к сервисам платформы</li>
            <li>Поддержки и обслуживания пользователей</li>
            <li>Анализа и улучшения функциональности системы</li>
            <li>Обеспечения безопасности и предотвращения мошенничества</li>
          </ul>
        </div>

        <div class="bg-gray-50 rounded-2xl p-6">
          <h2 class="text-xl font-semibold text-gray-800 mb-4">3. Хранение и защита данных</h2>
          <p class="text-gray-700">
            Все данные хранятся на защищённых серверах с использованием современных методов шифрования. Мы применяем
            комплексные меры безопасности для предотвращения несанкционированного доступа, включая:
          </p>
          <ul class="list-disc list-inside text-gray-700 mt-2 space-y-2">
            <li>Шифрование данных при передаче и хранении</li>
            <li>Регулярное обновление систем безопасности</li>
            <li>Строгий контроль доступа к данным</li>
          </ul>
        </div>

        <div class="bg-gray-50 rounded-2xl p-6">
          <h2 class="text-xl font-semibold text-gray-800 mb-4">4. Раскрытие информации третьим лицам</h2>
          <p class="text-gray-700">
            Мы не передаём ваши персональные данные третьим лицам, за исключением случаев:
          </p>
          <ul class="list-disc list-inside text-gray-700 mt-2 space-y-2">
            <li>Предусмотренных законодательством</li>
            <li>Явно согласованных с вами</li>
            <li>Необходимых для защиты наших прав и собственности</li>
          </ul>
        </div>

        <div class="bg-gray-50 rounded-2xl p-6">
          <h2 class="text-xl font-semibold text-gray-800 mb-4">5. Права пользователей</h2>
          <p class="text-gray-700">
            Вы имеете право:
          </p>
          <ul class="list-disc list-inside text-gray-700 mt-2 space-y-2">
            <li>Запросить доступ к вашим данным</li>
            <li>Требовать обновления или удаления данных</li>
            <li>Ограничить обработку ваших данных</li>
            <li>Получить копию ваших данных в структурированном формате</li>
          </ul>
        </div>

        <div class="bg-gray-50 rounded-2xl p-6">
          <h2 class="text-xl font-semibold text-gray-800 mb-4">6. Изменения политики</h2>
          <p class="text-gray-700">
            Мы оставляем за собой право обновлять данную политику конфиденциальности. Все изменения вступают в силу
            с момента публикации на данной странице. При существенных изменениях мы уведомим вас через платформу
            или по электронной почте.
          </p>
        </div>
      </div>

      <div class="flex flex-col md:flex-row items-center justify-between gap-4 border-t pt-6">
        <p class="text-sm text-gray-500">
          Последнее обновление: май 2025
        </p>
        <div class="flex gap-4">
          <a href="mailto:support@tip.com" class="text-blue-600 hover:text-blue-800 text-sm">
            Связаться с нами
          </a>
          <router-link to="/contacts" class="text-blue-600 hover:text-blue-800 text-sm">
            Контакты
          </router-link>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
.privacy-content-box :not(h1):not(h2) {
  text-align: left;
}

.privacy-content-box h2 {
  text-align: center;
}
</style>