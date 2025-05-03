<template>
  <footer id="footer" class="full-width-footer">
    <div class="w-full px-4 py-6 grid grid-cols-1 md:grid-cols-3 gap-8 text-sm">
      <div class="flex flex-col space-y-2">
        <RouterLink to="/dashboard">
          <img src="/logo.svg" alt="Логотип колледжа" class="h-10 mx-auto" />
        </RouterLink>
        <p>© 2025, Threat Intelligence Platform. Все права защищены.</p>
        <p class="text-gray-300">Проект разработан в рамках выпускной квалификационной работы.</p>
      </div>
      <div class="flex flex-col space-y-1 text-blue-400">
        <a href="#" class="hover:underline">Новости</a>
        <a href="#" class="hover:underline">Объявления</a>
        <a href="#" class="hover:underline">Информация о колледже</a>
        <a href="#" class="hover:underline">Карта сайта</a>
      </div>
      <div class="flex flex-col items-end space-y-1">
        <p class="text-lg font-semibold text-white">Мы всегда на связи</p>
        <div class="text-right text-gray-300">
          <div class="footer-contacts py-4">
            <div class="contact-info">
              <p class="text-sm">
                <a href="mailto:tip@gmail.com" class="text-blue-400 hover:underline">tip@support.com</a>
              </p>
              <p class="text-sm">8-017-017-17-17</p>
            </div>
            <router-link to="/contacts" class="text-blue-400 hover:underline">
              Все контакты
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
</script>
