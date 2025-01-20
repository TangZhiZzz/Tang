import request from '../utils/request'
import type { SysUser } from '../types/api'

// 获取用户列表
export function getUserList(params?: { keyword?: string }) {
  return request<SysUser[]>({
    url: '/User/list',
    method: 'get',
    params
  })
}

// 获取用户信息
export function getUserInfo(id: number) {
  return request<SysUser>({
    url: `/User/${id}`,
    method: 'get'
  })
}

// 添加用户
export function addUser(data: Partial<SysUser>) {
  return request({
    url: '/User',
    method: 'post',
    data
  })
}

// 修改用户
export function updateUser(data: Partial<SysUser>) {
  return request({
    url: '/User',
    method: 'put',
    data
  })
}

// 删除用户
export function deleteUser(id: number) {
  return request({
    url: `/User/${id}`,
    method: 'delete'
  })
}

// 获取用户角色
export function getUserRoles(userId: number) {
  return request<number[]>({
    url: `/User/${userId}/roles`,
    method: 'get'
  })
}

// 设置用户角色
export function setUserRoles(userId: number, roleIds: number[]) {
  return request({
    url: `/User/${userId}/roles`,
    method: 'post',
    data: roleIds
  })
} 