import request from '../utils/request'
import type { LoginParams, LoginResult } from '../types/api'

export function login(data: LoginParams) {
  return request<LoginResult>({
    url: '/Auth/login',
    method: 'post',
    data
  })
} 