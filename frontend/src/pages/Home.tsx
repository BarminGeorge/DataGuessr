import React, { useEffect, useState } from "react";
import Header from "../components/Header";
import http from "../api/http";
import { usePage } from "../PageContext";
import EnterModal from "../modals/EnterModal";
import { LoggingStatus, type CurrentAppState } from "../App";
import { gameHubService } from "../apiUtils/HubServices";
import { RoomPrivacy, type RoomDto, type PlayerDto } from "../apiUtils/dto";
import { createRoom, joinRoomByCode, findRandomRoom } from "../utils/RoomHubUtils";

function checkLogging(loggingStatus: any) {
    if (loggingStatus == LoggingStatus.Guest) {
        return true;
    }
    if (loggingStatus == LoggingStatus.Logged) {
        return true;
    }
    return false;
}


export default function HomePage(props: CurrentAppState) {
    const { setPage } = usePage();
    const [roomCode, setRoomCode] = useState("");


    return (
        <div className="global-container">

            {/* <button className="button-primary" onClick={() => setPage("game")}>В Игре</button>;   */}
            <div className={`modal centered-vertical-aligment ${checkLogging(props.loggingStatus) ? 'hide' : ''}`}>
                <EnterModal setLoggingStatus={props.setLoggingStatus} />
            </div>
            <div className={checkLogging(props.loggingStatus) ? "" : "blur"}>


                <Header variant={props.loggingStatus == LoggingStatus.Logged ?
                    "logo-and-avatar-and-interactive"
                    : "logo-and-login-button"}
                    interact_action={() => createRoom(props.user_id, setPage, props.room, props.setRoom)}
                    interact_label={"Create lobby"}
                />

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
                            <input
                                type="text"
                                className="text-input-primary"
                                placeholder="Введите код приглашения"
                                value={roomCode}
                                onChange={e => setRoomCode(e.target.value)}
                            />


                            <button
                                className="button-primary"
                                onClick={() => joinRoomByCode(props.user_id, roomCode, setPage, props.room, props.setRoom)}
                            >
                                Присоединиться
                            </button>


                            <span className="mx-1 text-gray-600">или</span>

                            <button
                                className="button-primary"
                                onClick={() => findRandomRoom(props.user_id, setPage, props.room, props.setRoom)}
                            >
                                Случайная игра
                            </button>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
