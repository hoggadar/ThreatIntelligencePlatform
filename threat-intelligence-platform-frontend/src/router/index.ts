import { createRouter, createWebHistory } from "vue-router";
import { useAuth } from "../services/authService";
import { getCookie } from "../utils/cookies";
import AnalyticsView from '../views/AnalyticsView.vue'

const routes = [
  {
    path: "/",
    name: "Home",
    component: () => import("../views/HomeView.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/dashboard",
    name: "Dashboard",
    component: () => import("../views/DashboardView.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/threats",
    name: "Threats",
    component: () => import("../views/ThreatsView.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/analytics",
    name: "Analytics",
    component: AnalyticsView,
    meta: { requiresAuth: true },
  },
  {
    path: "/login",
    name: "Login",
    component: () => import("../views/LoginView.vue"),
    meta: { guestOnly: true },
  },
  {
    path: "/register",
    name: "Register",
    component: () => import("../views/RegisterView.vue"),
    meta: { guestOnly: true },
  },
  {
    path: "/profile",
    name: "Profile",
    component: () => import("../views/ProfileView.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/admin",
    name: "Admin",
    component: () => import("../views/AdminView.vue"),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
  {
    path: "/contacts",
    name: "Contacts",
    component: () => import("../views/ContactsView.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/about",
    name: "About",
    component: () => import("../views/AboutView.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/policy",
    name: "Policy",
    component: () => import("../views/PolicyView.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/faq",
    name: "Faq",
    component: () => import("../views/FaqView.vue"),
    meta: { requiresAuth: false },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,

  scrollBehavior(to, from, savedPosition) {
    return savedPosition || { top: 0 };
  },
});

// Глобальный guard
router.beforeEach(async (to, from, next) => {
  const token = getCookie("auth_token");
  const isAuthenticated = !!token;
  const auth = useAuth();

  // Перенаправление на Dashboard если пользователь авторизован и пытается зайти на главную
  if (to.path === "/" && isAuthenticated) {
    next({ name: "Dashboard" });
    return;
  }

  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: "Login" });
  } else if (to.meta.guestOnly && isAuthenticated) {
    next({ name: "Dashboard" });
  } else if (to.meta.requiresAdmin) {
    try {
      const userData = await auth.getCurrentUser();
      if (userData.role !== "Admin") {
        next({ name: "Profile" });
      } else {
        next();
      }
    } catch (e) {
      next({ name: "Login" });
    }
  } else {
    next();
  }
});

export default router;
