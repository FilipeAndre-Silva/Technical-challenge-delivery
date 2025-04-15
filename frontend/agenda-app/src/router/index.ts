import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';
import RegisterView from '@/views/RegisterView.vue';
import LoginView from '@/views/LoginView.vue';
import DashboardView from '@/views/DashboardView.vue';
import UpdateEmailView from '@/views/UpdateEmailView.vue';
import CreateContactView from '@/views/CreateContactView.vue';
import ContactEditView from '@/views/ContactEditView.vue';


const routes: Array<RouteRecordRaw> = [
  {
    path: '/register',
    name: 'Register',
    component: RegisterView,
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
  },
  {
    path: '/reset-password',
    name: 'ResetPassword',
    component: () => import('@/views/ResetPasswordView.vue'),
  },
  {
    path: '/reset-password-confirmation',
    name: 'ResetPasswordConfirmation',
    component: () => import('@/views/ResetPasswordConfirmationView.vue'),
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: DashboardView,
    meta: { requiresAuth: true },
  },
  {
    path: '/update-email',
    name: 'UpdateEmail',
    component: UpdateEmailView,
    meta: { requiresAuth: true },
  },
  {
    path: '/',
    redirect: '/login',
  },
  {
    path: '/contacts/create',
    name: 'create-contact',
    component: CreateContactView,
    meta: { requiresAuth: true },
  },
  {
    path: '/contacts/edit/:id',
    name: 'update-contact',
    component: ContactEditView,
    meta: { requiresAuth: true },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('authToken');

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login');
  } else {
    next();
  }
});

export default router;