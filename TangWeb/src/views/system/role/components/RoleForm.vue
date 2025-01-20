<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { addRole, updateRole } from '@/api/role'
import type { SysRole } from '@/types/api'

const props = defineProps<{
  visible: boolean
  title?: string
  roleData?: Partial<SysRole>
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive<Partial<SysRole>>({
  roleName: '',
  roleCode: '',
  status: 1,
  remark: ''
})

const rules: FormRules = {
  roleName: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  roleCode: [
    { required: true, message: '请输入角色编码', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ]
}

// 初始化表单数据
const initForm = () => {
  if (props.roleData) {
    Object.assign(formData, props.roleData)
  } else {
    Object.assign(formData, {
      roleName: '',
      roleCode: '',
      status: 1,
      remark: ''
    })
  }
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        if (props.roleData?.id) {
          await updateRole(formData)
          ElMessage.success('修改成功')
        } else {
          await addRole(formData)
          ElMessage.success('添加成功')
        }
        emit('success')
        handleClose()
      } catch (error) {
        console.error(error)
      } finally {
        loading.value = false
      }
    }
  })
}

// 关闭弹窗
const handleClose = () => {
  emit('update:visible', false)
  formRef.value?.resetFields()
}

// 监听visible变化
watch(() => props.visible, (val) => {
  if (val) {
    initForm()
  }
})
</script>

<template>
  <el-dialog
    :title="title || (roleData?.id ? '编辑角色' : '新增角色')"
    :model-value="visible"
    @update:model-value="$emit('update:visible', $event)"
    @closed="handleClose"
    width="500px"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="rules"
      label-width="80px"
    >
      <el-form-item label="角色名称" prop="roleName">
        <el-input v-model="formData.roleName" />
      </el-form-item>
      <el-form-item label="角色编码" prop="roleCode">
        <el-input v-model="formData.roleCode" :disabled="!!roleData?.id" />
      </el-form-item>
      <el-form-item label="状态">
        <el-radio-group v-model="formData.status">
          <el-radio :label="1">启用</el-radio>
          <el-radio :label="0">禁用</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="备注">
        <el-input 
          v-model="formData.remark" 
          type="textarea" 
          :rows="3" 
        />
      </el-form-item>
    </el-form>
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