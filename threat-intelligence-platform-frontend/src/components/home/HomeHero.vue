<template>
  <div class="relative w-full min-w-0">
    <!-- Фоновый градиент и сетка на всю ширину -->
    <div class="absolute inset-0 w-screen left-1/2 right-1/2 -translate-x-1/2 z-0">
      <div class="w-full h-full bg-gradient-to-r from-indigo-900 to-purple-900 absolute inset-0"></div>
      <div class="absolute w-full h-full">
        <!-- Генерация анимированных частиц для фона -->
        <svg class="w-full h-full opacity-20" viewBox="0 0 100 100" preserveAspectRatio="none">
          <defs>
            <pattern id="cyber-grid" width="10" height="10" patternUnits="userSpaceOnUse">
              <path d="M 10 0 L 0 0 0 10" fill="none" stroke="rgba(255,255,255,0.3)" stroke-width="0.5"></path>
            </pattern>
          </defs>
          <rect width="100%" height="100%" fill="url(#cyber-grid)"></rect>

          <!-- Анимированные элементы -->
          <circle cx="20" cy="50" r="1" fill="#60A5FA" class="animate-ping">
            <animate attributeName="r" from="0.5" to="2" dur="4s" repeatCount="indefinite" />
          </circle>
          <circle cx="65" cy="30" r="1" fill="#818CF8" class="animate-ping" style="animation-delay: 1s;">
            <animate attributeName="r" from="0.5" to="2" dur="5s" repeatCount="indefinite" />
          </circle>
          <circle cx="85" cy="80" r="1" fill="#C4B5FD" class="animate-ping" style="animation-delay: 2s;">
            <animate attributeName="r" from="0.5" to="2" dur="3s" repeatCount="indefinite" />
          </circle>
        </svg>
      </div>
    </div>

    <!-- Основное содержимое -->
    <div class="relative w-full z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20 sm:py-24 lg:py-32">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <!-- Левая колонка - текст -->
          <div class="space-y-6 z-10">
            <div class="space-y-2">
              <p
                class="inline-block text-indigo-300 px-3 py-1 rounded-full text-sm font-semibold bg-indigo-800/50 backdrop-blur-sm">
                Защита для вашего бизнеса
              </p>
              <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold tracking-tight">
                <span class="text-white block">Threat Intelligence</span>
                <span
                  class="bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-violet-400 block">Platform</span>
              </h1>
            </div>

            <p class="text-xl text-indigo-100 max-w-2xl leading-relaxed">
              Современная платформа для мониторинга киберугроз, анализа безопасности и защиты вашей инфраструктуры от
              новейших векторов атак в режиме реального времени.
            </p>

            <div class="flex flex-col sm:flex-row gap-4 pt-4">
              <router-link to="/register"
                class="btn-primary text-center px-6 py-3 font-medium rounded-lg shadow-lg hover:shadow-indigo-500/30 transition-all duration-300">
                Начать бесплатно
              </router-link>
              <router-link to="/login"
                class="btn-secondary text-center px-6 py-3 font-medium rounded-lg shadow-lg transition-all duration-300">
                Войти в систему
              </router-link>
            </div>

            <div class="pt-6 grid grid-cols-2 gap-6">
              <div class="flex items-center">
                <div
                  class="flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-md bg-indigo-800/80 backdrop-blur-sm">
                  <svg class="h-6 w-6 text-indigo-200" xmlns="http://www.w3.org/2000/svg" fill="none"
                    viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                  </svg>
                </div>
                <p class="ml-4 text-base text-indigo-100">Безопасность<br>на первом месте</p>
              </div>
              <div class="flex items-center">
                <div
                  class="flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-md bg-indigo-800/80 backdrop-blur-sm">
                  <svg class="h-6 w-6 text-indigo-200" xmlns="http://www.w3.org/2000/svg" fill="none"
                    viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M13 10V3L4 14h7v7l9-11h-7z" />
                  </svg>
                </div>
                <p class="ml-4 text-base text-indigo-100">Мгновенная<br>реакция</p>
              </div>
            </div>
          </div>

          <!-- Правая колонка - визуализация безопасности -->
          <div class="relative lg:h-96 z-10">
            <div class="absolute inset-0 flex items-center justify-center">
              <div class="w-full max-w-lg h-full">
                <!-- Главный щит -->
                <div
                  class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-60 h-60 lg:w-80 lg:h-80">
                  <!-- Пульсирующие круги -->
                  <div class="absolute inset-0 w-full h-full rounded-full bg-blue-500/20 animate-pulse"></div>
                  <div class="absolute inset-[10%] w-[80%] h-[80%] rounded-full bg-indigo-500/30 animate-pulse"
                    style="animation-delay: 0.5s"></div>
                  <div class="absolute inset-[20%] w-[60%] h-[60%] rounded-full bg-violet-500/40 animate-pulse"
                    style="animation-delay: 1s"></div>

                  <!-- Центральный щит -->
                  <div class="absolute inset-[25%] w-[50%] h-[50%] flex items-center justify-center">
                    <svg class="w-full h-full text-white" viewBox="0 0 24 24" fill="none"
                      xmlns="http://www.w3.org/2000/svg">
                      <path
                        d="M20.9099 11.1198C20.9099 16.0098 17.3599 20.5998 12.5099 21.9298C12.1799 22.0198 11.8199 22.0198 11.4899 21.9298C6.63988 20.5998 3.08988 16.0098 3.08988 11.1198V6.72976C3.08988 5.90976 3.70988 5.17976 4.47988 5.01976L11.4299 3.01976C11.8099 2.92976 12.1999 2.92976 12.5799 3.01976L19.5299 5.01976C20.2999 5.17976 20.9199 5.90976 20.9199 6.72976L20.9099 11.1198Z"
                        stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                      <path
                        d="M12 12.5C13.1046 12.5 14 11.6046 14 10.5C14 9.39543 13.1046 8.5 12 8.5C10.8954 8.5 10 9.39543 10 10.5C10 11.6046 10.8954 12.5 12 12.5Z"
                        stroke="currentColor" stroke-width="1.5" fill="rgba(255,255,255,0.2)" />
                      <path d="M12 12.5V15.5" stroke="currentColor" stroke-width="1.5" stroke-miterlimit="10"
                        stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                  </div>
                </div>

                <!-- Анимированные атаки -->
                <div class="w-full h-full relative">
                  <!-- Радар-сканирование -->
                  <div
                    class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-60 h-60 lg:w-80 lg:h-80 border-2 border-indigo-500/30 rounded-full">
                    <div class="absolute inset-0 origin-center animate-spin" style="animation-duration: 8s;">
                      <div
                        class="absolute top-1/2 left-1/2 w-[50%] h-[1px] bg-gradient-to-r from-transparent to-indigo-400">
                      </div>
                    </div>
                  </div>

                  <!-- Атаки точки -->
                  <div class="absolute top-[20%] left-[15%] w-3 h-3 bg-red-500 rounded-full animate-ping"
                    style="animation-duration: 3s;"></div>
                  <div class="absolute top-[70%] left-[25%] w-2 h-2 bg-yellow-400 rounded-full animate-ping"
                    style="animation-duration: 4s; animation-delay: 1s;"></div>
                  <div class="absolute top-[30%] right-[20%] w-4 h-4 bg-blue-400 rounded-full animate-ping"
                    style="animation-duration: 5s; animation-delay: 2s;"></div>
                  <div class="absolute top-[60%] right-[15%] w-3 h-3 bg-green-400 rounded-full animate-ping"
                    style="animation-duration: 3.5s; animation-delay: 1.5s;"></div>
                </div>

                <!-- Интерактивные элементы -->
                <div
                  class="absolute bottom-0 left-0 lg:right-auto lg:top-0 lg:bottom-auto p-3 bg-indigo-800/60 backdrop-blur-sm rounded-lg">
                  <div class="flex items-center space-x-1">
                    <span class="inline-block w-2 h-2 bg-green-400 rounded-full"></span>
                    <span class="text-xs text-indigo-100 animate-pulse">Защита активна</span>
                  </div>
                </div>

                <div
                  class="absolute bottom-0 right-0 lg:left-auto lg:top-1/4 lg:bottom-auto p-3 bg-indigo-800/60 backdrop-blur-sm rounded-lg">
                  <div class="flex items-center space-x-1">
                    <span class="inline-block w-2 h-2 bg-blue-400 rounded-full animate-ping"></span>
                    <span class="text-xs text-indigo-100">Сканирование угроз</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.btn-primary {
  @apply bg-gradient-to-r from-indigo-500 to-purple-500 text-white border border-indigo-500;
}

.btn-secondary {
  @apply bg-indigo-900/50 text-indigo-100 border border-indigo-700/50 backdrop-blur-sm;
}
</style>