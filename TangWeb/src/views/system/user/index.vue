<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
import { getUserPage, deleteUser } from '@/api/user'
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

// 分页相关
const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)

// 加载用户列表
const loadUsers = async () => {
  loading.value = true
  const res = await getUserPage({ 
    PageIndex: pageIndex.value,
    PageSize: pageSize.value,
    keyword: keyword.value 
  })
  console.log('User list response:', res)
  userList.value = res.data.items
  total.value = res.data.total
  loading.value = false
}

// 处理分页变化
const handlePageChange = (page: number) => {
  pageIndex.value = page
  loadUsers()
}

const handleSizeChange = (size: number) => {
  pageSize.value = size
  pageIndex.value = 1
  loadUsers()
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
    <div class="header">
      <el-input v-model="keyword" placeholder="请输入用户名/昵称" clearable @clear="loadUsers" style="width: 200px">
        <template #prefix>
          <el-icon>
            <Search />
          </el-icon>
        </template>
      </el-input>
      <el-button type="primary" @click="loadUsers">搜索</el-button>
      <el-button type="success" @click="handleAdd">新增用户</el-button>
    </div>

    <div class="table-wrapper">
      <el-table 
        v-loading="loading" 
        :data="userList" 
        border 
        style="width: 100%"
        height="100%"
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
            <el-button type="success" link @click="handleAssignRole(row)">
              分配角色
            </el-button>
            <el-button type="danger" link @click="handleDelete(row.id)">
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <div class="footer">
      <el-pagination
        v-model:current-page="pageIndex"
        v-model:page-size="pageSize"
        :total="total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        background
        :pager-count="7"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
      />
    </div>

    <!-- 用户表单 -->
    <user-form v-model:visible="formVisible" :user-data="currentUser" @success="handleSuccess" />

    <!-- 用户角色分配 -->
    <user-role v-model:visible="roleVisible" :user="currentRoleUser" @success="loadUsers" />
  </div>
</template>

<style scoped>
.user-container {
  padding: 20px;
  height: calc(100vh - 84px); /* 减去顶部导航栏高度 */
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.header {
  display: flex;
  gap: 10px;
}

.table-wrapper {
  flex: 1;
  overflow: hidden;
}

.footer {
  display: flex;
  justify-content: flex-end;
}
</style>