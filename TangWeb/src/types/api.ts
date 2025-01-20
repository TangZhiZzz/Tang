// API 响应的通用格式
export interface ApiResponse<T = any> {
  Code: number
  Msg: string
  Data: T
}

// 登录相关
export interface LoginParams {
  userName: string
  password: string
}

export interface LoginResult {
  token: string
  // 可能还有其他登录返回信息
}

// 用户相关
export interface SysUser {
  id: number
  createTime: string
  updateTime?: string
  isDeleted: boolean
  userName: string
  password?: string
  nickName?: string
  avatar?: string
  status: number // 0:禁用,1:启用
  remark?: string
}

// 角色相关
export interface SysRole {
  id: number
  createTime: string
  updateTime?: string
  isDeleted: boolean
  roleName: string
  roleCode: string
  status: number // 0:禁用,1:启用
  remark?: string
}

// 权限相关
export interface SysPermission {
  id: number
  createTime: string
  updateTime?: string
  isDeleted: boolean
  parentId: number
  name: string
  type: number // 1:菜单,2:按钮
  permissionCode: string
  path?: string
  component?: string
  icon?: string
  sort: number
  status: number // 0:禁用,1:启用
  children?: SysPermission[]
}

// 用户相关接口的类型定义
export interface UserInfo {
  id: number
  username: string
  // 根据实际 swagger 文档补充其他字段
} 