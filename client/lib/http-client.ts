import axios from "axios";

export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:8000";

export const httpClient = axios.create({
  baseURL: API_BASE_URL,
//   withCredentials: true,
  timeout: 10_000,
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
});

httpClient.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem("swift-ride-token");
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

httpClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response) {
      console.error("Lỗi phản hồi từ server:", error.response.status, error.response.data);
    } else if (error.request) {
      console.error("Không nhận được phản hồi từ server:", error.request);
    } else {
      console.error("Lỗi khi thiết lập yêu cầu:", error.message);
    }
    return Promise.reject(error);
  }
);  
