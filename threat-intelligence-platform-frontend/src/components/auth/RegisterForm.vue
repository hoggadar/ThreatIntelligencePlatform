<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { Form, Field } from 'vee-validate'
import * as yup from 'yup'
import { register } from '../../services/authService'

const router = useRouter()
const submitting = ref(false)
const showPassword = ref(false)
const firstName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')
const error = ref<string | null>(null)

const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value
}

const registerSchema = yup.object({
  firstName: yup.string().required('Имя обязательно').min(2, 'Минимум 2 символа'),
  lastName: yup.string().required('Фамилия обязательна').min(2, 'Минимум 2 символа'),
  email: yup.string().required('Email обязателен').email('Невалидный email'),
  password: yup.string().required('Пароль обязателен').min(6, 'Минимум 6 символов'),
})

const handleRegister = async (values: any) => {
  if (submitting.value) return
  submitting.value = true
  error.value = null
  
  console.log('Данные на регистрацию:', {
  firstName: values.firstName,
  lastName: values.lastName,
  email: values.email,
  password: values.password,
})
  try {
    await register({
      firstName: values.firstName,
      lastName: values.lastName,
      email: values.email,
      password: values.password,
    })
    router.push('/login')
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Ошибка регистрации'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="flex justify-center items-center min-h-[70vh] px-4 py-6">
    <div class="w-full max-w-sm p-8 bg-white rounded-2xl shadow-lg">
      <h2 class="text-2xl font-bold text-center text-gray-800 mb-2">Регистрация в TIP</h2>
      <p class="text-sm text-center text-gray-500 mb-6">Создайте новый аккаунт</p>

      <div class="mb-6">
        <button type="button"
          class="flex w-full items-center justify-center gap-2 rounded border border-gray-400 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100 transition">
          <img src="/google.png" alt="Google" class="h-5 w-5" />
          Зарегистрироваться через Google
        </button>
      </div>

      <Form :validation-schema="registerSchema" @submit="handleRegister">
        <div class="space-y-4">
          <div class="flex gap-4">
            <Field name="firstName" v-slot="{ field, errorMessage }">
              <div class="space-y-1 w-1/2">
                <input v-bind="field" type="text" placeholder="Имя" class="input-field border-gray-400"
                  :class="{ 'border-red-500': errorMessage }" autocomplete="given-name" />
                <p class="h-4 text-left text-red-500 text-xs">{{ errorMessage || '' }}</p>
              </div>
            </Field>

            <Field name="lastName" v-slot="{ field, errorMessage }">
              <div class="space-y-1 w-1/2">
                <input v-bind="field" type="text" placeholder="Фамилия" class="input-field border-gray-400"
                  :class="{ 'border-red-500': errorMessage }" autocomplete="family-name" />
                <p class="h-4 text-left text-red-500 text-xs">{{ errorMessage || '' }}</p>
              </div>
            </Field>
          </div>

          <Field name="email" v-slot="{ field, errorMessage }">
            <div class="space-y-1">
              <input v-bind="field" type="email" placeholder="Email адрес" class="input-field border-gray-400"
                :class="{ 'border-red-500': errorMessage }" autocomplete="email" />
              <p class="h-4 text-left text-red-500 text-xs">{{ errorMessage || '' }}</p>
            </div>
          </Field>

          <Field name="password" v-slot="{ field, errorMessage }">
            <div class="relative space-y-1">
              <input v-bind="field" :type="showPassword ? 'text' : 'password'" placeholder="Пароль"
                class="input-field border-gray-400 pr-10" :class="{ 'border-red-500': errorMessage }"
                autocomplete="current-password" />

              <button type="button" @click="togglePasswordVisibility"
                class="absolute right-0 top-[-0.35rem] text-gray-500 hover:text-gray-700 focus:outline-none">
                <svg v-if="showPassword" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none"
                  viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round"
                    d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>

                <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round"
                    d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.542-7a9.956 9.956 0 012.224-3.592m1.249-1.287A9.956 9.956 0 0112 5c4.478 0 8.268 2.943 9.542 7a9.957 9.957 0 01-4.043 5.272M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" d="M3 3l18 18" />
                </svg>
              </button>

              <p class="h-4 text-left text-red-500 text-xs">{{ errorMessage || '' }}</p>
            </div>
          </Field>
        </div>

        <button type="submit" :disabled="submitting"
          class="w-full mt-6 py-2 px-4 text-white bg-blue-600 hover:bg-blue-700 font-medium rounded transition disabled:opacity-50">
          {{ submitting ? 'Проверка...' : 'Создать аккаунт' }}
        </button>
      </Form>

      <p class="mt-4 text-center text-sm text-gray-600">
        Уже есть аккаунт?
        <RouterLink to="/login" class="font-medium text-blue-600 hover:text-blue-500">Войти</RouterLink>
      </p>
    </div>
  </div>
</template>

<style scoped></style>
