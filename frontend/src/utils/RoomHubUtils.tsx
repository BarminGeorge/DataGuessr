import { type RoomDto, RoomPrivacy, GameMode } from "../apiUtils/dto";
import { gameHubService } from "../apiUtils/HubServices";
import type { NewGameNotification } from "../apiUtils/notifications";
import type { CurrentAppState } from "../App";
import { usePage } from "../PageContext";

export function in_room(props: CurrentAppState) {
    const onNewPlayer = (data: any) => {
        console.log(data);
        const player = data.player;

        props.setRoom(prevRoom => {
            if (!prevRoom) return prevRoom;
            return {
                ...prevRoom,
                players: [...prevRoom.players, player],
            };
        });
    };

    const onPlayerLeaved = (data: any) => {
        console.log(data);

        props.setRoom(prevRoom => {
            if (!prevRoom) return prevRoom;
            return {
                ...prevRoom,
                ownerId: data.ownerId,
                players: prevRoom.players.filter(p => p.id !== data.playerId),
            };
        });
    };

    const onNewGame = (data: NewGameNotification) => {
        console.log(data);
        props.setGame(data.game);
        props.setPage("game_round");

    }

    const offNewPlayer = gameHubService.onNewPlayer(onNewPlayer);
    const offPlayerLeaved = gameHubService.onPlayerLeaved(onPlayerLeaved);
    const offNewGame = gameHubService.onNewGame(onNewGame);


    gameHubService.onReturnToRoom(data => {
        console.log(data);
    });
    gameHubService.onShowLeaderBoard(data => {
        console.log(data);
    });

    return () => {
        offNewPlayer();
        offPlayerLeaved();
        offNewGame();
    };
}




//}


export async function findRandomRoom(
    userId: string,
    setPage: (page: any) => void,
    room: RoomDto,
    setRoom: (x: any) => void
) {
    const result = await gameHubService.findQuickRoom({
        userId
    });

    if (!result.success || !result.resultObj) {
        console.log(result);
        alert("Не удалось найти комнату");
        return;
    }

    setRoom(result.resultObj);
    setPage("room");
}

export async function joinRoomByCode(
    userId: string,
    roomId: string,
    setPage: (page: any) => void,
    room: RoomDto,
    setRoom: (x: any) => void
) {
    if (!roomId) {
        alert("Введите код комнаты");
        return;
    }

    const result = await gameHubService.joinRoom({
        userId,
        roomId
    });

    if (!result.success || !result.resultObj) {
        console.log(result);
        alert("Не удалось найти комнату");
        return;
    }

    setRoom(result.resultObj);
    setPage("room");
}

export async function createRoom(
    userId: string | null,
    setPage: (page: any) => void,
    room: RoomDto,
    setRoom: (x: any) => void) {

    if (userId == null)
        return;

    const result = await gameHubService.createRoom({
        userId: userId,
        privacy: RoomPrivacy.Public,
        maxPlayers: 8
    });
    console.log(result);

    if (!result.success || !result.resultObj) {
        alert("Не удалось создать комнату");
        return;
    }
    setRoom(result.resultObj);
    setPage("room");
}

export async function createGame(
    userId: string | null,
    roomId: string | undefined,
    setPage: (page: any) => void) {


    if (userId == null || !roomId) {
        alert("Не удалось начать игру");
        return;
    }
    const result = await gameHubService.createGame({
        userId: userId,
        roomId,
        mode: GameMode.Default,
        countQuestions: 5,
        questionDuration: 30
    });

    console.log(result);

    if (!result.success || !result.resultObj) {
        alert("Не удалось начать игру");
        return;
    }
    await startGame(userId, roomId);
    setPage("game_round");
}

export async function startGame(
    userId: string | null,
    roomId: string | undefined,
) {

    if (userId == null || !roomId) {
        alert("Не удалось начать игру");
        return;
    }
    const result = await gameHubService.startGame({
        userId: userId,
        roomId,
    });

    console.log(result);

    if (!result.success) {
        alert("Не удалось начать игру");
        return;
    }

}

export async function leaveRoom(
    userId: string | null,
    roomId: string | undefined,
    setPage: (page: any) => void,
    setRoom: (x: any) => void) {


    if (userId == null || !roomId) {
        alert("Не удалось покинуть комнату");
        return;
    }
    const result = await gameHubService.leaveRoom({
        userId: userId,
        roomId,
    });
    console.log(result);

    if (!result.success) {
        alert("Не удалось покинуть комнату");
        return;
    }
    setRoom(null);
    setPage("home");
}