import request from '../utils/request'
import type { SysPermission } from '../types/api'

// 获取权限树
export function getPermissionTree(params?: { keyword?: string }) {
  return request<SysPermission[]>({
    url: '/Permission/tree',
    method: 'get',
    params
  })
}

// 获取权限信息
export function getPermissionInfo(id: number) {
  return request<SysPermission>({
    url: `/Permission/${id}`,
    method: 'get'
  })
}

// 添加权限
export function addPermission(data: Partial<SysPermission>) {
  return request({
    url: '/Permission',
    method: 'post',
    data
  })
}

// 修改权限
export function updatePermission(data: Partial<SysPermission>) {
  return request({
    url: '/Permission',
    method: 'put',
    data
  })
}

// 删除权限
export function deletePermission(id: number) {
  return request({
    url: `/Permission/${id}`,
    method: 'delete'
  })
} 