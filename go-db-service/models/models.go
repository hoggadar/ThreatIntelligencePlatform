package models

import (
	"time"
)

// IoCDto представляет структуру данных для IOCs (Indicators of Compromise)
type IoCDto struct {
	ID             string            `json:"id"`
	Source         string            `json:"source"`
	FirstSeen      *time.Time        `json:"first_seen"`
	LastSeen       *time.Time        `json:"last_seen"`
	Type           string            `json:"type"`
	Value          string            `json:"value"`
	Tags           []string          `json:"tags"`
	AdditionalData map[string]string `json:"additional_data"`
}

// StoreRequest представляет запрос для записи в базу данных
type StoreRequest struct {
	IoCs []IoCDto `json:"iocs"`
}

// LoadRequest представляет запрос для загрузки из базы данных
type LoadRequest struct {
	Limit  int64  `json:"limit"`
	Offset int64  `json:"offset"`
	Filter string `json:"filter"`
}

// LoadResponse представляет ответ при загрузке данных из базы
type LoadResponse struct {
	IoCs []IoCDto `json:"iocs"`
}

// StreamStoreRequest представляет запрос для стримовой записи
type StreamStoreRequest struct {
	Ioc IoCDto `json:"ioc"`
}

// StreamLoadResponse представляет ответ для стримовой загрузки
type StreamLoadResponse struct {
	Ioc IoCDto `json:"ioc"`
}
