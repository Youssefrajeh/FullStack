const routes = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      {
        path: '', // Default child path
        name: 'home',
        component: () => import('pages/HomePage.vue'),
      },
      {
        path: 'brands', // Correct path for brands
        name: 'brands',
        component: () => import('pages/BrandPage.vue'),
      },
      {
        path: 'Cart',
        name: 'Cart',
        component: () => import('pages/CartPage.vue'),
      },
      {
        path: 'Products',
        name: 'products',
        component: () => import('pages/ProductPage.vue'),
      },
      {
        path: 'register',
        name: 'register',
        component: () => import('pages/RegisterPage.vue'),
      },
      {
        path: 'login',
        name: 'login',
        component: () => import('pages/LoginPage.vue'),
      },
      {
        path: 'logout',
        name: 'logout',
        component: () => import('pages/LogoutPage.vue'),
      },
      {
        path: 'order-history',
        name: 'order-history',
        component: () => import('pages/OrderHistoryPage.vue'),
      },
      {
        path: 'maps',
        name: 'maps',
        component: () => import('pages/MapEx1Page.vue'),
      },
      {
        path: 'maps2',
        name: 'maps2',
        component: () => import('pages/MapEx2.vue'),
      },
      {
        path: 'maps3',
        name: 'maps3',
        component: () => import('pages/MapEx3.vue'),
      },
      {
        path: 'utility',
        name: 'utility',
        component: () => import('pages/UtilPage.vue'),
      },
      {
        path: 'branches',
        name: 'branches',
        component: () => import('pages/BranchLocator.vue'),
      },
    ],
  },
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
]
export default routes
