import axios from "axios";
import type { AxiosInstance, AxiosResponse } from "axios";
import { ElMessage } from "element-plus";
import type { ApiResponse } from "../types/api";

// 创建 axios 实例
const service: AxiosInstance = axios.create({
  baseURL: "/api",
  timeout: 15000,
  headers: {
    "Content-Type": "application/json;charset=utf-8",
  },
});

// 请求拦截器
service.interceptors.request.use(
  (config) => {
    console.log("Request:", {
      url: config.url,
      method: config.method,
      data: config.data,
    });

    const token = localStorage.getItem("token");
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
service.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    console.log("Response:", response.data);

    const res = response.data;
    // 检查响应状态
    if (res.Code === undefined) {
      // 如果响应中没有 Code 字段，直接返回数据
      return response.data;
    }

    if (res.Code !== 200) {
      ElMessage({
        message: res.Msg || "请求错误",
        type: "error",
        duration: 5 * 1000,
      });
      return Promise.reject(new Error(res.Msg || "请求错误"));
    }

    return res.Data;
  },
  (error) => {
    console.error("Response Error:", {
      status: error.response?.status,
      data: error.response?.data,
      config: error.config,
    });

    // 处理特定的错误状态码
    if (error.response?.status === 401) {
      // 未授权，清除 token 并跳转到登录页
      localStorage.removeItem("token");
      window.location.href = "/login";
      return Promise.reject(new Error("请重新登录"));
    }

    ElMessage({
      message: error.response?.data?.Msg || error.message || "网络错误",
      type: "error",
      duration: 5 * 1000,
    });
    return Promise.reject(error);
  }
);

export default service;
