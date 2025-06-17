import axios from "../api/axios";

export interface IoCDTO {
  id: string;
  source: string;
  firstSeen: string;
  lastSeen: string;
  type: string;
  value: string;
  tags: string[];
  additionalData: Record<string, any>;
}

export interface IoCListResponse {
  items: IoCDTO[];
  totalCount: number;
  pageSize: number;
  pageIndex: number;
  totalPages: number;
}

export interface IoCFilter {
  type?: string;
  source?: string;
  search?: string;
  limit?: number;
  offset?: number;
}

// Получить все IoC с пагинацией
export const getAllIoCs = async (
  limit = 10,
  offset = 0,
  search = "",
  type = "",
  source = ""
): Promise<IoCListResponse> => {
  try {
    let url = `/IoC/GetAll?limit=${limit}&offset=${offset}`;

    if (search) {
      url += `&search=${encodeURIComponent(search)}`;
    }

    if (type) {
      url += `&type=${encodeURIComponent(type)}`;
    }

    if (source) {
      url += `&source=${encodeURIComponent(source)}`;
    }

    const response = await axios.get(url);
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

// Получить общее количество IoC
export const getIoCCount = async (): Promise<number> => {
  try {
    const response = await axios.get("/IoC/Count");
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения количества индикаторов"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить общее количество IoC с учетом фильтров
export const getFilteredIoCCount = async (
  search = "",
  type = "",
  source = ""
): Promise<number> => {
  try {
    let url = "/IoC/Count";
    const params = [];

    if (search) {
      params.push(`search=${encodeURIComponent(search)}`);
    }

    if (type) {
      params.push(`type=${encodeURIComponent(type)}`);
    }

    if (source) {
      params.push(`source=${encodeURIComponent(source)}`);
    }

    if (params.length > 0) {
      url += "?" + params.join("&");
    }

    const response = await axios.get(url);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения количества индикаторов"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить количество по типам индикаторов
export const getIoCCountByType = async (): Promise<Record<string, number>> => {
  try {
    const response = await axios.get("/IoC/CountByType");
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения статистики по типам"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить количество по источникам
export const getIoCCountBySource = async (): Promise<
  Record<string, number>
> => {
  try {
    const response = await axios.get("/IoC/CountBySource");
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message || "Ошибка получения статистики по источникам"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить количество по типу и источнику
export const getIoCCountByTypeAndSource = async (
  type: string
): Promise<Record<string, number>> => {
  try {
    const response = await axios.get(`/IoC/CountByTypeAndSource?type=${type}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message ||
          `Ошибка получения статистики по типу ${type}`
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить количество по источнику и типу
export const getIoCCountBySourceAndType = async (
  source: string
): Promise<Record<string, number>> => {
  try {
    const response = await axios.get(
      `/IoC/CountBySourceAndType?source=${source}`
    );
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message ||
          `Ошибка получения статистики по источнику ${source}`
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить количество для конкретного типа
export const getIoCCountSpecificType = async (
  type: string
): Promise<number> => {
  try {
    const response = await axios.get(`/IoC/CountSpecificType?type=${type}`);
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message ||
          `Ошибка получения количества для типа ${type}`
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить количество для конкретного источника
export const getIoCCountSpecificSource = async (
  source: string
): Promise<number> => {
  try {
    const response = await axios.get(
      `/IoC/CountSpecificSource?source=${source}`
    );
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message ||
          `Ошибка получения количества для источника ${source}`
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};

// Получить типы по источникам
export const getIoCCountTypesBySource = async (): Promise<
  Record<string, Record<string, number>>
> => {
  try {
    const response = await axios.get("/IoC/CountTypesBySource");
    return response.data;
  } catch (err: any) {
    if (err.response) {
      throw new Error(
        err.response.data.message ||
          "Ошибка получения статистики типов по источникам"
      );
    } else {
      throw new Error("Ошибка сети или сервера");
    }
  }
};
