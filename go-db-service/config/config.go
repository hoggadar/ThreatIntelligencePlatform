package config

import (
	"awesomeProject/broker"
	"fmt"
	"net/url"
	"os"
	"strconv"
	"strings"
)

// Config holds the application configuration
type Config struct {
	LoggerConfig LoggerConfig
	DBConfig     DBConfig
	ServerConfig ServerConfig
	BrokerConfig broker.BrokerConfig
}

type ServerConfig struct {
	Port string
}

type DBConfig struct {
	DBHost     string
	DBPort     string
	DBUser     string
	DBPassword string
	DBName     string
}

type LoggerConfig struct {
	LogLevel    string
	NodeIP      string
	PodIP       string
	ServiceName string
}

// LoadConfig loads configuration from environment variables
func LoadConfig() (Config, error) {
	bSize, _ := strconv.Atoi(getEnv("BATCH_SIZE", "100"))
	config := Config{
		ServerConfig: ServerConfig{
			Port: getEnv("SERVER_PORT", ":8080"),
		},
		DBConfig: DBConfig{
			DBHost:     getEnv("DB_HOST", "localhost"),
			DBPort:     getEnv("DB_PORT", "5432"),
			DBUser:     getEnv("DB_USER", "user"),
			DBPassword: getEnv("DB_PASSWORD", "password"),
			DBName:     getEnv("DB_NAME", "app_db"),
		},
		LoggerConfig: LoggerConfig{
			LogLevel:    getEnv("LOG_LEVEL", "debug"),
			NodeIP:      getEnv("NODE_IP", "127.0.0.1"),
			PodIP:       getEnv("POD_IP", "127.0.0.1"),
			ServiceName: getEnv("SERVICE_NAME", "my_service"),
		},
		BrokerConfig: broker.BrokerConfig{

			BatchSize:  bSize,
			Topic:      getEnv("TOPIC", "ioc.normalized.queue"),
			BrokerAddr: getEnv("BROKER_ADDR", "localhost:9092"),
		},
	}
	return config, nil
}

// String returns a string representation of the Config struct in the format: field: value
func (cfg Config) String() string {
	var sb strings.Builder

	// ServerConfig
	sb.WriteString(fmt.Sprintf("ServerConfig:\n"))
	sb.WriteString(fmt.Sprintf("  Port: %s\n", cfg.ServerConfig.Port))

	// DBConfig
	sb.WriteString(fmt.Sprintf("DBConfig:\n"))
	sb.WriteString(fmt.Sprintf("  DBHost: %s\n", cfg.DBConfig.DBHost))
	sb.WriteString(fmt.Sprintf("  DBPort: %s\n", cfg.DBConfig.DBPort))
	sb.WriteString(fmt.Sprintf("  DBUser: %s\n", cfg.DBConfig.DBUser))
	sb.WriteString(fmt.Sprintf("  DBPassword: %s\n", cfg.DBConfig.DBPassword))
	sb.WriteString(fmt.Sprintf("  DBName: %s\n", cfg.DBConfig.DBName))

	// LoggerConfig
	sb.WriteString(fmt.Sprintf("LoggerConfig:\n"))
	sb.WriteString(fmt.Sprintf("  LogLevel: %s\n", cfg.LoggerConfig.LogLevel))
	sb.WriteString(fmt.Sprintf("  NodeIP: %s\n", cfg.LoggerConfig.NodeIP))
	sb.WriteString(fmt.Sprintf("  PodIP: %s\n", cfg.LoggerConfig.PodIP))
	sb.WriteString(fmt.Sprintf("  ServiceName: %s\n", cfg.LoggerConfig.ServiceName))

	// BrokerConfig
	sb.WriteString(fmt.Sprintf("BrokerConfig:\n"))
	sb.WriteString(fmt.Sprintf("  BatchSize: %d\n", cfg.BrokerConfig.BatchSize))
	sb.WriteString(fmt.Sprintf("  Topic: %s\n", cfg.BrokerConfig.Topic))
	sb.WriteString(fmt.Sprintf("  BrokerAddr: %s\n", cfg.BrokerConfig.BrokerAddr))

	return sb.String()
}

func (cfg *DBConfig) ConnStr() string {
	return fmt.Sprintf("tcp://%s:%s?username=%s&password=%s&database=%s",
		cfg.DBHost, cfg.DBPort,
		url.QueryEscape(cfg.DBUser),
		url.QueryEscape(cfg.DBPassword),
		cfg.DBName)

}

// getEnv gets an environment variable or returns a default value
func getEnv(key, defaultValue string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return defaultValue
}
