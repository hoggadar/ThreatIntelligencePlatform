import axios from "../api/axios";

export interface UserDTO {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

export interface UserListResponse {
  items: UserDTO[];
  pageIndex: number;
  totalPages: number;
  totalCount: number;
  pageSize: number;
}

export interface CreateUserDTO {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface UpdateUserDTO {
  firstName: string;
  lastName: string;
  email: string;
}

export interface RoleManagementDTO {
  userId: string;
  role: string;
}

export interface RoleDTO {
  id: string;
  name: string;
  description: string;
}

export interface RoleListResponse {
  items: RoleDTO[];
  pageIndex: number;
  totalPages: number;
  totalCount: number;
  pageSize: number;
}

// Получить всех пользователей с пагинацией
export const getAllUsers = async (
  pageIndex = 1,
  pageSize = 10
): Promise<UserListResponse> => {
  try {
    const response = await axios.get(
      `/User/GetAll?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
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
export const getUserById = async (userId: string): Promise<UserDTO> => {
  try {
    const response = await axios.get(`/User/GetById/${userId}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Пользователь не найден");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить пользователя по email
export const getUserByEmail = async (email: string): Promise<UserDTO> => {
  try {
    const response = await axios.get(`/User/GetByEmail/${email}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Пользователь не найден");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Создать пользователя
export const createUser = async (userData: CreateUserDTO): Promise<UserDTO> => {
  try {
    const response = await axios.post("/User/Create", userData);
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
  userData: UpdateUserDTO
): Promise<UserDTO> => {
  try {
    const response = await axios.put(`/User/Update/${userId}`, userData);
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

// Удалить пользователя
export const deleteUser = async (userId: string): Promise<void> => {
  try {
    await axios.delete(`/User/Delete/${userId}`);
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

// Получить роли пользователя
export const getUserRoles = async (userId: string): Promise<string[]> => {
  try {
    const response = await axios.get(`/User/GetUserRoles/${userId}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Ошибка получения ролей");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Добавить пользователя в роль
export const addToRole = async (
  userId: string,
  role: string
): Promise<void> => {
  try {
    await axios.post("/User/AddToRole", { userId, role });
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Ошибка добавления роли");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Удалить пользователя из роли
export const removeFromRole = async (
  userId: string,
  role: string
): Promise<void> => {
  try {
    await axios.post("/User/RemoveFromRole", { userId, role });
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Ошибка удаления роли");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить все роли с пагинацией
export const getAllRoles = async (
  pageIndex = 1,
  pageSize = 10
): Promise<RoleListResponse> => {
  try {
    const response = await axios.get(
      `/Role/GetAll?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Ошибка получения ролей");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить роль по имени
export const getRoleByName = async (name: string): Promise<RoleDTO> => {
  try {
    const response = await axios.get(`/Role/GetByName/${name}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Роль не найдена");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить роли пользователя в формате RoleDTO
export const getUserRolesDetails = async (
  userId: string
): Promise<RoleDTO[]> => {
  try {
    const response = await axios.get(`/Role/GetUserRoles/${userId}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения ролей пользователя"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};
