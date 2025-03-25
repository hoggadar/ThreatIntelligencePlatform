import { createRouter, createWebHistory } from "vue-router";

  
const routes = [
  {
    path: "/",
    redirect: "/dashboard",
  },
  {
    path: "/dashboard",
    name: "Dashboard",
    component: () => import("../views/DashboardView.vue"),
  },
  {
    path: "/threats",
    name: "Threats",
    component: () => import("../views/ThreatsView.vue"),
  },
  {
    path: "/analytics",
    name: "Analytics",
    component: () => import("../views/AnalyticsView.vue"),
  },
  {
    path: "/login",
    name: "Login",
    component: () => import("../views/LoginView.vue"),
  },
  {
    path: "/register",
    name: "Register",
    component: () => import("../views/RegisterView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
