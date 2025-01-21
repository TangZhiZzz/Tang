<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
import { getRoleList, deleteRole } from '@/api/role'
import type { SysRole } from '@/types/api'
import RoleForm from './components/RoleForm.vue'
import RolePermission from './components/RolePermission.vue'

const roleList = ref<SysRole[]>([])
const loading = ref(false)
const keyword = ref('')

// 表单相关
const formVisible = ref(false)
const currentRole = ref<Partial<SysRole>>()

// 权限分配相关
const permissionVisible = ref(false)
const currentPermissionRole = ref<SysRole>()

// 加载角色列表
const loadRoles = async () => {
  loading.value = true
  const res = await getRoleList({ keyword: keyword.value })
  roleList.value = res.data
  loading.value = false

}

// 添加角色
const handleAdd = () => {
  currentRole.value = undefined
  formVisible.value = true
}

// 编辑角色
const handleEdit = (row: SysRole) => {
  currentRole.value = { ...row }
  formVisible.value = true
}

// 删除角色
const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要删除该角色吗？', '提示', {
      type: 'warning'
    })
    await deleteRole(id)
    ElMessage.success('删除成功')
    loadRoles()
  } catch (error) {
    console.error('删除角色失败:', error)
  }
}

// 表单提交成功
const handleSuccess = () => {
  loadRoles()
}

// 分配权限
const handleAssignPermission = (row: SysRole) => {
  currentPermissionRole.value = row
  permissionVisible.value = true
}

onMounted(() => {
  loadRoles()
})
</script>

<template>
  <div class="role-container">
    <div class="search-bar">
      <el-input v-model="keyword" placeholder="请输入角色名称" clearable @clear="loadRoles" style="width: 200px">
        <template #prefix>
          <el-icon>
            <Search />
          </el-icon>
        </template>
      </el-input>
      <el-button type="primary" @click="loadRoles">搜索</el-button>
      <el-button type="success" @click="handleAdd">新增角色</el-button>
    </div>

    <el-table v-loading="loading" :data="roleList" border style="width: 100%">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="roleName" label="角色名称" />
      <el-table-column prop="roleCode" label="角色编码" />
      <el-table-column prop="status" label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? '启用' : '禁用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="remark" label="备注" show-overflow-tooltip />
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
          <el-button type="success" link @click="handleAssignPermission(row)">
            分配权限
          </el-button>
          <el-button type="danger" link @click="handleDelete(row.id)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 角色表单 -->
    <role-form v-model:visible="formVisible" :role-data="currentRole" @success="handleSuccess" />

    <!-- 角色权限分配 -->
    <role-permission v-model:visible="permissionVisible" :role="currentPermissionRole" @success="loadRoles" />
  </div>
</template>

<style scoped>
.role-container {
  padding: 20px;
}

.search-bar {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
}
</style>