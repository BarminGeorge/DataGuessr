import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import type {
    CreateGameRequest, CreateRoomRequest, FindQuickRoomRequest,
    FinishGameRequest, JoinRoomRequest, KickPlayerRequest, LeaveRoomRequest,
    StartGameRequest,
    SubmitAnswerRequest
} from "./requests";
import type { GameDto, OperationResult, RoomDto } from "./dto";
import type {
    NewGameNotification, NewPlayerNotification,
    NewQuestionNotification, PlayerLeavedNotification, QuestionClosedNotification, ReturnToRoomNotification,
    StatisticNotification
} from "./notifications";


type NotificationCallback<T = any> = (data: T) => void;

class GameHubService {
    private connection: HubConnection | null = null;
    private isConnected = false;
    private notificationCallbacks = new Map<string, Set<Function>>();

    // === Подключение ===
    async connect(token?: string): Promise<boolean> {
        if (this.connection && this.connection.state === HubConnectionState.Connected) {
            return true;
        }

        try {
            this.connection = new HubConnectionBuilder()
                .withUrl('https://fiitguesser.ru/api/appHub', {
                    accessTokenFactory: () => token || '',
                    withCredentials: true
                })
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: (retryContext) => {
                        return Math.min(retryContext.previousRetryCount * 1000, 10000);
                    }
                })
                .build();

            // Настраиваем обработчики уведомлений
            this.setupNotificationHandlers();

            await this.connection.start();
            this.isConnected = true;
            console.log('SignalR подключен');
            return true;
        } catch (error) {
            console.error('Ошибка подключения SignalR:', error);
            this.connection = null;
            this.isConnected = false;
            throw error;
        }
    }

    async disconnect(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            this.connection = null;
            this.isConnected = false;
            this.notificationCallbacks.clear();
        }
    }

    getConnectionState(): HubConnectionState | null {
        return this.connection?.state || null;
    }

    // === Методы хаба ===
    // Комнаты
    async createRoom(request: CreateRoomRequest): Promise<OperationResult<RoomDto>> {
        return this.invoke<RoomDto>('CreateRoom', request);
    }

    async joinRoom(request: JoinRoomRequest): Promise<OperationResult<RoomDto>> {
        return this.invoke<RoomDto>('JoinRoom', request);
    }

    async leaveRoom(request: LeaveRoomRequest): Promise<OperationResult> {
        return this.invoke('LeaveRoom', request);
    }

    async findQuickRoom(request: FindQuickRoomRequest): Promise<OperationResult<RoomDto>> {
        return this.invoke<RoomDto>('FindQuickRoom', request);
    }

    async kickPlayer(request: KickPlayerRequest): Promise<OperationResult> {
        return this.invoke('KickPlayerFromRoom', request);
    }

    async createGame(request: CreateGameRequest): Promise<OperationResult<GameDto>> {
        return this.invoke<GameDto>('CreateGame', request);
    }

    async startGame(request: StartGameRequest): Promise<OperationResult> {
        return this.invoke('StartGame', request);
    }

    async submitAnswer(request: SubmitAnswerRequest): Promise<OperationResult> {
        return this.invoke('SubmitAnswer', request);
    }

    async finishGame(request: FinishGameRequest): Promise<OperationResult<RoomDto>> {
        return this.invoke<RoomDto>('FinishGame', request);
    }

    // === Подписка на уведомления ===
    onNewPlayer(callback: NotificationCallback<NewPlayerNotification>): () => void {
        return this.subscribe('NewPlayerEntered', callback);
    }

    onPlayerLeaved(callback: NotificationCallback<PlayerLeavedNotification>): () => void {
        return this.subscribe('PlayerLeaved', callback);
    }

    onNewGame(callback: NotificationCallback<NewGameNotification>): () => void {
        return this.subscribe('NewGameAdded', callback);
    }

    onReturnToRoom(callback: NotificationCallback<ReturnToRoomNotification>): () => void {
        return this.subscribe('ReturnToRoom', callback);
    }

    onNewQuestion(callback: NotificationCallback<NewQuestionNotification>): () => void {
        return this.subscribe('QuestionWasAsked', callback);
    }

    onQuestionClosed(callback: NotificationCallback<QuestionClosedNotification>): () => void {
        return this.subscribe('QuestionClosed', callback);
    }

    onShowLeaderBoard(callback: NotificationCallback<StatisticNotification>): () => void {
        return this.subscribe('ShowLeaderBoard', callback);
    }

    // === Вспомогательные методы ===
    private async invoke<T>(methodName: string, ...args: any[]): Promise<OperationResult<T>> {
        if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
            throw new Error('SignalR соединение не установлено');
        }

        try {
            const result = await this.connection.invoke<T>(methodName, ...args);
            return result;
        } catch (error: any) {
            return {
                success: false,
                message: error.message || `Ошибка вызова ${methodName}`
            };
        }
    }

    private setupNotificationHandlers(): void {
        if (!this.connection) return;

        // Регистрируем обработчики для каждого типа уведомлений
        const handlers = [
            { method: 'NewPlayerEntered', key: 'NewPlayerEntered' },
            { method: 'PlayerLeaved', key: 'PlayerLeaved' },
            { method: 'NewGameAdded', key: 'NewGameAdded' },
            { method: 'ReturnToRoom', key: 'ReturnToRoom' },
            { method: 'QuestionWasAsked', key: 'QuestionWasAsked' },
            { method: 'QuestionClosed', key: 'QuestionClosed' },
            { method: 'ShowLeaderBoard', key: 'ShowLeaderBoard' }
        ];

        handlers.forEach(({ method, key }) => {
            this.connection?.on(method, (data: any) => {
                this.notifySubscribers(key, data);
            });
        });
    }

    private subscribe(eventName: string, callback: Function): () => void {
        if (!this.notificationCallbacks.has(eventName)) {
            this.notificationCallbacks.set(eventName, new Set());
        }

        this.notificationCallbacks.get(eventName)!.add(callback);
        console.log(eventName);
        // Возвращаем функцию для отписки
        return () => {
            const callbacks = this.notificationCallbacks.get(eventName);
            if (callbacks) {
                callbacks.delete(callback);
                if (callbacks.size === 0) {
                    this.notificationCallbacks.delete(eventName);
                }
            }
        };
    }

    private notifySubscribers(eventName: string, data: any): void {
        const callbacks = this.notificationCallbacks.get(eventName);
        if (callbacks) {
            callbacks.forEach(callback => {
                try {
                    callback(data);
                } catch (error) {
                    console.error(`Ошибка в обработчике ${eventName}:`, error);
                }
            });
        }
    }

    // Утилиты для управления группами (если нужно напрямую)
    async joinGroup(groupName: string): Promise<void> {
        if (this.connection) {
            await this.connection.invoke('AddToGroup', groupName);
        }
    }

    async leaveGroup(groupName: string): Promise<void> {
        if (this.connection) {
            await this.connection.invoke('RemoveFromGroup', groupName);
        }
    }
}

export const gameHubService = new GameHubService();