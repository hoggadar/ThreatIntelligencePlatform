package main

import (
	"testing"
)

func BenchmarkStructs(b *testing.B) {
	for i := 0; i < b.N; i++ {
		structs(100) // Параметр 1000 можно изменить для тестирования разных размеров
	}
}

func BenchmarkInts(b *testing.B) {
	for i := 0; i < b.N; i++ {
		ints(100) // Параметр 1000 можно изменить для тестирования разных размеров
	}
}
