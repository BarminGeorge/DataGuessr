import type {AnswerDto, GameDto, PlayerDto, RoomDto, StatisticDto } from "./dto";

export interface GameNotification {
    methodName: string;
    data: any;
}

export interface NewPlayerNotification {
    player: PlayerDto;
}

export interface PlayerLeavedNotification {
    playerId: string;
    ownerId: string;
}

export interface NewGameNotification {
    game: GameDto;
}

export interface ReturnToRoomNotification {
    room: RoomDto;
}

export interface NewQuestionNotification {
    questionId: string;
    formulation: string;
    imageUrl: string;
    endTime: string;
    durationSeconds: number;
}

export interface QuestionClosedNotification {
    questionId: string;
    correctAnswer: AnswerDto;
}

export interface StatisticNotification {
    statistic: StatisticDto;
}