<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getUserList } from '@/api/user'
import { getRoleList } from '@/api/role'
import { getPermissionTree } from '@/api/permission'

const loading = ref(false)
const statistics = ref({
  userCount: 0,
  roleCount: 0,
  permissionCount: 0
})

const loadStatistics = async () => {
  loading.value = true
  try {
    const [users, roles, permissions] = await Promise.all([
      getUserList(),
      getRoleList(),
      getPermissionTree()
    ])
    console.log(users.data);  
    statistics.value = {
      userCount: users.data.length,
      roleCount: roles.data.length,
      permissionCount: countPermissions(permissions.data)
    }
  } catch (error) {
    console.error('加载统计数据失败:', error)
  } finally {
    loading.value = false
  }
}

// 递归计算权限总数
const countPermissions = (permissions: any[]): number => {
  return permissions.reduce((total, item) => {
    return total + 1 + (item.children?.length ? countPermissions(item.children) : 0)
  }, 0)
}

onMounted(() => {
  loadStatistics()
})
</script>

<template>
  <div class="dashboard" v-loading="loading">
    <h2>系统概览</h2>
    
    <el-row :gutter="20" class="statistics">
      <el-col :span="8">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>用户总数</span>
              <el-icon><User /></el-icon>
            </div>
          </template>
          <div class="card-content">
            {{ statistics.userCount }}
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>角色总数</span>
              <el-icon><Lock /></el-icon>
            </div>
          </template>
          <div class="card-content">
            {{ statistics.roleCount }}
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>权限总数</span>
              <el-icon><Setting /></el-icon>
            </div>
          </template>
          <div class="card-content">
            {{ statistics.permissionCount }}
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style scoped>
.dashboard {
  padding: 20px;
}

.statistics {
  margin-top: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-content {
  font-size: 36px;
  font-weight: bold;
  text-align: center;
  color: #409EFF;
  padding: 20px 0;
}
</style> 