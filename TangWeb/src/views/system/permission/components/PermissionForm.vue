<script setup lang="ts">
import { ref, reactive, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { addPermission, updatePermission, getPermissionTree } from '@/api/permission'
import type { SysPermission } from '@/types/api'
import IconSelect from '@/components/IconSelect/index.vue'

const props = defineProps<{
  visible: boolean
  title?: string
  permissionData?: Partial<SysPermission>
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const formRef = ref<FormInstance>()
const loading = ref(false)
const parentOptions = ref<SysPermission[]>([])

const formData = reactive<Partial<SysPermission>>({})

const rules: FormRules = {
  name: [
    { required: true, message: '请输入权限名称', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  permissionCode: [
    { required: true, message: '请输入权限编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  path: [
    { required: true, message: '请输入路由地址', trigger: 'blur', 
      validator: (rule, value, callback) => {
        if (formData.type === 1 && !value) {
          callback(new Error('菜单类型必须填写路由地址'))
        } else {
          callback()
        }
      }
    }
  ],
  component: [
    { required: true, message: '请输入组件路径', trigger: 'blur',
      validator: (rule, value, callback) => {
        if (formData.type === 1 && !value) {
          callback(new Error('菜单类型必须填写组件路径'))
        } else {
          callback()
        }
      }
    }
  ]
}

// 加载父级权限选项
const loadParentOptions = async () => {
  try {
    const data = await getPermissionTree()
    parentOptions.value = data
  } catch (error) {
    console.error('获取权限树失败:', error)
  }
}

// 计算处理后的父级权限选项
const processedParentOptions = computed(() => {
  // 如果是编辑状态，需要过滤掉当前节点及其子节点
  if (props.permissionData?.id) {
    const filterChildren = (tree: SysPermission[]): SysPermission[] => {
      return tree.filter(node => {
        if (node.id === props.permissionData?.id) return false
        if (node.children) {
          node.children = filterChildren(node.children)
        }
        return true
      })
    }
    return filterChildren(parentOptions.value)
  }
  return parentOptions.value
})

// 初始化表单数据
const initForm = () => {
  if (props.permissionData) {
    // 编辑时，复制传入的数据
    Object.assign(formData, props.permissionData)
  } else {
    // 新增时，只设置必要的默认值
    Object.assign(formData, {
      type: 1,  // 默认为菜单类型
      sort: 0,  // 默认排序为0
      status: 1 // 默认启用
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
        if (props.permissionData?.id) {
          await updatePermission(formData)
          ElMessage.success('修改成功')
        } else {
          await addPermission(formData)
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
  // 清空表单数据
  Object.keys(formData).forEach(key => {
    delete formData[key]
  })
  formRef.value?.resetFields()
}

// 监听visible变化
watch(() => props.visible, (val) => {
  if (val) {
    initForm()
    loadParentOptions()
  }
})
</script>

<template>
  <el-dialog
    :title="title || (permissionData?.id ? '编辑权限' : '新增权限')"
    :model-value="visible"
    @update:model-value="$emit('update:visible', $event)"
    @closed="handleClose"
    width="600px"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="rules"
      label-width="100px"
    >
      <el-form-item label="上级权限">
        <el-tree-select
          v-model="formData.parentId"
          :data="processedParentOptions"
          :props="{
            value: 'id',
            label: 'name',
            children: 'children'
          }"
          placeholder="请选择上级权限，不选则为顶级权限"
          check-strictly
          default-expand-all
          clearable
        />
      </el-form-item>
      <el-form-item label="权限名称" prop="name">
        <el-input v-model="formData.name" />
      </el-form-item>
      <el-form-item label="权限类型" prop="type">
        <el-radio-group v-model="formData.type">
          <el-radio :label="1">菜单</el-radio>
          <el-radio :label="2">按钮</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="权限编码" prop="permissionCode">
        <el-input v-model="formData.permissionCode" />
      </el-form-item>
      <template v-if="formData.type === 1">
        <el-form-item label="路由地址" prop="path">
          <el-input v-model="formData.path" />
        </el-form-item>
        <el-form-item label="组件路径" prop="component">
          <el-input v-model="formData.component" />
        </el-form-item>
        <el-form-item label="图标" prop="icon">
          <icon-select v-model="formData.icon" />
        </el-form-item>
      </template>
      <el-form-item label="排序" prop="sort">
        <el-input-number v-model="formData.sort" :min="0" />
      </el-form-item>
      <el-form-item label="状态">
        <el-radio-group v-model="formData.status">
          <el-radio :label="1">启用</el-radio>
          <el-radio :label="0">禁用</el-radio>
        </el-radio-group>
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