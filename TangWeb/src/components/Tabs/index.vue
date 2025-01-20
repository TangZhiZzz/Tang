<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { RouteLocationNormalized } from 'vue-router'

interface TabItem {
  path: string
  title: string
  icon?: string
}

const route = useRoute()
const router = useRouter()

const tabs = ref<TabItem[]>([
  { path: '/dashboard', title: '仪表盘', icon: 'Monitor' }
])
const activeTab = ref('/dashboard')

// 添加标签页
const addTab = (route: RouteLocationNormalized) => {
  const { path, meta } = route
  if (!tabs.value.some(tab => tab.path === path)) {
    tabs.value.push({
      path,
      title: meta.title as string,
      icon: meta.icon as string
    })
  }
  activeTab.value = path
}

// 关闭标签页
const closeTab = (path: string) => {
  const index = tabs.value.findIndex(tab => tab.path === path)
  if (index === -1) return
  
  // 如果关闭的是当前标签，则跳转到前一个标签
  if (path === activeTab.value) {
    const nextTab = tabs.value[index - 1] || tabs.value[index + 1]
    if (nextTab) {
      router.push(nextTab.path)
    }
  }
  
  tabs.value.splice(index, 1)
}

// 关闭其他标签页
const closeOtherTabs = () => {
  const currentTab = tabs.value.find(tab => tab.path === activeTab.value)
  if (currentTab) {
    tabs.value = [
      { path: '/dashboard', title: '仪表盘', icon: 'Monitor' },
      ...(currentTab.path === '/dashboard' ? [] : [currentTab])
    ]
  }
}

// 关闭所有标签页
const closeAllTabs = () => {
  tabs.value = [{ path: '/dashboard', title: '仪表盘', icon: 'Monitor' }]
  router.push('/dashboard')
}

// 监听路由变化
watch(
  () => route.path,
  (path) => {
    if (path === '/login') return
    addTab(route)
  },
  { immediate: true }
)
</script>

<template>
  <div class="tabs-container">
    <el-scrollbar 
      class="tabs-scrollbar" 
      wrap-class="scrollbar-wrapper"
      :height="40"
    >
      <div class="tabs-wrapper">
        <el-tabs
          v-model="activeTab"
          type="card"
          @tab-click="(tab) => router.push(tab.props.name)"
          @tab-remove="closeTab"
        >
          <el-tab-pane
            v-for="tab in tabs"
            :key="tab.path"
            :label="tab.title"
            :name="tab.path"
            :closable="tab.path !== '/dashboard'"
          >
            <template #label>
              <el-icon v-if="tab.icon">
                <component :is="tab.icon" />
              </el-icon>
              <span>{{ tab.title }}</span>
            </template>
          </el-tab-pane>
        </el-tabs>
      </div>
    </el-scrollbar>
    
    <div class="tabs-actions">
      <el-dropdown trigger="click">
        <el-button type="primary" link>
          操作
          <el-icon class="el-icon--right">
            <arrow-down />
          </el-icon>
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item @click="closeOtherTabs">
              关闭其他标签页
            </el-dropdown-item>
            <el-dropdown-item @click="closeAllTabs">
              关闭所有标签页
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<style scoped>
.tabs-container {
  display: flex;
  align-items: center;
  height: 40px;
  padding: 0 16px;
  background: #fff;
  border-bottom: 1px solid #dcdfe6;
}

.tabs-scrollbar {
  flex: 1;
}

/* 隐藏纵向滚动条 */
:deep(.scrollbar-wrapper) {
  overflow-y: hidden;
}

.tabs-wrapper {
  height: 100%;
  display: flex;
  align-items: center;
}

:deep(.el-tabs) {
  height: 100%;
}

:deep(.el-tabs__header) {
  margin: 0;
}

:deep(.el-tabs__nav) {
  border: none !important;
}

:deep(.el-tabs__item) {
  height: 40px;
  line-height: 40px;
  border: none !important;
  background: transparent !important;
}

:deep(.el-tabs__item.is-active) {
  background: #f0f2f5 !important;
}

.el-dropdown {
  margin-left: 16px;
}

.el-icon {
  margin-right: 4px;
  vertical-align: middle;
}

span {
  vertical-align: middle;
}
</style> 