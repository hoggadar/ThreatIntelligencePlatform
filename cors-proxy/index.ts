import express, { Request, Response } from 'express';
import axios from 'axios';
import cors from 'cors';

const app = express();
const port = 3000;

// CORS middleware для разрешения запросов с любого источника
app.use(cors({
  origin: '*',  // Разрешаем запросы с любого источника
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization'], // Разрешаем заголовки Content-Type и Authorization
}));

// Body parser для обработки JSON
app.use(express.json());

// Прокси запрос для логина
app.post('/api/auth/login', async (req: Request, res: Response) => {
  console.log('Request headers:', req.headers); // Логируем заголовки запроса
  try {
    const response = await axios.post('http://localhost:8888/api/auth/login', req.body, {
      headers: req.headers, // Перенаправляем все заголовки от клиента
    });
    res.json(response.data);
  } catch (error: any) {
    console.error('Ошибка при запросе на сервер:', error);
    res.status(500).json({ message: error?.response?.data?.message ?? 'Ошибка при авторизации' });
  }
});

// Прокси запрос для регистрации
app.post('/api/auth/signup', async (req: Request, res: Response) => {
  try {
    const response = await axios.post('http://localhost:8888/api/auth/signup', req.body, {
      headers: req.headers, // Передаем заголовки от клиента
    });
    res.status(response.status).json(response.data);
  } catch (error: any) {
    console.error('Ошибка при регистрации:', error.message);
    res.status(error?.response?.status ?? 500).json({
      message: error?.response?.data?.message ?? 'Ошибка при регистрации',
    });
  }
});

// Запуск сервера
app.listen(port, async () => {
  console.log(`🚀 Proxy server is running at http://localhost:${port}`);
});
