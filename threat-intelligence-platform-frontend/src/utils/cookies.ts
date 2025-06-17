// Имя для резервного хранилища, если куки не работают
const STORAGE_PREFIX = "tip_";

// Функция установки cookie с резервным хранением
export const setCookie = (
  name: string,
  value: string,
  days = 7,
  path = "/"
): void => {
  try {
    if (typeof document !== 'undefined') {
      const expires = new Date();
      expires.setTime(expires.getTime() + days * 24 * 60 * 60 * 1000);

      // Устанавливаем куку с датой истечения
      document.cookie = `${name}=${encodeURIComponent(
        value
      )}; expires=${expires.toUTCString()}; path=${path}`;

      // Проверяем, установилась ли кука
      if (!hasCookie(name)) {
        // Резервное хранение в sessionStorage
        sessionStorage.setItem(`${STORAGE_PREFIX}${name}`, value);

        // Для обратной совместимости также сохраняем в localStorage
        try {
          localStorage.setItem(name, value);
        } catch (e) {
          // Игнорируем ошибки localStorage
        }
      }
    } else {
      // Если document не определен, используем sessionStorage как основное хранилище
      sessionStorage.setItem(`${STORAGE_PREFIX}${name}`, value);
      try {
        localStorage.setItem(name, value);
      } catch (e) {
        // Игнорируем ошибки localStorage
      }
    }
  } catch (error) {
    // В случае ошибки (даже если document определен, но есть другие проблемы)
    sessionStorage.setItem(`${STORAGE_PREFIX}${name}`, value);

    // Для обратной совместимости также сохраняем в localStorage
    try {
      localStorage.setItem(name, value);
    } catch (e) {
      // Игнорируем ошибки localStorage
    }
  }
};

// Функция получения cookie по имени с поддержкой резервного хранилища
export const getCookie = (name: string): string | null => {
  // Сначала пытаемся получить значение из куки
  if (typeof document !== 'undefined') {
    const nameWithEq = `${name}=`;
    const cookies = document.cookie.split(";");

    for (let i = 0; i < cookies.length; i++) {
      let cookie = cookies[i].trim();
      if (cookie.indexOf(nameWithEq) === 0) {
        return decodeURIComponent(cookie.substring(nameWithEq.length));
      }
    }
  }

  // Если в куки не нашли или document не определен, проверяем sessionStorage
  const storageValue = sessionStorage.getItem(`${STORAGE_PREFIX}${name}`);
  if (storageValue) {
    return storageValue;
  }

  // Для обратной совместимости проверяем localStorage
  try {
    const localValue = localStorage.getItem(name);
    if (localValue) {
      return localValue;
    }
  } catch (e) {
    // Игнорируем ошибки localStorage
  }

  return null;
};

// Функция удаления cookie
export const deleteCookie = (name: string, path = "/"): void => {
  try {
    if (typeof document !== 'undefined') {
      document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=${path};`;
    }
  } catch (error) {
    // Игнорируем ошибки
  }

  // Удаляем также из sessionStorage
  try {
    sessionStorage.removeItem(`${STORAGE_PREFIX}${name}`);
  } catch (e) {
    // Игнорируем ошибки
  }

  // Удаляем из localStorage
  try {
    localStorage.removeItem(name);
  } catch (e) {
    // Игнорируем ошибки
  }
};

// Проверка наличия cookie или значения в sessionStorage
export const hasCookie = (name: string): boolean => {
  // Сначала проверяем куки
  if (typeof document !== 'undefined') {
    const nameWithEq = `${name}=`;
    const cookies = document.cookie.split(";");

    for (let i = 0; i < cookies.length; i++) {
      let cookie = cookies[i].trim();
      if (cookie.indexOf(nameWithEq) === 0) {
        return true;
      }
    }
  }

  // Затем проверяем sessionStorage
  return sessionStorage.getItem(`${STORAGE_PREFIX}${name}`) !== null;
};

// Тестовая функция для проверки работоспособности куки
export const testCookieSupport = (): boolean => {
  if (typeof document === 'undefined') {
    return false;
  }
  try {
    const testName = "cookie_test";
    const testValue = "test_value";

    // Пытаемся установить тестовую куку
    document.cookie = `${testName}=${testValue}; path=/;`;

    // Проверяем, установилась ли она
    const success = document.cookie.indexOf(`${testName}=${testValue}`) !== -1;

    // Удаляем тестовую куку
    document.cookie = `${testName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;

    return success;
  } catch (error) {
    return false;
  }
};
