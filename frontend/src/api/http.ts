import axios, { type AxiosInstance, type  AxiosRequestConfig, type InternalAxiosRequestConfig, type AxiosResponse } from "axios";

const api: AxiosInstance = axios.create({
baseURL: "http://localhost:5209", // поменяй на свой URL API
timeout: 10000,
});


// Перехватчик запросов — можно добавлять токены
api.interceptors.request.use(
    (config) => {
// Пример: добавить Authorization
    const token = localStorage.getItem("token");
    if (token && config.headers) config.headers.Authorization = `Bearer  ${token}`;

    return config;
    },
(error) => Promise.reject(error)
);


// Перехватчик ответов — можно ловить 401, логировать ошибки
api.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error) => {
    console.error("API Error:", error.response || error);
    return Promise.reject(error);
}
);


// Универсальные методы
export const http = {
get: async <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
const response = await api.get<T>(url, config);
return response.data;
},


post: async <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
const response = await api.post<T>(url, data, config);
return response.data;
},


put: async <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
const response = await api.put<T>(url, data, config);
return response.data;
},


delete: async <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
const response = await api.delete<T>(url, config);
return response.data;
},
};


export default http;