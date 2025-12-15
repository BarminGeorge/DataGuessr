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
import type { RoomDto } from "./apiUtils/dto";

export enum LoggingStatus {
    NotLogged,
    Logged,
    Guest
}

export interface CurrentAppState {
    loggingStatus: LoggingStatus,
    setLoggingStatus: (x: any) => void,

    user_id: string| null,
    room: RoomDto | null,
    setRoom: (x: any) => void | null

};

export default function App() {
    const { page, setPage } = usePage();
    const [loggingStatus, setLoggingStatus] = useState(LoggingStatus.NotLogged);
    const [room, setRoom] = useState(null);


    useEffect(() => {
        gameHubService.connect().catch(err => {
            console.error("SignalR error", err);
            alert("Не удалось подключиться к серверу");
        });
    }, []);

    const user_id = localStorage.getItem("user_id");
    const props: CurrentAppState = { loggingStatus, setLoggingStatus, user_id, room, setRoom };
    console.log(props);


    return (
        <div>
            {page === "home" && <Home {...props} />}
            {page === "login" && <LoginPage {...props} />}
            {page === "registration" && <RegistrationPage {...props} />}
            {page === "profile" && <ProfilePage />}
            {page === "room" && <LobbyPage {...props} />}
            {page === "game_round" && <GameRoundPage />}
            {page === "game_leaderboard" && <GameLeaderboard />}
            {page === "game_leaderboard_final" && <GameLeaderboardFinal />}
        </div>
    );
}

