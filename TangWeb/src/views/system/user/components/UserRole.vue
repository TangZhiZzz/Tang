<script setup lang="ts">
import { ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getRoleList } from '@/api/role'
import { getUserRoles, setUserRoles } from '@/api/user'
import type { SysRole, SysUser } from '@/types/api'

const props = defineProps<{
  visible: boolean
  user?: SysUser
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const loading = ref(false)
const roleList = ref<SysRole[]>([])
const selectedRoles = ref<number[]>([])

// 加载角色列表
const loadRoles = async () => {
  try {
    const data = await getRoleList()
    roleList.value = data
  } catch (error) {
    console.error('获取角色列表失败:', error)
  }
}

// 加载用户角色
const loadUserRoles = async () => {
  if (!props.user?.id) return
  
  try {
    const data = await getUserRoles(props.user.id)
    selectedRoles.value = data.map((item) => item.id)
  } catch (error) {
    console.error('获取用户角色失败:', error)
  }
}

// 提交分配
const handleSubmit = async () => {
  if (!props.user?.id) return
  
  loading.value = true
  try {
    await setUserRoles(props.user.id, selectedRoles.value)
    ElMessage.success('分配成功')
    emit('success')
    handleClose()
  } catch (error) {
    console.error('分配角色失败:', error)
  } finally {
    loading.value = false
  }
}

// 关闭弹窗
const handleClose = () => {
  emit('update:visible', false)
  selectedRoles.value = []
}

// 监听visible变化
watch(() => props.visible, async (val) => {
  if (val && props.user) {
    await loadRoles()
    await loadUserRoles()
  }
})
</script>

<template>
  <el-dialog
    :title="`分配角色 - ${user?.userName || ''}`"
    :model-value="visible"
    @update:model-value="$emit('update:visible', $event)"
    @closed="handleClose"
    width="500px"
  >
    <div v-loading="loading">
      <el-checkbox-group v-model="selectedRoles">
        <el-space wrap>
          <el-checkbox
            v-for="role in roleList"
            :key="role.id"
            :label="role.id"
            :disabled="role.status === 0"
          >
            {{ role.roleName }}
            <el-tag 
              size="small" 
              :type="role.status === 1 ? 'success' : 'danger'"
              class="role-status"
            >
              {{ role.status === 1 ? '启用' : '禁用' }}
            </el-tag>
          </el-checkbox>
        </el-space>
      </el-checkbox-group>
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

<style scoped>
.role-status {
  margin-left: 8px;
}
</style> 