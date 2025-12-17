import React from "react";
import Header from "../components/Header";
import { api, http } from "../api/http";

import { useState } from "react";
import { usePage } from "../PageContext";
import { validateLogin, validatePassword, validateUsername } from "../utils/validations";
import { apiService } from "../apiUtils/endPointsServices";
import type { CurrentAppState } from "../App";
import type { UserDto } from "../apiUtils/dto";

async function handleRegistration(
    login: string,
    password: string,
    playerName: string,
    avatar: any,
    props: CurrentAppState) {

    const data = { login, password, playerName, avatar };
    const result = await apiService.register(data);

    if (!result.success) {
        console.error(result.message);
        return;
    }

    const user: UserDto = { id: result.resultObj?.id, avatarUrl: result.resultObj?.avatarUrl, playerName: result.resultObj?.playerName }
    props.setUser(user);
    props.setLoggingStatus(1);
    props.setPage("home");
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
                    <label className="avatar-input-primary">
                        <input
                            type="file"
                            accept="image/*"
                            className="avatar-input-hidden"
                            onChange={(e) => {
                                const file = e.target.files?.[0] ?? null;
                                setAvatar(file);
                            }}
                        />

                        <div className="avatar-preview">
                            {avatar ? (
                                <img
                                    src={URL.createObjectURL(avatar)}
                                    alt="avatar preview"
                                />
                            ) : (
                                <span>?</span>
                            )}
                        </div>

                        <div className="avatar-text">
                            {avatar ? avatar.name : "Выберите аватар"}
                        </div>
                    </label>

                </div>

                <button className="button-primary"
                    onClick={() => handleRegistration(login, password, playerName, avatar, props)}>Зарегистрироваться</button>

            </div>
        </div>
    );
}



