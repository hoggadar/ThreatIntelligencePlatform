import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:3000',  // Прокси на сервер на порту 3000
        changeOrigin: true,               // Изменяет origin на целевой сервер
        secure: false,                    // Для работы с http, если целевой сервер на https
        rewrite: (path) => path.replace(/^\/api/, ''),  // Убираем /api из пути, чтобы запросы шли на /auth/login и т.д.
      },
    },
  },
});