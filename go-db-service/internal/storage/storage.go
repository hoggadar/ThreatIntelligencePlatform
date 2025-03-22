package storage

import (
	"awesomeProject/models"
	"awesomeProject/pkg/logger"
	"context"
	"database/sql"
	"encoding/json"
	"fmt"
	_ "github.com/ClickHouse/clickhouse-go"
	"go.uber.org/zap"
	"strings"
	"time"
)

const (
	maxRetries    = 5               // Максимальное количество попыток подключения
	retryInterval = 5 * time.Second // Интервал между попытками в секундах
)

// ClickHouseStorage - реализация хранилища для ClickHouse
type ClickHouseStorage struct {
	db     *sql.DB
	logger *logger.CustomZapLogger
}

// NewClickHouseStorage - создание нового ClickHouseStorage с накатыванием миграций
func NewClickHouseStorage(dsn string, logger *logger.CustomZapLogger) (*ClickHouseStorage, error) {
	var db *sql.DB
	var err error

	// Пытаемся подключиться несколько раз
	for attempts := 1; attempts <= maxRetries; attempts++ {
		db, err = sql.Open("clickhouse", dsn)
		if err != nil {
			logger.Error("Failed to connect to ClickHouse", zap.Int("attempt", attempts), zap.Error(err))
			if attempts < maxRetries {
				logger.Info(fmt.Sprintf("Retrying in %v...", retryInterval))
				time.Sleep(retryInterval)
				continue
			}
			return nil, fmt.Errorf("failed to connect to ClickHouse after retries: %v", err)
		}

		// Проверка доступности базы данных через пинг
		err = db.Ping()
		if err != nil {
			logger.Error("Failed to ping ClickHouse", zap.Int("attempt", attempts), zap.Error(err))
			if attempts < maxRetries {
				logger.Info(fmt.Sprintf("Retrying in %v...", retryInterval))
				time.Sleep(retryInterval)
				continue
			}
			return nil, fmt.Errorf("failed to ping ClickHouse after retries: %v", err)
		}

		// Если подключение и пинг прошли успешно
		logger.Info("Successfully connected to ClickHouse")

		return &ClickHouseStorage{db: db, logger: logger}, nil
	}

	return nil, fmt.Errorf("failed to connect to ClickHouse after maximum retries")
}

func (s *ClickHouseStorage) ApplyHardcodedMigration() error {
	migrationSQL := `
        CREATE TABLE IF NOT EXISTS ioc_data (
            id UUID,
            source String,
            first_seen DateTime,
            last_seen DateTime,
            type String,
            value String,
            tags String,
            additional_data String
        ) ENGINE = MergeTree()
        PARTITION BY toYYYYMM(first_seen)
        ORDER BY (id)
    `

	// Выполнение запроса
	_, err := s.db.Exec(migrationSQL)
	if err != nil {
		s.logger.Error("Failed to execute hardcoded migration", zap.Error(err))
		return fmt.Errorf("failed to execute hardcoded migration: %v", err)
	}

	s.logger.Info("Hardcoded migration applied successfully")
	return nil
}

//// Migrate - метод для накатывания миграций
//func (s *ClickHouseStorage) Migrate() error {
//	files, err := ioutil.ReadDir(migrateDir)
//	if err != nil {
//		s.logger.Error("Failed to read migration directory", zap.Error(err))
//		return fmt.Errorf("failed to read migration directory: %v", err)
//	}
//
//	// Сортируем файлы миграций
//	var migrationFiles []string
//	for _, file := range files {
//		if strings.HasSuffix(file.Name(), ".sql") {
//			migrationFiles = append(migrationFiles, file.Name())
//		}
//	}
//
//	// Сортируем файлы по имени, чтобы они применялись в правильном порядке
//	sort.Strings(migrationFiles)
//
//	// Применяем миграции
//	for _, fileName := range migrationFiles {
//		migrationFilePath := filepath.Join(migrateDir, fileName)
//		s.logger.Info(fmt.Sprintf("Applying migration: %s", fileName))
//
//		// Читаем содержимое миграции
//		migrationSQL, err := ioutil.ReadFile(migrationFilePath)
//		if err != nil {
//			s.logger.Error("Failed to read migration file", zap.String("file", fileName), zap.Error(err))
//			return fmt.Errorf("failed to read migration file %s: %v", fileName, err)
//		}
//
//		// Выполняем миграцию
//		_, err = s.db.Exec(string(migrationSQL))
//		if err != nil {
//			s.logger.Error("Failed to execute migration", zap.String("file", fileName), zap.Error(err))
//			return fmt.Errorf("failed to execute migration %s: %v", fileName, err)
//		}
//
//		s.logger.Info(fmt.Sprintf("Migration %s applied successfully", fileName))
//	}
//
//	return nil
//}

// UnaryStore - метод для сохранения данных в ClickHouse
func (s *ClickHouseStorage) UnaryStore(ctx context.Context, iocs []models.IoCDto) error {
	query := `INSERT INTO ioc_data (
        id, source, first_seen, last_seen, type, value, tags, additional_data
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)`

	tx, err := s.db.BeginTx(ctx, nil)
	if err != nil {
		s.logger.Error(fmt.Sprintf("failed to begin transaction %v", err))
		return err
	}

	stmt, err := tx.PrepareContext(ctx, query)
	if err != nil {
		s.logger.Error(fmt.Sprintf("failed to prepare statement %v", err))
		return err
	}
	defer stmt.Close()

	for _, ioc := range iocs {
		tags := strings.Join(ioc.Tags, ",")
		additionalData, err := json.Marshal(ioc.AdditionalData)
		if err != nil {
			s.logger.Error(fmt.Sprintf("failed to marshal additional_data %v", err))
			return err
		}

		_, err = stmt.ExecContext(ctx,
			ioc.ID, ioc.Source, ioc.FirstSeen, ioc.LastSeen,
			ioc.Type, ioc.Value, tags, string(additionalData),
		)
		if err != nil {
			s.logger.Error(fmt.Sprintf("failed to execute statement %v", err))
			return err
		}
	}

	if err := tx.Commit(); err != nil {
		s.logger.Error(fmt.Sprintf("failed to commit transaction %v", err))
		return err
	}

	return nil
}

// UnaryLoad - метод для загрузки данных из ClickHouse с поддержкой пагинации
func (s *ClickHouseStorage) UnaryLoad(ctx context.Context, request models.LoadRequest) ([]models.IoCDto, error) {
	baseQuery := `SELECT id, source, first_seen, last_seen, type, value, tags, additional_data FROM ioc_data`
	var query string
	var args []interface{}

	if request.Filter != "" {
		query = baseQuery + ` WHERE (toString(id) LIKE ? OR source LIKE ? OR type LIKE ? OR value LIKE ? OR tags LIKE ?) LIMIT ? OFFSET ?`
		filter := "%" + request.Filter + "%"
		args = append(args, filter, filter, filter, filter, filter, request.Limit, request.Offset)
	} else {
		query = baseQuery + ` LIMIT ? OFFSET ?`
		args = append(args, request.Limit, request.Offset)
	}

	rows, err := s.db.QueryContext(ctx, query, args...)
	if err != nil {
		s.logger.Error("Failed to execute query", zap.Error(err))
		return nil, err
	}
	defer rows.Close()

	var result []models.IoCDto
	for rows.Next() {
		var ioc models.IoCDto
		var tagsJSON, additionalDataJSON string

		if err := rows.Scan(&ioc.ID, &ioc.Source, &ioc.FirstSeen, &ioc.LastSeen, &ioc.Type, &ioc.Value, &tagsJSON, &additionalDataJSON); err != nil {
			s.logger.Error("Failed to scan row", zap.Error(err))
			return nil, err
		}

		// Разбираем JSON-данные
		if err := json.Unmarshal([]byte(additionalDataJSON), &ioc.AdditionalData); err != nil {
			s.logger.Warn("Failed to unmarshal additional_data JSON", zap.Error(err))
		}

		// Преобразуем строку tags в слайс
		ioc.Tags = strings.Split(tagsJSON, ",")

		result = append(result, ioc)
	}

	if err := rows.Err(); err != nil {
		s.logger.Error("Rows iteration error", zap.Error(err))
		return nil, err
	}

	return result, nil
}

func (s *ClickHouseStorage) StreamStore(ctx context.Context, stream <-chan models.IoCDto) error {
	query := `
        INSERT INTO ioc_data (id, source, first_seen, last_seen, type, value, tags, additional_data)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)`

	s.logger.Info("Starting StreamStore...")

	// Подготовка транзакции
	tx, err := s.db.BeginTx(ctx, nil)
	if err != nil {
		return fmt.Errorf("failed to begin transaction: %v", err)
	}

	stmt, err := tx.PrepareContext(ctx, query)
	if err != nil {
		tx.Rollback() // Откат транзакции в случае ошибки
		return fmt.Errorf("failed to prepare statement: %v", err)
	}
	defer stmt.Close()

	for {
		select {
		case <-ctx.Done(): // Если контекст завершён
			s.logger.Warn("StreamStore: Context canceled")
			tx.Rollback()
			return ctx.Err()

		case ioc, open := <-stream: // Читаем данные из канала
			if !open {
				// Если поток закрыт, завершить транзакцию и выйти
				err := tx.Commit()
				if err != nil {
					s.logger.Error("Failed to commit transaction", zap.Error(err))
					return err
				}
				s.logger.Info("StreamStore completed successfully")
				return nil
			}

			// Формируем запрос на вставку
			tagsJSON, _ := json.Marshal(ioc.Tags)
			additionalDataJSON, _ := json.Marshal(ioc.AdditionalData)

			_, err := stmt.ExecContext(ctx, ioc.ID, ioc.Source, ioc.FirstSeen, ioc.LastSeen, ioc.Type, ioc.Value, string(tagsJSON), string(additionalDataJSON))
			if err != nil {
				s.logger.Error("Failed to insert IoC into database", zap.Error(err))
				tx.Rollback() // Откат транзакции в случае ошибки
				return err
			}
		}
	}
}

func (s *ClickHouseStorage) StreamLoad(ctx context.Context, request models.LoadRequest) (chan *models.IoCDto, error) {
	baseQuery := `
        SELECT id, source, first_seen, last_seen, type, value, tags, additional_data
        FROM ioc_data`

	var query string
	var args []interface{}

	if request.Filter != "" {
		query = baseQuery + ` WHERE (toString(id)  LIKE ? OR source LIKE ? OR type LIKE ? OR value LIKE ? OR tags LIKE ?) LIMIT ? OFFSET ?`
		filter := "%" + request.Filter + "%"
		args = append(args, filter, filter, filter, filter, filter, request.Limit, request.Offset)
	} else {
		query = baseQuery + ` LIMIT ? OFFSET ?`
		args = append(args, request.Limit, request.Offset)
	}

	rows, err := s.db.QueryContext(ctx, query, args...)
	if err != nil {
		return nil, fmt.Errorf("failed to execute query: %v", err)
	}

	output := make(chan *models.IoCDto, 100) // Буферизированный канал

	go func() {
		defer close(output)
		defer rows.Close()

		for rows.Next() {
			var ioc models.IoCDto
			var tagsJSON, additionalDataJSON string

			if err := rows.Scan(&ioc.ID, &ioc.Source, &ioc.FirstSeen, &ioc.LastSeen, &ioc.Type, &ioc.Value, &tagsJSON, &additionalDataJSON); err != nil {
				s.logger.Error("Failed to scan row", zap.Error(err))
				return
			}

			// Десериализуем JSON-поля
			if err := json.Unmarshal([]byte(tagsJSON), &ioc.Tags); err != nil {
				s.logger.Warn("Failed to unmarshal tags JSON", zap.Error(err))
			}
			if err := json.Unmarshal([]byte(additionalDataJSON), &ioc.AdditionalData); err != nil {
				s.logger.Warn("Failed to unmarshal additional_data JSON", zap.Error(err))
			}

			// Отправляем обработанный объект в канал
			select {
			case output <- &ioc:
			case <-ctx.Done():
				s.logger.Warn("StreamLoad canceled due to context timeout or cancellation")
				return
			}
		}

		if err := rows.Err(); err != nil {
			s.logger.Error("Error iterating over rows", zap.Error(err))
		}
	}()

	return output, nil
}
