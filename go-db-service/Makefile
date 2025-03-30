.PHONY: lint run test postman-test all

# Путь к основной точке входа в приложение
MAIN=cmd/main.go

# ID коллекции Postman
POSTMAN_COLLECTION_ID=42733086-6fcbd909-5bf0-4e70-a884-7b5f144213c6

# API ключ Postman (лучше через переменную окружения)
POSTMAN_API_KEY=PMAK-67c3e9200a2c2f00012ab62e-a4a612df66601c6bb19b6f7f22895f1795

# Путь к Postman CLI
POSTMAN_CLI=./bin/postman

# Цель по умолчанию
all: lint run postman-test

# Линтер проекта
lint:
	@echo "Запуск golangci-lint..."
	@./bin/golangci-lint run ./... || (echo "Линтер нашел ошибки!" && exit 1)
	@echo "Линтер не нашел ошибок."

# Запуск проекта
run:
	@echo "Запуск проекта..."
	@go run $(MAIN)

postman-test:
	@echo "Авторизация в Postman..."
	@$(POSTMAN_CLI) login --with-api-key $(POSTMAN_API_KEY)
	@echo "Запуск тестов Postman..."
	@$(POSTMAN_CLI) collection run $(POSTMAN_COLLECTION_ID) --env-var base_url=http://localhost:8080 -v --reporters cli > postman-test-output.log
	@cat postman-test-output.log

db-run:
	docker run -d \
      --name clickhouse-server \
      -p 8123:8123 \
      -v /path/to/local/data:/var/lib/clickhouse \
      clickhouse/clickhouse-server:latest

proto-gen:
	protoc --go_out=. --go-grpc_out=. api/proto/database-v2.proto
