package storage

import (
	"awesomeProject/models"
	"awesomeProject/pkg/logger"
	"context"
	"database/sql"
	"encoding/json"
	"fmt"
	"strings"
	"time"

	_ "github.com/ClickHouse/clickhouse-go"
	"go.uber.org/zap"
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
		) ENGINE = ReplacingMergeTree(last_seen)
	PARTITION BY toYYYYMM(first_seen)
	ORDER BY (value);
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
		// Convert tags to lowercase
		lowerTags := make([]string, len(ioc.Tags))
		for i, tag := range ioc.Tags {
			lowerTags[i] = strings.ToLower(tag)
		}
		tags := strings.Join(lowerTags, ",")

		additionalData, err := json.Marshal(ioc.AdditionalData)
		if err != nil {
			s.logger.Error(fmt.Sprintf("failed to marshal additional_data %v", err))
			return err
		}

		_, err = stmt.ExecContext(ctx,
			ioc.ID,
			strings.ToLower(ioc.Source),
			ioc.FirstSeen,
			ioc.LastSeen,
			strings.ToLower(ioc.Type),
			strings.ToLower(ioc.Value),
			tags,
			string(additionalData),
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
	var queryBuilder strings.Builder
	var args []interface{}

	// Строим базовый запрос
	queryBuilder.WriteString(baseQuery)

	if request.Filter != "" {
		queryBuilder.WriteString(` WHERE (toString(id) LIKE ? OR source LIKE ? OR type LIKE ? OR value LIKE ? OR tags LIKE ?)`)
		filter := "%" + request.Filter + "%"
		args = append(args, filter, filter, filter, filter, filter)
	}

	// Добавляем пагинацию
	queryBuilder.WriteString(` LIMIT ? OFFSET ?`)
	args = append(args, request.Limit, request.Offset)

	// Логируем запрос с реальными значениями
	queryWithValues := queryBuilder.String()
	for _, arg := range args {
		// Преобразуем каждый аргумент в строку и подставляем вместо плейсхолдера
		// Здесь мы также добавляем кавычки для строк, чтобы они правильно выглядели в SQL-запросе
		queryWithValues = strings.Replace(queryWithValues, "?", fmt.Sprintf("'%v'", arg), 1)
	}

	// Логируем итоговый запрос с подставленными значениями
	s.logger.Debug(fmt.Sprintf("Executing query: %s", queryWithValues))

	rows, err := s.db.QueryContext(ctx, queryBuilder.String(), args...)
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

			// Convert tags to lowercase
			lowerTags := make([]string, len(ioc.Tags))
			for i, tag := range ioc.Tags {
				lowerTags[i] = strings.ToLower(tag)
			}
			tagsJSON, _ := json.Marshal(lowerTags)
			additionalDataJSON, _ := json.Marshal(ioc.AdditionalData)

			_, err := stmt.ExecContext(ctx,
				ioc.ID,
				strings.ToLower(ioc.Source),
				ioc.FirstSeen,
				ioc.LastSeen,
				strings.ToLower(ioc.Type),
				strings.ToLower(ioc.Value),
				string(tagsJSON),
				string(additionalDataJSON),
			)
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

	s.logger.Debug(fmt.Sprintf(query, "\n", args))

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

func (s *ClickHouseStorage) AllIocsCount(ctx context.Context) (int64, error) {
	query := `SELECT count(*) FROM ioc_data`

	var count int64
	err := s.db.QueryRowContext(ctx, query).Scan(&count)
	if err != nil {
		s.logger.Error("Failed to count all IoCs", zap.Error(err))
		return 0, fmt.Errorf("failed to count all IoCs: %v", err)
	}

	return count, nil
}

func (s *ClickHouseStorage) CountByType(ctx context.Context) (map[string]int64, error) {
	query := `SELECT type, count(*) FROM ioc_data GROUP BY type`

	rows, err := s.db.QueryContext(ctx, query)
	if err != nil {
		s.logger.Error("Failed to count IoCs by type", zap.Error(err))
		return nil, fmt.Errorf("failed to count IoCs by type: %v", err)
	}
	defer rows.Close()

	result := make(map[string]int64)
	for rows.Next() {
		var typeName string
		var count int64
		if err := rows.Scan(&typeName, &count); err != nil {
			s.logger.Error("Failed to scan row in CountByType", zap.Error(err))
			return nil, fmt.Errorf("failed to scan row in CountByType: %v", err)
		}
		result[typeName] = count
	}

	return result, nil
}

func (s *ClickHouseStorage) CountSpecificType(ctx context.Context, typeName string) (int64, error) {
	query := `SELECT count(*) FROM ioc_data WHERE type = ?`

	var count int64
	err := s.db.QueryRowContext(ctx, query, typeName).Scan(&count)
	if err != nil {
		s.logger.Error("Failed to count IoCs of specific type", zap.Error(err))
		return 0, fmt.Errorf("failed to count IoCs of specific type: %v", err)
	}

	return count, nil
}

func (s *ClickHouseStorage) CountBySource(ctx context.Context) (map[string]int64, error) {
	query := `SELECT source, count(*) FROM ioc_data GROUP BY source`

	rows, err := s.db.QueryContext(ctx, query)
	if err != nil {
		s.logger.Error("Failed to count IoCs by source", zap.Error(err))
		return nil, fmt.Errorf("failed to count IoCs by source: %v", err)
	}
	defer rows.Close()

	result := make(map[string]int64)
	for rows.Next() {
		var source string
		var count int64
		if err := rows.Scan(&source, &count); err != nil {
			s.logger.Error("Failed to scan row in CountBySource", zap.Error(err))
			return nil, fmt.Errorf("failed to scan row in CountBySource: %v", err)
		}
		result[source] = count
	}

	return result, nil
}

func (s *ClickHouseStorage) CountTypesBySource(ctx context.Context) (map[string]map[string]int64, error) {
	query := `
		SELECT source, type, count() as count
		FROM ioc_data
		GROUP BY source, type
	`

	rows, err := s.db.QueryContext(ctx, query)
	if err != nil {
		s.logger.Error("Error executing CountTypesBySource query", zap.Error(err))
		return nil, err
	}
	defer rows.Close()

	result := make(map[string]map[string]int64)
	for rows.Next() {
		var source, typeName string
		var count int64
		if err := rows.Scan(&source, &typeName, &count); err != nil {
			s.logger.Error("Error scanning CountTypesBySource row", zap.Error(err))
			return nil, err
		}
		if _, exists := result[source]; !exists {
			result[source] = make(map[string]int64)
		}
		result[source][typeName] = count
	}

	return result, nil
}

func (s *ClickHouseStorage) CountBySourceAndType(ctx context.Context, sourceName string) (map[string]int64, error) {
	query := `
		SELECT type, count() as count
		FROM ioc_data
		WHERE source = ?
		GROUP BY type
	`

	rows, err := s.db.QueryContext(ctx, query, sourceName)
	if err != nil {
		s.logger.Error("Error executing CountBySourceAndType query", zap.Error(err))
		return nil, err
	}
	defer rows.Close()

	result := make(map[string]int64)
	for rows.Next() {
		var typeName string
		var count int64
		if err := rows.Scan(&typeName, &count); err != nil {
			s.logger.Error("Error scanning CountBySourceAndType row", zap.Error(err))
			return nil, err
		}
		result[typeName] = count
	}

	return result, nil
}

func (s *ClickHouseStorage) CountByTypeAndSource(ctx context.Context, typeName string) (map[string]int64, error) {
	query := `
		SELECT source, count() as count
		FROM ioc_data
		WHERE type = ?
		GROUP by source
	`

	rows, err := s.db.QueryContext(ctx, query, typeName)
	if err != nil {
		s.logger.Error("Error executing CountByTypeAndSource query", zap.Error(err))
		return nil, err
	}
	defer rows.Close()

	result := make(map[string]int64)
	for rows.Next() {
		var source string
		var count int64
		if err := rows.Scan(&source, &count); err != nil {
			s.logger.Error("Error scanning CountByTypeAndSource row", zap.Error(err))
			return nil, err
		}
		result[source] = count
	}

	return result, nil
}

func (s *ClickHouseStorage) CountSpecificSource(ctx context.Context, sourceName string) (int64, error) {
	query := `SELECT count(*) FROM ioc_data WHERE source = ?`

	var count int64
	err := s.db.QueryRowContext(ctx, query, sourceName).Scan(&count)
	if err != nil {
		s.logger.Error("Failed to count IoCs of specific source", zap.Error(err))
		return 0, fmt.Errorf("failed to count IoCs of specific source: %v", err)
	}

	return count, nil
}
