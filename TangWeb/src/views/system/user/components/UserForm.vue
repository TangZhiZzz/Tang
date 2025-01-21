<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { addUser, updateUser } from '@/api/user'
import type { SysUser } from '@/types/api'

const props = defineProps<{
  visible: boolean
  title?: string
  userData?: Partial<SysUser>
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive<Partial<SysUser>>({
  userName: '',
  password: '',
  nickName: '',
  status: 1,
  remark: ''
})

const rules: FormRules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  nickName: [
    { required: true, message: '请输入昵称', trigger: 'blur' }
  ]
}

// 初始化表单数据
const initForm = () => {
  if (props.userData) {
    Object.assign(formData, props.userData)
  } else {
    Object.assign(formData, {
      userName: '',
      password: '',
      nickName: '',
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
        if (props.userData?.id) {
          await updateUser(formData)
          ElMessage.success('修改成功')
        } else {
          await addUser(formData)
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
  <el-dialog :title="title || (userData?.id ? '编辑用户' : '新增用户')" :model-value="visible"
    @update:model-value="$emit('update:visible', $event)" @closed="handleClose" width="500px">
    <el-form ref="formRef" :model="formData" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="userName">
        <el-input v-model="formData.userName" :disabled="!!userData?.id" />
      </el-form-item>
      <el-form-item label="密码" prop="password" v-if="!userData?.id">
        <el-input v-model="formData.password" type="password" show-password />
      </el-form-item>
      <el-form-item label="昵称" prop="nickName">
        <el-input v-model="formData.nickName" />
      </el-form-item>
      <el-form-item label="状态">
        <el-radio-group v-model="formData.status">
          <el-radio :label="1">启用</el-radio>
          <el-radio :label="0">禁用</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="备注">
        <el-input v-model="formData.remark" type="textarea" :rows="3" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" :loading="loading" @click="handleSubmit">
        确定
      </el-button>
    </template>
  </el-dialog>
</template>