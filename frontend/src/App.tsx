// import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import LoginPage from "./pages/LoginPage";
import RegistrationPage from "./pages/RegistrationPage";

import { usePage } from "./PageContext";
import ProfilePage from "./pages/ProfilePage";
import LobbyPage from "./pages/game/LobbyPage";
import GameRoundPage from "./pages/game/GameRoundPage";
import GameLeaderboard from "./pages/game/GameLeaderboard";
import GameLeaderboardFinal from "./pages/game/GameLeaderboardFinal";
import { useEffect, useState } from "react";
import { gameHubService } from "./apiUtils/HubServices";
import type { GameDto, RoomDto } from "./apiUtils/dto";
import { in_room } from "./utils/RoomHubUtils";

export enum LoggingStatus {
    NotLogged,
    Logged,
    Guest
}

export interface CurrentAppState {
    loggingStatus: LoggingStatus,
    setLoggingStatus: (x: any) => void,

    user_id: string | null,

    room: RoomDto | null,
    setRoom: (x: any) => void | null,

    game: GameDto | null,
    setGame: (x: any) => void | null,

    page: any,
    setPage: (x: any) => void | null

};

export default function App() {
    const { page, setPage } = usePage();
    const [loggingStatus, setLoggingStatus] = useState(LoggingStatus.NotLogged);
    const [room, setRoom] = useState(null);
    const [game, setGame] = useState(null);


    useEffect(()  => {
        gameHubService.connect().catch(err => {
            console.error("SignalR error", err);
            alert("Не удалось подключиться к SignalR");
        });
    }, []);

   
    
    const user_id = localStorage.getItem("user_id");
    // Service Locator think how to kill him 
    const props: CurrentAppState = { loggingStatus, setLoggingStatus, user_id, room, setRoom, game, setGame, page, setPage };
    console.log(props);
    
    useEffect(() => {
        if (!room) return;

        const roomN = in_room(props);

        return () => {
            roomN();
        };
    }, [props.room]);

    useEffect(() => {
        if (!game) return;

        const offQuestion = gameHubService.onNewQuestion(data => {
            ;
        });

        return () => {
            offQuestion();
        };
    }, [props.game]);

    return (
        <div>
            {page === "home" && <Home {...props} />}
            {page === "login" && <LoginPage {...props} />}
            {page === "registration" && <RegistrationPage {...props} />}
            {page === "profile" && <ProfilePage />}
            {page === "room" && <LobbyPage {...props} />}
            {page === "game_round" && <GameRoundPage {...props} />}
            {page === "game_leaderboard" && <GameLeaderboard />}
            {page === "game_leaderboard_final" && <GameLeaderboardFinal />}
        </div>
    );
}

