import React from "react";
import Header from "../components/Header";

import { usePage } from "../PageContext";
import { http } from "../api/http";
import { useState } from "react";
import { validateLogin, validatePassword } from "../utils/validations";
import { apiService } from "../apiUtils/endPointsServices";
import { LoggingStatus } from "../App";


async function handleLogin(
    login: string,
    password: string,
    setLoggingStatus: (status: any) => void,
    setPage: (page: any) => void) {

    const data = { login, password };
    const result = await apiService.login(data);

    if (!result.success) {
        console.error(result.message);
        return;
    }
    setLoggingStatus(LoggingStatus.Logged);
    console.log(result);
    if (result.resultObj) {
        localStorage.setItem("user_id", result.resultObj?.id);
        localStorage.setItem("player_name", result.resultObj?.playerName);
        localStorage.setItem("avatar_url", result.resultObj?.avatarUrl);
    }
    setPage("home");
}




export default function LoginPage(props: any) {
    const { setPage } = usePage();
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");

    props = props.props;
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
                    onClick={() => handleLogin(login, password, props.setLoggingStatus, setPage)}>
                    Войти
                </button>

            </div>
        </div>
    );
}