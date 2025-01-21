<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
import { getPermissionTree, deletePermission, updatePermission } from '@/api/permission'
import type { SysPermission } from '@/types/api'
import PermissionForm from './components/PermissionForm.vue'

const permissionList = ref<SysPermission[]>([])
const loading = ref(false)
const keyword = ref('')

// 表单相关
const formVisible = ref(false)
const currentPermission = ref<Partial<SysPermission>>()

// 加载权限树
const loadPermissions = async () => {
  loading.value = true
  const res = await getPermissionTree({ keyword: keyword.value })
  permissionList.value = res.data
  loading.value = false

}

// 添加权限
const handleAdd = (parentId?: number) => {
  currentPermission.value = parentId ? { parentId } : undefined
  formVisible.value = true
}

// 编辑权限
const handleEdit = (row: SysPermission) => {
  currentPermission.value = { ...row }
  formVisible.value = true
}

// 删除权限
const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要删除该权限吗？删除后其子权限也会被删除！', '提示', {
      type: 'warning'
    })
    await deletePermission(id)
    ElMessage.success('删除成功')
    loadPermissions()
  } catch (error) {
    console.error('删除权限失败:', error)
  }
}

// 切换状态
const handleStatusChange = async (row: SysPermission) => {
  try {
    await updatePermission({
      ...row,
      status: row.status === 1 ? 0 : 1
    })
    ElMessage.success('更新成功')
    loadPermissions()
  } catch (error) {
    console.error('更新状态失败:', error)
    // 恢复原状态
    row.status = row.status === 1 ? 0 : 1
  }
}

// 表单提交成功
const handleSuccess = () => {
  loadPermissions()
}

onMounted(() => {
  loadPermissions()
})
</script>

<template>
  <div class="permission-container">
    <div class="search-bar">
      <el-input v-model="keyword" placeholder="请输入权限名称" clearable @clear="loadPermissions" style="width: 200px">
        <template #prefix>
          <el-icon>
            <Search />
          </el-icon>
        </template>
      </el-input>
      <el-button type="primary" @click="loadPermissions">搜索</el-button>
      <el-button type="success" @click="handleAdd()">新增权限</el-button>
    </div>

    <el-table v-loading="loading" :data="permissionList" border row-key="id" :tree-props="{ children: 'children' }"
      style="width: 100%">
      <el-table-column prop="name" label="权限名称" min-width="200">
        <template #default="{ row }">
          <span>{{ row.name }}</span>
          <el-button type="primary" link @click.stop="handleAdd(row.id)">
            新增子权限
          </el-button>
        </template>
      </el-table-column>
      <el-table-column prop="permissionCode" label="权限编码" min-width="150" />
      <el-table-column prop="type" label="类型" width="100">
        <template #default="{ row }">
          <el-tag :type="row.type === 1 ? 'primary' : 'success'">
            {{ row.type === 1 ? '菜单' : '按钮' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="path" label="路由地址" min-width="150" />
      <el-table-column prop="component" label="组件路径" min-width="150" />
      <el-table-column prop="icon" label="图标" width="100">
        <template #default="{ row }">
          <el-icon v-if="row.icon">
            <component :is="row.icon" />
          </el-icon>
        </template>
      </el-table-column>
      <el-table-column prop="sort" label="排序" width="80" />
      <el-table-column prop="status" label="状态" width="100">
        <template #default="{ row }">
          <el-switch v-model="row.status" :active-value="1" :inactive-value="0" @change="handleStatusChange(row)" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="180" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" link @click="handleEdit(row)">
            编辑
          </el-button>
          <el-button type="danger" link @click="handleDelete(row.id)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 权限表单 -->
    <permission-form v-model:visible="formVisible" :permission-data="currentPermission" @success="handleSuccess" />
  </div>
</template>

<style scoped>
.permission-container {
  padding: 20px;
}

.search-bar {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
}
</style>