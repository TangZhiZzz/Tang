import { createRouter, createWebHistory } from 'vue-router'
import Layout from '../layout/index.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('../views/login/index.vue'),
      meta: { title: '登录' }
    },
    {
      path: '/',
      component: Layout,
      redirect: '/dashboard',
      children: [
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: () => import('../views/dashboard/index.vue'),
          meta: { title: '仪表盘', icon: 'Monitor' }
        }
      ]
    },
    {
      path: '/system',
      component: Layout,
      redirect: '/system/user',
      meta: { title: '系统管理', icon: 'Setting' },
      children: [
        {
          path: 'user',
          name: 'User',
          component: () => import('../views/system/user/index.vue'),
          meta: { title: '用户管理' }
        },
        {
          path: 'role',
          name: 'Role',
          component: () => import('../views/system/role/index.vue'),
          meta: { title: '角色管理' }
        },
        {
          path: 'permission',
          name: 'Permission',
          component: () => import('../views/system/permission/index.vue'),
          meta: { title: '权限管理' }
        }
      ]
    }
  ]
})

// 路由守卫
router.beforeEach((to, from, next) => {
  // 设置标题
  document.title = `${to.meta.title} - Tang Admin`
  
  // 判断是否需要登录
  const token = localStorage.getItem('token')
  if (to.path !== '/login' && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router 