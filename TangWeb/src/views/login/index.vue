<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { login } from '@/api/auth'
import type { LoginParams } from '@/types/api'

const router = useRouter()

const loginForm = reactive<LoginParams>({
  userName: 'admin',
  password: '123456'
})

const loading = ref(false)

const handleLogin = async () => {
  loading.value = true
  try {
    const res = await login(loginForm)
    console.log('Login response:', res)
    localStorage.setItem('token', res.data.token)
    ElMessage.success('登录成功')
    router.push('/')
  } catch (error) {
    console.error('登录失败:', error)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-container">
    <el-card class="login-card">
      <h2>Tang Admin</h2>
      <el-form>
        <el-form-item>
          <el-input v-model="loginForm.userName" placeholder="用户名" prefix-icon="User" />
        </el-form-item>
        <el-form-item>
          <el-input v-model="loginForm.password" type="password" placeholder="密码" prefix-icon="Lock"
            @keyup.enter="handleLogin" />
        </el-form-item>
        <el-form-item>
          <el-button :loading="loading" type="primary" class="login-button" @click="handleLogin">
            登录
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<style scoped>
.login-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f0f2f5;
}

.login-card {
  width: 360px;
  text-align: center;
}

.login-button {
  width: 100%;
}
</style>