import axios from "../api/axios";

export interface IoC {
  id: string;
  type: string;
  value: string;
  confidence: number;
  source: string;
  createdAt: string;
  updatedAt?: string;
  description?: string;
  tags?: string[];
}

export interface IoCFilter {
  type?: string;
  source?: string;
  confidence?: number;
  dateFrom?: string;
  dateTo?: string;
  tags?: string[];
}

// Получить все IoC с фильтрацией
export const getIoCs = async (filter?: IoCFilter): Promise<IoC[]> => {
  try {
    const response = await axios.get("/ioc", { params: filter });
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения индикаторов"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить IoC по ID
export const getIoCById = async (iocId: string): Promise<IoC> => {
  try {
    const response = await axios.get(`/ioc/${iocId}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(err.response.data.message || "Индикатор не найден");
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Удалить IoC
export const deleteIoC = async (iocId: string): Promise<void> => {
  try {
    await axios.delete(`/ioc/${iocId}`);
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка удаления индикатора"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};
