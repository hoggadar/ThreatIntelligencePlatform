<template>
  <footer id="footer" class="full-width-footer bg-gray-900 text-gray-300 py-8 mt-12">
    <div class="w-full px-4 grid grid-cols-1 md:grid-cols-3 gap-8 text-sm">
      <div class="flex flex-col space-y-4">
        <!-- ДОРАБОТАТЬ ЧТОБЫ НА САЙТЕ СМОТРЕЛОСЬ РОВНО -->
        <RouterLink to="/dashboard" class="flex justify-center md:justify-start">
          <img src="/logo.svg" alt="Логотип колледжа" class="h-10" />
        </RouterLink>
        <p>© 2025, Threat Intelligence Platform. Все права защищены.</p>
        <p class="text-gray-500">Проект разработан в рамках выпускной квалификационной работы.</p>
      </div>

      <div class="flex flex-col">
        <h3 class="text-white text-lg font-semibold mb-2">Полезные ссылки</h3>
        <div class="grid grid-cols-2 gap-y-2 gap-x-2">
          <RouterLink to="/dashboard" class="hover:text-white transition">Панель управления</RouterLink>
          <RouterLink to="/policy" class="hover:text-white transition">Конфиденциальность</RouterLink>
          <RouterLink to="/threats" class="hover:text-white transition">Угрозы и инциденты</RouterLink>
          <RouterLink to="/contacts" class="hover:text-white transition">Контакты</RouterLink>
          <RouterLink to="/analytics" class="hover:text-white transition">Аналитика угроз</RouterLink>
          <RouterLink to="/faq" class="hover:text-white transition">FAQ</RouterLink>
          <RouterLink to="/about" class="hover:text-white transition col-span-2">О платформе</RouterLink>
        </div>
      </div>

      <div class="flex flex-col space-y-2 text-right">
        <p class="text-lg font-semibold text-white">Мы всегда на связи</p>
        <p class="text-sm text-gray-300">
          Почта:
          <a href="mailto:tip@support.com" class="text-blue-400 hover:underline">tip@support.com</a>
        </p>
        <p class="text-sm text-gray-300">Телефон: 8-017-017-17-17</p>
        <RouterLink to="/contacts" class="text-blue-400 hover:underline text-sm">
          Все контакты
        </RouterLink>
      </div>
    </div>

    <div class="mt-8 border-t border-gray-700 pt-4 text-center text-xs text-gray-500">
      Threat Intelligence Platform © 2025
    </div>
  </footer>
</template>

<script setup lang="ts">
</script>
