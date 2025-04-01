<template>

</template>

<style>
.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors;
}

.btn-primary {
  @apply bg-primary-600 text-white hover:bg-primary-700;
}

.input {
  @apply w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring-2 focus:ring-primary-500 focus:border-transparent;
}

.input-field {
  @apply w-full rounded border px-3 py-2 text-sm text-gray-700 bg-white focus:border-blue-500 focus:ring-blue-500 outline-none transition;
}

/* Стили для автозаполнения в Chrome */
input:-webkit-autofill {
  box-shadow: 0 0 0px 1000px white inset !important;
  -webkit-box-shadow: 0 0 0px 1000px white inset !important;
  -webkit-text-fill-color: #1f2937 !important;
  transition: background-color 5000s ease-in-out 0s;
}

/* Стили для автозаполнения в Firefox */
input:-moz-autofill {
  box-shadow: 0 0 0px 1000px white inset !important;
  -moz-box-shadow: 0 0 0px 1000px white inset !important;
  -moz-text-fill-color: #1f2937 !important;
}
</style> 