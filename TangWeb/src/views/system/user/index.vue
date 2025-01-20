<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
import { getUserList, deleteUser } from '@/api/user'
import type { SysUser } from '@/types/api'
import UserForm from './components/UserForm.vue'
import UserRole from './components/UserRole.vue'

const userList = ref<SysUser[]>([])
const loading = ref(false)
const keyword = ref('')

// 表单相关
const formVisible = ref(false)
const currentUser = ref<Partial<SysUser>>()

// 角色分配相关
const roleVisible = ref(false)
const currentRoleUser = ref<SysUser>()

// 加载用户列表
const loadUsers = async () => {
  loading.value = true
  try {
    const data = await getUserList({ keyword: keyword.value })
    console.log('User list response:', data)  // 添加调试日志
    userList.value = data
  } catch (error) {
    console.error('获取用户列表失败:', error)
    ElMessage.error(error.message || '获取用户列表失败')
  } finally {
    loading.value = false
  }
}

// 添加用户
const handleAdd = () => {
  currentUser.value = undefined
  formVisible.value = true
}

// 编辑用户
const handleEdit = (row: SysUser) => {
  currentUser.value = { ...row }
  formVisible.value = true
}

// 删除用户
const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要删除该用户吗？', '提示', {
      type: 'warning'
    })
    await deleteUser(id)
    ElMessage.success('删除成功')
    loadUsers()
  } catch (error) {
    console.error('删除用户失败:', error)
  }
}

// 表单提交成功
const handleSuccess = () => {
  loadUsers()
}

// 分配角色
const handleAssignRole = (row: SysUser) => {
  currentRoleUser.value = row
  roleVisible.value = true
}

onMounted(() => {
  loadUsers()
})
</script>

<template>
  <div class="user-container">
    <div class="search-bar">
      <el-input
        v-model="keyword"
        placeholder="请输入用户名/昵称"
        clearable
        @clear="loadUsers"
        style="width: 200px"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>
      <el-button type="primary" @click="loadUsers">搜索</el-button>
      <el-button type="success" @click="handleAdd">新增用户</el-button>
    </div>

    <el-table
      v-loading="loading"
      :data="userList"
      border
      style="width: 100%"
    >
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="userName" label="用户名" />
      <el-table-column prop="nickName" label="昵称" />
      <el-table-column prop="status" label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? '启用' : '禁用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="180">
        <template #default="{ row }">
          {{ new Date(row.createTime).toLocaleString() }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" link @click="handleEdit(row)">
            编辑
          </el-button>
          <el-button 
            type="success" 
            link
            @click="handleAssignRole(row)"
          >
            分配角色
          </el-button>
          <el-button 
            type="danger" 
            link 
            @click="handleDelete(row.id)"
          >
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 用户表单 -->
    <user-form
      v-model:visible="formVisible"
      :user-data="currentUser"
      @success="handleSuccess"
    />

    <!-- 用户角色分配 -->
    <user-role
      v-model:visible="roleVisible"
      :user="currentRoleUser"
      @success="loadUsers"
    />
  </div>
</template>

<style scoped>
.user-container {
  padding: 20px;
}

.search-bar {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
}
</style> 