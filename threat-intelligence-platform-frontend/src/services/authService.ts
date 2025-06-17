import axios from "../api/axios";
import { ref, computed } from "vue";
import { jwtDecode } from "jwt-decode";
import {
  setCookie,
  getCookie,
  deleteCookie,
  hasCookie,
} from "../utils/cookies";

// Имена cookie для токенов
const TOKEN_COOKIE_NAME = "auth_token";

interface LoginPayload {
  email: string;
  password: string;
}

interface RegisterPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

interface DecodedJwtPayload {
  [key: string]: any;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
}

const isAuthenticated = ref(false);

// Получение токена из cookie
const getToken = (): string | null => {
  return getCookie(TOKEN_COOKIE_NAME);
};

const checkAuth = () => {
  // Проверка наличия cookie с токеном
  const hasToken = hasCookie(TOKEN_COOKIE_NAME);
  isAuthenticated.value = hasToken;
  return isAuthenticated.value;
};

export const login = async ({ email, password }: LoginPayload) => {
  try {
    const response = await axios.post("/auth/login", { email, password });

    if (response.data && response.data.token) {
      const token = response.data.token;

      // Сохраняем токен в cookie
      setCookie(TOKEN_COOKIE_NAME, token, 1, "/");

      // Резервное сохранение флага для совместимости
      try {
        localStorage.setItem("auth_token_debug", "token_exists");
      } catch (e) {
        // Игнорируем ошибки localStorage
      }

      isAuthenticated.value = true;
    }
    return response.data;
  } catch (err: any) {
    if (err.response) {
      // Для всех случаев неверной авторизации (401, 404, неверные учетные данные)
      if (
        err.response.status === 400 ||
        err.response.status === 401 ||
        err.response.status === 404
      ) {
        throw new Error("Неверный email или пароль");
      } else {
        throw new Error("Ошибка авторизации");
      }
    } else if (err.request) {
      // Запрос был сделан, но не получен ответ
      throw new Error("Сервер недоступен. Пожалуйста, попробуйте позже");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

export const register = async ({
  firstName,
  lastName,
  email,
  password,
}: RegisterPayload) => {
  try {
    const response = await axios.post("/auth/signup", {
      firstName,
      lastName,
      email,
      password,
    });
    return response.data;
  } catch (err: any) {
    if (err.response) {
      const status = err.response.status;
      const errorData = err.response.data;

      if (status === 400) {
        // Обработка ошибок валидации
        if (
          errorData?.message?.toLowerCase().includes("email") &&
          (errorData?.message?.toLowerCase().includes("exist") ||
            errorData?.message?.toLowerCase().includes("already"))
        ) {
          throw new Error("Пользователь с таким email уже существует");
        } else if (
          errorData?.message?.toLowerCase().includes("password") ||
          errorData?.message?.toLowerCase().includes("пароль")
        ) {
          throw new Error(errorData.message || "Проблема с паролем");
        } else if (errorData?.errors) {
          // Если сервер возвращает список ошибок валидации
          const errors = Object.values(
            errorData.errors as Record<string, string[]>
          ).flat();
          if (errors.length > 0) {
            throw new Error(errors[0]);
          }
        }
        throw new Error(
          errorData?.message || "Некорректные данные для регистрации"
        );
      } else if (status === 409) {
        // Конфликт (обычно дублирование email)
        throw new Error("Пользователь с таким email уже существует");
      } else {
        throw new Error(errorData?.message || "Ошибка регистрации");
      }
    } else if (err.request) {
      // Запрос был сделан, но не получен ответ
      throw new Error("Сервер недоступен. Пожалуйста, попробуйте позже");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

export const logout = () => {
  // Удаляем токен из всех возможных мест хранения

  // 1. Удаляем из cookie
  deleteCookie(TOKEN_COOKIE_NAME, "/");

  // 2. Очищаем localStorage
  try {
    localStorage.removeItem(TOKEN_COOKIE_NAME);
    localStorage.removeItem("auth_token_debug");
  } catch (e) {
    // Игнорируем ошибки localStorage
  }

  // 3. Очищаем sessionStorage
  try {
    sessionStorage.removeItem(`tip_${TOKEN_COOKIE_NAME}`);
    sessionStorage.removeItem(TOKEN_COOKIE_NAME);
  } catch (e) {
    // Игнорируем ошибки sessionStorage
  }

  // 4. Для полной очистки также устанавливаем куку с истекшим сроком действия
  try {
    document.cookie = `${TOKEN_COOKIE_NAME}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/;`;
  } catch (e) {
    // Игнорируем ошибки
  }

  // Обновляем состояние авторизации
  isAuthenticated.value = false;
};

export const getCurrentUser = async () => {
  const token = getToken();
  if (!token) throw new Error("Нет токена");

  const decodedToken = jwtDecode<DecodedJwtPayload>(token);

  const userId =
    decodedToken[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
    ];
  const email =
    decodedToken[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
    ];
  const role =
    decodedToken[
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    ];

  return {
    id: userId,
    email,
    role,
    firstName: "Имя", // можно захардкодить или попытаться извлечь позже
    lastName: "Фамилия",
  };
};

// Экспортируем функцию получения токена для использования в других файлах
export const getAuthToken = getToken;

export const useAuth = () => {
  return {
    isAuthenticated: computed(() => isAuthenticated.value),
    checkAuth,
    login,
    logout,
    getCurrentUser,
    getToken,
  };
};
