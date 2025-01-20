<script setup lang="ts">
import { ref, computed } from 'vue'
import * as ElementPlusIcons from '@element-plus/icons-vue'

const props = defineProps<{
  modelValue?: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const searchKeyword = ref('')
const dialogVisible = ref(false)

// 计算当前值
const currentValue = computed({
  get: () => props.modelValue || '',
  set: (value) => emit('update:modelValue', value)
})

// 获取所有图标
const iconList = computed(() => {
  const icons = Object.keys(ElementPlusIcons).map(key => ({
    name: key,
    component: ElementPlusIcons[key as keyof typeof ElementPlusIcons]
  }))
  
  if (!searchKeyword.value) return icons
  
  return icons.filter(icon => 
    icon.name.toLowerCase().includes(searchKeyword.value.toLowerCase())
  )
})

// 选择图标
const handleSelect = (iconName: string) => {
  currentValue.value = iconName
  dialogVisible.value = false
}

// 清除图标
const handleClear = () => {
  currentValue.value = ''
}
</script>

<template>
  <div class="icon-select">
    <el-input
      :model-value="currentValue"
      @update:model-value="currentValue = $event"
      placeholder="请选择图标"
      readonly
      clearable
      @clear="handleClear"
    >
      <template #prefix>
        <el-icon v-if="currentValue">
          <component :is="currentValue" />
        </el-icon>
        <el-icon v-else>
          <Picture />
        </el-icon>
      </template>
      <template #append>
        <el-button @click="dialogVisible = true">
          选择图标
        </el-button>
      </template>
    </el-input>

    <el-dialog
      title="选择图标"
      v-model="dialogVisible"
      width="800px"
    >
      <el-input
        v-model="searchKeyword"
        placeholder="搜索图标"
        clearable
        style="margin-bottom: 20px"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>

      <div class="icon-list">
        <div
          v-for="icon in iconList"
          :key="icon.name"
          class="icon-item"
          @click="handleSelect(icon.name)"
        >
          <el-icon>
            <component :is="icon.component" />
          </el-icon>
          <span class="icon-name">{{ icon.name }}</span>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<style scoped>
.icon-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 16px;
  max-height: 500px;
  overflow-y: auto;
}

.icon-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 16px;
  border: 1px solid #eee;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
}

.icon-item:hover {
  background-color: #f5f7fa;
  border-color: #409EFF;
  color: #409EFF;
}

.icon-name {
  font-size: 12px;
  word-break: break-all;
  text-align: center;
}

.icon-item .el-icon {
  font-size: 24px;
}
</style> 