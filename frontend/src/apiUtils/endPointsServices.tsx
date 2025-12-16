import http from '../api/http';
import type {
    CreateGuestRequest, 
    LoginRequest, 
    LoginResponse, 
    OperationResult, 
    RegisterRequest,
    RoomDto, 
    RoomPrivacyResponse, 
    UpdateUserRequest, 
    UserDto } from "./dto";



class ApiService {
    // === Пользователи ===
    async register(data: RegisterRequest): Promise<OperationResult<UserDto>> {
        const formData = new FormData();
        formData.append('login', data.login);
        formData.append('password', data.password);
        formData.append('playerName', data.playerName);
        formData.append('avatar', data.avatar);

        try {
            const response = await http.post<UserDto>('/api/register', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });

            return {
                success: true,
                resultObj: response
            };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка регистрации'
            };
        }
    }
    
    async login(data: LoginRequest): Promise<OperationResult<LoginResponse>> {
        try {
            const response = await http.post<LoginResponse>('/api/login', data);
            return { success: true, resultObj: response };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка авторизации'
            };
        }
    }

    async updateUser(data: UpdateUserRequest): Promise<OperationResult<UserDto>> {
        const formData = new FormData();
        formData.append('userId', data.userId);
        formData.append('playerName', data.playerName);
        formData.append('avatar', data.avatar);

        try {
            const response = await http.post<UserDto>(`/api/${data.userId}/userUpdate`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
            return { success: true, resultObj: response };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка обновления пользователя'
            };
        }
    }

    

    async createGuest(data: CreateGuestRequest): Promise<OperationResult<UserDto>> {
        const formData = new FormData();
        formData.append('playerName', data.playerName);
        formData.append('avatar', data.avatar);

        try {
            const response = await http.post<UserDto>('/api/guest', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
            return { success: true, resultObj: response };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка создания гостя'
            };
        }
    }

    async getRoomPrivacy(inviteCode: string): Promise<OperationResult<RoomPrivacyResponse>> {
        try {
            const response = await http.get<RoomPrivacyResponse>(`/api/rooms/${inviteCode}/privacy`);
            return { success: true, resultObj: response };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка получения информации о комнате'
            };
        }
    }
}

export const apiService = new ApiService();