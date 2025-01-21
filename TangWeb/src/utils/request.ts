import axios from "axios";
import type { AxiosInstance } from "axios";
import { ElMessage } from "element-plus";

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
  (response) => {
    console.log("Response:", response);

    const res = response.data;
    if (res.code === undefined) {
      return Promise.resolve(response.data);
    }

    if (res.code !== 200) {
      ElMessage({
        message: res.message || "请求错误",
        type: "error",
        duration: 5 * 1000,
      });
      return Promise.reject(new Error(res.message || "请求错误"));
    }
    return Promise.resolve(res);
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
      message: error.response?.data?.message || error.message || "网络错误",
      type: "error",
      duration: 5 * 1000,
    });
    return Promise.reject(error);
  }
);

export default service;
