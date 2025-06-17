import axios from "../api/axios";

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  roles?: string[];
  createdAt?: string;
  lastLogin?: string;
}

export interface CreateUserPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: string;
}

export interface UpdateUserPayload {
  firstName?: string;
  lastName?: string;
  email?: string;
  role?: string;
}

// Получить всех пользователей (только для админа)
export const getAllUsers = async (): Promise<User[]> => {
  try {
    const response = await axios.get("/users");
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения пользователей"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить пользователя по ID
export const getUserById = async (userId: string): Promise<User> => {
  try {
    const response = await axios.get(`/users/${userId}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Пользователь не найден");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить пользователя по ID с использованием API /api/User/GetById/{id}
export const getUserByIdFromApi = async (userId: string): Promise<User> => {
  try {
    const response = await axios.get(`/User/GetById/${userId}`);
    const userData = response.data;

    // Преобразуем данные из API в формат нашего приложения
    return {
      id: userData.id,
      firstName: userData.firstName || "",
      lastName: userData.lastName || "",
      email: userData.email,
      role:
        userData.roles && userData.roles.length > 0
          ? userData.roles[0]
          : "User",
      roles: userData.roles || [],
    };
  } catch (err: any) {
    if (err.response) {
      // Специальная обработка для 401
      if (err.response.status === 401) {
        throw new Error("Ошибка авторизации: требуется повторный вход");
      }

      throw new Error(err.response.data.message || "Пользователь не найден");
    } else if (err.request) {
      throw new Error("Сервер не отвечает, проверьте соединение");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Создать пользователя (только для админа)
export const createUser = async (
  userData: CreateUserPayload
): Promise<User> => {
  try {
    const response = await axios.post("/users", userData);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка создания пользователя"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Обновить пользователя
export const updateUser = async (
  userId: string,
  userData: UpdateUserPayload
): Promise<User> => {
  try {
    const response = await axios.put(`/users/${userId}`, userData);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка обновления пользователя"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Обновить пользователя через API /api/User/Update/{id}
export const updateUserViaApi = async (
  userId: string,
  userData: {
    firstName: string;
    lastName: string;
    email: string;
  }
): Promise<User> => {
  try {
    const response = await axios.put(`/User/Update/${userId}`, userData);
    const updatedUser = response.data;

    // Приводим данные к формату нашего приложения
    return {
      id: updatedUser.id,
      firstName: updatedUser.firstName || "",
      lastName: updatedUser.lastName || "",
      email: updatedUser.email,
      role:
        updatedUser.roles && updatedUser.roles.length > 0
          ? updatedUser.roles[0]
          : "User",
      roles: updatedUser.roles || [],
    };
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка обновления пользователя"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Удалить пользователя (только для админа)
export const deleteUser = async (userId: string): Promise<void> => {
  try {
    await axios.delete(`/users/${userId}`);
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка удаления пользователя"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить пользователя по Email
export const getUserByEmail = async (email: string): Promise<User> => {
  try {
    const response = await axios.get(`/User/GetByEmail/${email}`);
    const userData = response.data;

    // Преобразуем данные из API в формат нашего приложения
    return {
      id: userData.id,
      firstName: userData.firstName || "",
      lastName: userData.lastName || "",
      email: userData.email,
      role:
        userData.roles && userData.roles.length > 0
          ? userData.roles[0]
          : "User",
      roles: userData.roles || [],
    };
  } catch (err: any) {
    if (err.response) {
      // Специальная обработка для 401
      if (err.response.status === 401) {
        throw new Error("Ошибка авторизации: требуется повторный вход");
      }

      throw new Error(err.response.data.message || "Пользователь не найден");
    } else if (err.request) {
      throw new Error("Сервер не отвечает, проверьте соединение");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};
