import axios from "../api/axios";
import { ref, computed } from "vue";
import { jwtDecode } from 'jwt-decode';

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

const checkAuth = () => {
  isAuthenticated.value = !!localStorage.getItem("token");
  return isAuthenticated.value;
};

export const login = async ({ email, password }: LoginPayload) => {
  console.log("Sending login request with:", { email, password });
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
      throw new Error(err.response.data.message || "Ошибка входа");
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
      throw new Error(err.response.data.message || "Ошибка регистрации");
    } else {
      console.error("Registration failed:", err);
      throw new Error("Ошибка сети или сервера");
    }
  }
};

export const logout = () => {
  localStorage.removeItem("token");
  isAuthenticated.value = false;
};

export const getCurrentUser = async () => {
  const token = localStorage.getItem("token");
  if (!token) throw new Error("Нет токена");

  const decodedToken = jwtDecode<DecodedJwtPayload>(token);

  const userId = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
  const email = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
  const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  // Это пока условно — если сервер не даёт имя и фамилию, то можно оставить только email
  return {
    id: userId,
    email,
    role,
    firstName: "Имя", // можно захардкодить или попытаться извлечь позже
    lastName: "Фамилия"
  };
};

export const useAuth = () => {
  return {
    isAuthenticated: computed(() => isAuthenticated.value),
    checkAuth,
    login,
    logout,
    getCurrentUser,
  };
};
