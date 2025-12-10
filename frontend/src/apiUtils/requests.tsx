import type {AnswerDto, GameMode, QuestionDto, RoomPrivacy } from "./dto";

export interface CreateRoomRequest {
    userId: string;
    privacy: RoomPrivacy;
    password?: string;
    maxPlayers: number;
}

export interface JoinRoomRequest {
    userId: string;
    roomId: string;
    password?: string;
}

export interface LeaveRoomRequest {
    userId: string;
    roomId: string;
}

export interface FindQuickRoomRequest {
    userId: string;
}

export interface KickPlayerRequest {
    userId: string;
    roomId: string;
    removedPlayerId: string;
}

export interface CreateGameRequest {
    userId: string;
    roomId: string;
    mode: GameMode;
    countQuestions: number;
    questionDuration: number;
    questions?: QuestionDto[]; // Уточнить тип
}

export interface StartGameRequest {
    userId: string;
    roomId: string;
}

export interface SubmitAnswerRequest {
    gameId: string;
    questionId: string;
    playerId: string;
    answer: AnswerDto;
}

export interface FinishGameRequest {
    userId: string;
    roomId: string;
}