import axios from "../api/axios";
import { ref, computed } from "vue";

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

// Состояние авторизации
const isAuthenticated = ref(false);

// Проверка авторизации
const checkAuth = () => {
  isAuthenticated.value = !!localStorage.getItem("token");
  return isAuthenticated.value;
};

// Вход в систему
export const login = async ({ email, password }: LoginPayload) => {
  console.log("Sending login request with:", { email, password }); // Логируем данные перед отправкой
  try {
    const response = await axios.post("/auth/login", { email, password });
    if (response.data.token) {
      localStorage.setItem("token", response.data.token);
      isAuthenticated.value = true;
    }
    return response.data;
  } catch (err: any) {
    if (err.response) {
      console.error("Login failed:", err.response.data);
      throw new Error(err.response.data.message || "Ошибка входа"); // Пробрасываем ошибку дальше
    } else {
      console.error("Login error:", err);
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
      console.error("Registration failed:", err.response.data);
      throw new Error(err.response.data.message || "Ошибка регистрации"); // Пробрасываем ошибку дальше
    } else {
      console.error("Registration failed:", err);
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Выход из системы
export const logout = () => {
  localStorage.removeItem("token");
  isAuthenticated.value = false;
};

// Экспортируем состояние и методы
export const useAuth = () => {
  return {
    isAuthenticated: computed(() => isAuthenticated.value),
    checkAuth,
    login,
    logout,
  };
};
