import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import * as ElementPlusIcons from '@element-plus/icons-vue'
import 'element-plus/dist/index.css'
import router from './router'
import App from './App.vue'
import './style.css'

const app = createApp(App)

// 注册所有图标
for (const [key, component] of Object.entries(ElementPlusIcons)) {
  app.component(key, component)
}

app.use(ElementPlus, {
  locale: zhCn,
})
app.use(createPinia())
app.use(router)

app.mount('#app')
