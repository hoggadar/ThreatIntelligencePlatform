import express, { Request, Response } from 'express';
import axios from 'axios';
import cors from 'cors';

const app = express();
const port = 3000;

// CORS middleware
app.use(cors({
  origin: '*',
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization'],
}));

// Body parser
app.use(express.json());

// Preflight
app.options('/', (_req, res) => {
  res.sendStatus(200);
});

app.post('/api/auth/login', async (req: Request, res: Response) => {
  console.log('Request headers:', req.headers); // Логируем заголовки
  try {
    const response = await axios.post('http://localhost:8888/api/auth/login', req.body, {
      headers: req.headers, // Передаем все заголовки, полученные от клиента
    });
    res.json(response.data);
  } catch (error) {
    console.error('Ошибка при запросе на сервер:', error);
    res.status(500).json({ message: 'Ошибка при авторизации' });
  }
});

app.post('/api/auth/signup', async (req: Request, res: Response) => {
  try {
    const response = await axios.post('http://localhost:8888/api/auth/signup', req.body, {
      headers: req.headers,
    });
    res.status(response.status).json(response.data);
  } catch (error: any) {
    console.error('Ошибка при регистрации:', error.message);
    res.status(error?.response?.status || 500).json({
      message: error?.response?.data?.message || 'Ошибка при регистрации',
    });
  }
});

app.listen(port, () => {
  console.log(`🚀 Proxy server is running at http://localhost:${port}`);
});
