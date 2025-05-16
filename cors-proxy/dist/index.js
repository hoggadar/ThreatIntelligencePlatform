"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const axios_1 = __importDefault(require("axios"));
const cors_1 = __importDefault(require("cors"));
const app = (0, express_1.default)();
const port = 3000;
// CORS middleware для разрешения запросов с любого источника
app.use((0, cors_1.default)({
    origin: '*', // Разрешаем запросы с любого источника
    methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
    allowedHeaders: ['Content-Type', 'Authorization'], // Разрешаем заголовки Content-Type и Authorization
}));
// Body parser для обработки JSON
app.use(express_1.default.json());
// Прокси запрос для логина
app.post('/api/auth/login', (req, res) => __awaiter(void 0, void 0, void 0, function* () {
    var _a, _b, _c;
    console.log('Request headers:', req.headers); // Логируем заголовки запроса
    try {
        const response = yield axios_1.default.post('http://localhost:8888/api/auth/login', req.body, {
            headers: req.headers, // Перенаправляем все заголовки от клиента
        });
        res.json(response.data);
    }
    catch (error) {
        console.error('Ошибка при запросе на сервер:', error);
        res.status(500).json({ message: (_c = (_b = (_a = error === null || error === void 0 ? void 0 : error.response) === null || _a === void 0 ? void 0 : _a.data) === null || _b === void 0 ? void 0 : _b.message) !== null && _c !== void 0 ? _c : 'Ошибка при авторизации' });
    }
}));
// Прокси запрос для регистрации
app.post('/api/auth/signup', (req, res) => __awaiter(void 0, void 0, void 0, function* () {
    var _a, _b, _c, _d, _e;
    try {
        const response = yield axios_1.default.post('http://localhost:8888/api/auth/signup', req.body, {
            headers: req.headers, // Передаем заголовки от клиента
        });
        res.status(response.status).json(response.data);
    }
    catch (error) {
        console.error('Ошибка при регистрации:', error.message);
        res.status((_b = (_a = error === null || error === void 0 ? void 0 : error.response) === null || _a === void 0 ? void 0 : _a.status) !== null && _b !== void 0 ? _b : 500).json({
            message: (_e = (_d = (_c = error === null || error === void 0 ? void 0 : error.response) === null || _c === void 0 ? void 0 : _c.data) === null || _d === void 0 ? void 0 : _d.message) !== null && _e !== void 0 ? _e : 'Ошибка при регистрации',
        });
    }
}));
// Запуск сервера
app.listen(port, () => __awaiter(void 0, void 0, void 0, function* () {
    console.log(`🚀 Proxy server is running at http://localhost:${port}`);
}));
