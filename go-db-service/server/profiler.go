package server

import (
	"net/http"
	_ "net/http/pprof" // Подключение pprof
)

// StartProfiler запускает сервер профайлера на отдельном маршруте.
func StartProfiler() {
	go func() {
		if err := http.ListenAndServe(":6060", nil); err != nil {
			panic("Failed to start pprof server: " + err.Error())
		}
	}()
}
