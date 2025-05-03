import { ref, computed } from "vue";

const isAuthenticated = ref(false);

export const useAuthStore = () => {
  const checkAuth = () => {
    isAuthenticated.value = !!localStorage.getItem("token");
  };

  const login = () => {
    isAuthenticated.value = true;
  };

  const logout = () => {
    isAuthenticated.value = false;
  };

  return {
    isAuthenticated: computed(() => isAuthenticated.value),
    checkAuth,
    login,
    logout,
  };
};
