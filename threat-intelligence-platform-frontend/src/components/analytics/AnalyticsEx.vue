<script setup lang="ts">
</script>

<template>
  <div class="space-y-6 px-4 py-6">

    <div class="analytics-grid">
      <div class="analytics-card">
        <h3 class="text-lg font-semibold">Распределение угроз по типам</h3>
        <div class="chart-placeholder">
          График распределения угроз
        </div>
      </div>
      <div class="analytics-card">
        <h3 class="text-lg font-semibold">Тренды угроз</h3>
        <div class="chart-placeholder">
          График трендов
        </div>
      </div>
      <div class="analytics-card wide">
        <h3 class="text-lg font-semibold">Временная шкала инцидентов</h3>
        <div class="chart-placeholder">
          График временной шкалы
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.analytics-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.analytics-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.analytics-card.wide {
  grid-column: 1 / -1;
}

.chart-placeholder {
  height: 300px;
  background-color: #f5f5f5;
  border-radius: 4px;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 1rem;
  color: #666;
}
</style>
