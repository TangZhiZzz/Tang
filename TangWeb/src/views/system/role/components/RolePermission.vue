<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getPermissionTree } from '@/api/permission'
import { getRolePermissions, setRolePermissions } from '@/api/role'
import type { SysPermission, SysRole } from '@/types/api'
import type { ElTree } from 'element-plus'

const props = defineProps<{
  visible: boolean
  role?: SysRole
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const loading = ref(false)
const permissionTree = ref<SysPermission[]>([])
const checkedKeys = ref<number[]>([])
const treeRef = ref<InstanceType<typeof ElTree>>()

const defaultProps = {
  children: 'children',
  label: 'name'
}

// 加载权限树
const loadPermissionTree = async () => {
  try {
    const data = await getPermissionTree()
    permissionTree.value = data
  } catch (error) {
    console.error('获取权限树失败:', error)
  }
}

// 加载角色权限
const loadRolePermissions = async () => {
  if (!props.role?.id) return
  
  try {
    const data = await getRolePermissions(props.role.id)
    checkedKeys.value = data.map((item) => item.id)
  } catch (error) {
    console.error('获取角色权限失败:', error)
  }
}

// 提交分配
const handleSubmit = async () => {
  if (!props.role?.id || !treeRef.value) return
  
  loading.value = true
  try {
    // 获取所有选中的节点（包括半选中的父节点）
    const checkedNodes = treeRef.value.getCheckedNodes()
    const halfCheckedNodes = treeRef.value.getHalfCheckedNodes()
    const allSelectedIds = [...checkedNodes, ...halfCheckedNodes].map(node => node.id)
    
    console.log('Selected permissions:', allSelectedIds)
    await setRolePermissions(props.role.id, allSelectedIds)
    ElMessage.success('分配成功')
    emit('success')
    handleClose()
  } catch (error) {
    console.error('分配权限失败:', error)
  } finally {
    loading.value = false
  }
}

// 关闭弹窗
const handleClose = () => {
  emit('update:visible', false)
  checkedKeys.value = []
}

// 监听visible变化
watch(() => props.visible, async (val) => {
  if (val && props.role) {
    await loadPermissionTree()
    await loadRolePermissions()
  }
})
</script>

<template>
  <el-dialog
    :title="`分配权限 - ${role?.roleName || ''}`"
    :model-value="visible"
    @update:model-value="$emit('update:visible', $event)"
    @closed="handleClose"
    width="600px"
  >
    <div v-loading="loading">
      <el-tree
        ref="treeRef"
        :data="permissionTree"
        :props="defaultProps"
        show-checkbox
        node-key="id"
        :default-checked-keys="checkedKeys"
        default-expand-all
      />
    </div>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button 
        type="primary" 
        :loading="loading"
        @click="handleSubmit"
      >
        确定
      </el-button>
    </template>
  </el-dialog>
</template> 