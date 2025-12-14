export interface OperationResult<T = any> {
    success: boolean;
    resultObj?: T;
    message?: string;
}

export interface UserDto {
    id: string;
    playerName: string;
    avatarUrl: string;
}

export interface LoginResponse {
    token: string;
    id: string;
    playerName: string;
    avatarUrl: string;
}

export interface RegisterRequest {
    login: string;
    password: string;
    playerName: string;
    avatar: File;
}

export interface RegisterResponse {
    user: UserDto;
}

export interface LoginRequest {
    login: string;
    password: string;
}

export interface UpdateUserRequest {
    userId: string;
    playerName: string;
    avatar: File;
}

export interface CreateGuestRequest {
    playerName: string;
    avatar: File;
}

export enum RoomPrivacy {
    Public = "public",
    Private = "private"
}

export interface PlayerDto {
    id: string;
    playerName: string;
    avatarUrl: string;
}

export interface RoomDto {
    id: string;
    privacy: RoomPrivacy;
    maxPlayers: number;
    players: PlayerDto[];
    ownerId: string;
    closedAt: string;
}

export interface RoomPrivacyResponse {
    roomPrivacy: RoomPrivacy;
}

export enum GameMode {
    Default = "Default",
    BoolMode = "BoolMode",
}

export interface GameDto {
    id: string;
    mode: GameMode;
    questions: QuestionDto[];
    questionsCount: number;
}

export interface QuestionDto {
    formulation: string;
    imageUrl: string;
    mode: GameMode;
}

export interface StatisticDto {
    scores: { [playerId: string]: number };
}

export interface AnswerDto {
    value: any
}
