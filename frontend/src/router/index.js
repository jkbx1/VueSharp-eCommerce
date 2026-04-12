import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/sale',
      name: 'sale',
      component: () => import('../views/SalesView.vue')
    },
    {
      path: '/search',
      name: 'search',
      component: () => import('../views/SearchView.vue')
    },
    {
      path: '/track-order',
      name: 'track-order',
      component: () => import('../views/TrackOrderView.vue')
    },
    {
      path: '/novelties',
      name: 'novelties',
      component: () => import('../views/NoveltiesView.vue')
    },
    {
      path: '/kids',
      name: 'kids',
      component: () => import('../views/KidsView.vue')
    },
    {
      path: '/kids/:category',
      name: 'kids-category',
      component: () => import('../views/KidsView.vue')
    },
    {
      path: '/women',
      name: 'women',
      component: () => import('../views/WomenView.vue')
    },
    {
      path: '/women/:category',
      name: 'women-category',
      component: () => import('../views/WomenView.vue')
    },
    {
      path: '/men',
      name: 'men',
      component: () => import('../views/MenView.vue')
    },
    {
      path: '/men/:category',
      name: 'men-category',
      component: () => import('../views/MenView.vue')
    },
    {
      path: '/settings',
      name: 'settings',
      component: () => import('../views/SettingsView.vue')
    },
    {
      path: '/product/:id',
      name: 'product-detail',
      component: () => import('../views/ProductView.vue'),
      props: true
    },
    {
      path: '/checkout-gate',
      name: 'checkout-gate',
      component: () => import('../views/CheckoutGateView.vue')
    },
    {
      // Guest checkout is intentionally NOT guarded — anonymous orders are allowed.
      path: '/checkout',
      name: 'checkout',
      component: () => import('../views/CheckoutView.vue')
    },
    {
      path: '/order-confirmation/:id',
      name: 'order-confirmation',
      component: () => import('../views/OrderConfirmationView.vue'),
      props: true
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },
    {
      // ── All /account routes require authentication ──────────────────────────
      // The meta: { requiresAuth: true } on the parent is inherited by all
      // nested children (profile, orders, security) automatically.
      path: '/account',
      meta: { requiresAuth: true },
      component: () => import('../layouts/DashboardLayout.vue'),
      children: [
        { path: '', redirect: '/account/profile' },
        { path: 'profile', component: () => import('../views/ProfileView.vue') },
        { path: 'orders', component: () => import('../views/OrdersView.vue') },
        { path: 'security', component: () => import('../views/SecurityView.vue') }
      ]
    }
  ],
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// ── Global Navigation Guard ─────────────────────────────────────────────────
// Checks localStorage directly (no Pinia dependency at this stage) for a
// stored JWT token. If a route requires auth and the token is absent,
// the user is redirected to /login with their intended destination preserved
// as a `redirect` query parameter, e.g. /login?redirect=/account/orders.
// LoginView reads this param and navigates there after a successful login.
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)

  if (!requiresAuth) {
    next()
    return
  }

  const token = localStorage.getItem('vuesharp_jwt')

  if (token) {
    next()
  } else {
    next({
      name: 'login',
      query: { redirect: to.fullPath }
    })
  }
})

export default router
