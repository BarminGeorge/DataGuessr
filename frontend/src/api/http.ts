import axios, { type AxiosInstance, type  AxiosRequestConfig, type InternalAxiosRequestConfig, type AxiosResponse } from "axios";

export const api: AxiosInstance = axios.create({
baseURL: "https://fiitguesser.ru/api", // поменяй на свой URL API
timeout: 10000,
});


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