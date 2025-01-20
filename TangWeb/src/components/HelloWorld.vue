<script setup lang="ts">
import { ref } from 'vue'
import { useCounterStore } from '../stores/counter'
import { getUserInfo } from '../api/user'

defineProps<{ msg: string }>()
const store = useCounterStore()

// 测试 API 调用
const testApi = async () => {
  try {
    const res = await getUserInfo()
    console.log('用户信息:', res)
  } catch (error) {
    console.error('获取用户信息失败:', error)
  }
}
</script>

<template>
  <h1>{{ msg }}</h1>

  <div class="card">
    <el-button type="primary" @click="store.increment">
      Pinia count is {{ store.count }}
    </el-button>
    <el-button type="success" @click="testApi">测试 API</el-button>
    <p>
      Edit
      <code>components/HelloWorld.vue</code> to test HMR
    </p>
  </div>

  <p>
    Check out
    <a href="https://vuejs.org/guide/quick-start.html#local" target="_blank"
      >create-vue</a
    >, the official Vue + Vite starter
  </p>
  <p>
    Learn more about IDE Support for Vue in the
    <a
      href="https://vuejs.org/guide/scaling-up/tooling.html#ide-support"
      target="_blank"
      >Vue Docs Scaling up Guide</a
    >.
  </p>
  <p class="read-the-docs">Click on the Vite and Vue logos to learn more</p>
</template>

<style scoped>
.read-the-docs {
  color: #888;
}
</style>
