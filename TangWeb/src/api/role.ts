import request from "../utils/request";
import type { SysPermission, SysRole } from "../types/api";

// 获取角色列表
export function getRoleList(params?: { keyword?: string }) {
  return request<SysRole[]>({
    url: "/Role/list",
    method: "get",
    params,
  });
}

// 获取角色信息
export function getRoleInfo(id: number) {
  return request<SysRole>({
    url: `/Role/${id}`,
    method: "get",
  });
}

// 添加角色
export function addRole(data: Partial<SysRole>) {
  return request({
    url: "/Role",
    method: "post",
    data,
  });
}

// 修改角色
export function updateRole(data: Partial<SysRole>) {
  return request({
    url: "/Role",
    method: "put",
    data,
  });
}

// 删除角色
export function deleteRole(id: number) {
  return request({
    url: `/Role/${id}`,
    method: "delete",
  });
}

// 获取角色权限
export function getRolePermissions(roleId: number) {
  return request<SysPermission[]>({
    url: `/Role/${roleId}/permissions`,
    method: "get",
  });
}

// 设置角色权限
export function setRolePermissions(roleId: number, permissionIds: number[]) {
  return request({
    url: `/Role/${roleId}/permissions`,
    method: "post",
    data: permissionIds,
  });
}
