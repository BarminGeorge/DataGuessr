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
import { useState } from "react";

export enum LoggingStatus {
    NotLogged,
    Logged,
    Guest
}

export default function App() {
    const { page, setPage } = usePage();
    const [loggingStatus, setLoggingStatus] = useState(LoggingStatus.NotLogged);

    const props = { loggingStatus, setLoggingStatus };
    
    
    return (
        <div>
            {page === "home" && <Home props={props} />}
            {page === "login" && <LoginPage props={props} />}
            {page === "registration" && <RegistrationPage props={props} />}
            {page === "profile" && <ProfilePage />}
            {page === "room" && <LobbyPage />}
            {page === "game_round" && <GameRoundPage />}
            {page === "game_leaderboard" && <GameLeaderboard />}
            {page === "game_leaderboard_final" && <GameLeaderboardFinal />}
        </div>
    );
}

