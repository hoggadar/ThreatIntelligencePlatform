# Используем официальный образ Go
FROM golang:latest AS builder

# Устанавливаем рабочую директорию внутри контейнера
WORKDIR /app

# Копируем Go модули
COPY go.mod go.sum ./

# Обновляем зависимости
RUN go mod tidy

# Копируем весь исходный код в контейнер
COPY . .

# Устанавливаем рабочую директорию для компиляции (где находится main.go)
WORKDIR /app/cmd

# Компилируем приложение
RUN go build -o main .

# Указываем команду для запуска приложения
CMD ["./main"]