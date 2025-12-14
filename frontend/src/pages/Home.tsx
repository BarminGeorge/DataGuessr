import React, { useState } from "react";
import Header from "../components/Header";
import TextInput from "../components/TextInputs";
import http from "../api/http";
import { usePage } from "../PageContext";
import EnterModal from "../modals/EnterModal";
import { LoggingStatus } from "../App";

async function RandomRoom(
    setPage: (page: any) => void) {

    try {
        const res: any = await http.get(
            "/rooms");

        setPage("room");
    } catch (e) {
        alert(e);
        setPage("room");
    }
}

function checkLogging(loggingStatus: any) {
    if (loggingStatus == LoggingStatus.Guest) {
        return true;
    }
    if (loggingStatus == LoggingStatus.Logged) {
        return true;
    }
    return false;
}

export default function HomePage(props: any) {
    const { setPage } = usePage();
    props = props.props;
    
    return (
        <div className="global-container">

            {/* <button className="button-primary" onClick={() => setPage("game")}>В Игре</button>;   */}
            <div className={`modal centered-vertical-aligment ${checkLogging(props.loggingStatus) ? 'hide' : ''}`}>
                <EnterModal setLoggingStatus={props.setLoggingStatus} />
            </div>
            <div className={checkLogging(props.loggingStatus) ? "" : "blur"}>


                <Header variant={checkLogging(props.loggingStatus) ? "logo-and-avatar-and-create-room" : "logo-and-login-button"} />
                <div className="main-container">
                    <div className="secondary-container">
                        <div className="left-aligment title-variant-1">Время</div>
                        <div className="centered-aligment title-variant-2">
                            Угадывать
                        </div>
                        <div className="right-aligment  title-variant-3">Время</div>
                    </div>

                    <div className="right-aligment">
                        <p className="secondary-text max-w-xl">
                            Это мега крутая игра где тебе нужно по фотографии угадать время когда
                            она сделана, на всё про всё несколько минут, угадал лучше всех —
                            победил!
                        </p>
                    </div>

                    {/* join game buttons */}
                    <div className="secondary-container">
                        <div className="right-aligment">
                            <TextInput Text="Введите код приглашения" />

                            <button className="button-primary">Присоединиться</button>

                            <span className="mx-1 text-gray-600">или</span>

                            <button className="button-primary" onClick={() => RandomRoom(setPage)}>Случайная игра</button>;
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
