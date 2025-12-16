import React from "react";
import Header from "../components/Header";
import { api, http } from "../api/http";

import { useState } from "react";
import { usePage } from "../PageContext";
import { validateLogin, validatePassword, validateUsername } from "../utils/validations";
import { apiService } from "../apiUtils/endPointsServices";
import type { CurrentAppState } from "../App";

async function handleRegistration(
    login: string,
    password: string,
    playerName: string,
    avatar: any,
    setLoggingStatus: (status: any) => void,
    setPage: (page: any) => void) {

    const data = { login, password, playerName, avatar };
    const result = await apiService.register(data);

    if (!result.success) {
        console.error(result.message);
        return;
    }
    if (result.resultObj) {
        localStorage.setItem("user_id", result.resultObj?.id);
        localStorage.setItem("player_name", result.resultObj?.playerName);
        localStorage.setItem("avatar_url", result.resultObj?.avatarUrl);
    }
    setLoggingStatus(1);
    setPage("home");
}

export default function RegistrationPage(props: CurrentAppState) {
    const { setPage } = usePage();
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [playerName, setPlayerName] = useState("");
    const [avatar, setAvatar] = useState<File | null>(null);

    return (
        <div className="global-container">
            <Header />
            <div className="main-container">
                <div className="title-text">
                    Регистрация
                </div>


                <div className="secondary-container">
                    <input
                        type="text"
                        className="text-input-primary"
                        placeholder={"Придумайте логин"}
                        onChange={(e) => setLogin(e.target.value)}
                    />
                    <span className="accent-text">
                        {validateLogin(login)}
                    </span>
                </div>
                <div className="secondary-container">
                    <input
                        type="text"
                        className="text-input-primary"
                        placeholder={"Придумайте пароль"}
                        onChange={(e) => setPassword(e.target.value)} />
                    <span className="accent-text">
                        {validatePassword(password)}
                    </span>
                </div>
                <div className="secondary-container">
                    <input
                        type="text"
                        className="text-input-primary"
                        placeholder={"Придумайте имя пользователя"}
                        onChange={(e) => setPlayerName(e.target.value)} />
                    <span className="accent-text">
                        {validateUsername(playerName)}
                    </span>
                </div>
                <div className="secondary-container">
                <input
                    type="file"
                    className="text-input-primary"
                    placeholder={"Выберите аватар"}
                    onChange={(e) => setAvatar(e.target.files && e.target.files[0] ? e.target.files[0] : null)} />
                </div>

                <button className="button-primary"
                    onClick={() => handleRegistration(login, password, playerName, avatar, props.setLoggingStatus, setPage)}>Зарегистрироваться</button>

            </div>
        </div>
    );
}



