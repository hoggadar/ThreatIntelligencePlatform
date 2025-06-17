import axios from "axios";
import { getCookie, deleteCookie } from "../utils/cookies";

// Имя cookie для токена
const TOKEN_COOKIE_NAME = "auth_token";

const instance = axios.create({
  baseURL: "http://localhost:8888/api",
  headers: {
    "Content-Type": "application/json",
  },
});

instance.interceptors.request.use(
  (config) => {
    // Получаем токен из cookie
    const token = getCookie(TOKEN_COOKIE_NAME);
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Функция для очистки токена
const clearToken = () => {
  // Удаляем токен из cookie
  deleteCookie(TOKEN_COOKIE_NAME, "/");

  // Очищаем localStorage
  try {
    localStorage.removeItem(TOKEN_COOKIE_NAME);
    localStorage.removeItem("auth_token_debug");
  } catch (e) {
    // Игнорируем ошибки localStorage
  }
};

// Список URL-путей, для которых не делаем автоматический редирект при 403
const noRedirectOn403Paths = ["/User/GetById/", "/User/GetByEmail/"];

// Перехватчик ответов для обработки ошибок
instance.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Проверяем URL запроса, чтобы определить, нужно ли делать редирект при 403
    const requestUrl = error.config?.url;
    const is403Error = error.response && error.response.status === 403;
    const shouldSkipRedirect =
      is403Error &&
      requestUrl &&
      noRedirectOn403Paths.some((path) => requestUrl.includes(path));

    // Если получили ошибку 401 (Unauthorized) или 403 (Forbidden) для неисключенных путей
    if (
      error.response &&
      (error.response.status === 401 ||
        (error.response.status === 403 && !shouldSkipRedirect))
    ) {
      // Очищаем токен
      clearToken();

      // Перенаправляем на страницу входа, если мы не на ней
      // Добавляем проверку window, чтобы избежать ошибок при SSR
      if (typeof window !== 'undefined') {
      const currentPath = window.location.pathname;
      if (currentPath !== "/login" && currentPath !== "/register") {
        // Используем location.href вместо router.push, так как router может быть недоступен здесь
        window.location.href = "/login";
        }
      }
    }
    return Promise.reject(error);
  }
);

export default instance;
