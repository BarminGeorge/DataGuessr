import React from "react";
import Header from "../components/Header";

import { usePage } from "../PageContext";
import { http } from "../api/http";
import { useState } from "react";
import { validateLogin, validatePassword } from "../utils/validations";
import { apiService } from "../apiUtils/endPointsServices";
import { LoggingStatus, type CurrentAppState } from "../App";
import type { UserDto } from "../apiUtils/dto";


async function handleLogin(
    login: string,
    password: string,
    props: CurrentAppState) {

    const data = { login, password };
    const result = await apiService.login(data);

    if (!result.success) {
        console.error(result.message);
        return;
    }
    props.setLoggingStatus(LoggingStatus.Logged);
    console.log(result);

    const user: UserDto = { id: result.resultObj?.id, avatarUrl: result.resultObj?.avatarUrl, playerName: result.resultObj?.playerName }
    props.setUser(user);
    props.setPage("home");
}




export default function LoginPage(props: CurrentAppState) {
    const { setPage } = usePage();
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");

    return (
        <div className="global-container">
            <Header />
            <div className="main-container">
                <div className="title-text">
                    Вход
                </div>

                <div className="secondary-container">
                    <input
                        type="text"
                        className="text-input-primary"
                        placeholder={"Введите логин"}
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
                        placeholder={"Введите пароль"}
                        onChange={(e) => setPassword(e.target.value)} />
                    <span className="accent-text">
                        {validatePassword(password)}
                    </span>

                </div>
                <div className="secondary-text">Нет аккаунта?

                    <span
                        className="link-text"
                        onClick={() => setPage("registration")}>
                        Зарегистрироваться
                    </span>

                </div>

                <button className="button-primary"
                    onClick={() => handleLogin(login, password, props )}>
                    Войти
                </button>

            </div>
        </div>
    );
}