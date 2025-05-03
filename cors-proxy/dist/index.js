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
// CORS middleware
app.use((0, cors_1.default)({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
    allowedHeaders: ['Content-Type', 'Authorization'],
}));
// Body parser
app.use(express_1.default.json());
// Preflight
app.options('/', (_req, res) => {
    res.sendStatus(200);
});
app.post('/api/auth/login', (req, res) => __awaiter(void 0, void 0, void 0, function* () {
    console.log('Request headers:', req.headers); // Логируем заголовки
    try {
        const response = yield axios_1.default.post('http://localhost:8888/api/auth/login', req.body, {
            headers: req.headers, // Передаем все заголовки, полученные от клиента
        });
        res.json(response.data);
    }
    catch (error) {
        console.error('Ошибка при запросе на сервер:', error);
        res.status(500).json({ message: 'Ошибка при авторизации' });
    }
}));
app.post('/api/auth/signup', (req, res) => __awaiter(void 0, void 0, void 0, function* () {
    var _a, _b, _c;
    try {
        const response = yield axios_1.default.post('http://localhost:8888/api/auth/signup', req.body, {
            headers: req.headers,
        });
        res.status(response.status).json(response.data);
    }
    catch (error) {
        console.error('Ошибка при регистрации:', error.message);
        res.status(((_a = error === null || error === void 0 ? void 0 : error.response) === null || _a === void 0 ? void 0 : _a.status) || 500).json({
            message: ((_c = (_b = error === null || error === void 0 ? void 0 : error.response) === null || _b === void 0 ? void 0 : _b.data) === null || _c === void 0 ? void 0 : _c.message) || 'Ошибка при регистрации',
        });
    }
}));
app.listen(port, () => {
    console.log(`🚀 Proxy server is running at http://localhost:${port}`);
});
