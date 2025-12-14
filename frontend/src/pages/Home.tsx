import React, { useEffect, useState } from "react";
import Header from "../components/Header";
import http from "../api/http";
import { usePage } from "../PageContext";
import EnterModal from "../modals/EnterModal";
import { LoggingStatus } from "../App";
import { gameHubService } from "../apiUtils/HubServices";
import { RoomPrivacy } from "../apiUtils/dto";


async function findRandomRoom(
    userId: string,
    setPage: (page: any ) => void
) {
    const result = await gameHubService.findQuickRoom({
        userId
    });

    if (!result.success || !result.resultObj) {
        console.log(result.message);
        alert("Не удалось найти комнату");
        return;
    }

    setPage("room");
}

async function joinRoomByCode(
    userId: string,
    roomId: string,
    setPage: (page: any) => void
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
        console.log(result.message);
        alert("Не удалось найти комнату");
        return;
    }

    setPage("room");
}

async function createRoom(
    userId: string,
    setPage: (page: any) => void) {

    const result = await gameHubService.createRoom({
        userId: userId,
        privacy: RoomPrivacy.Public,
        maxPlayers: 8
    });
    console.log(result.message);

    if (!result.success || !result.resultObj) {    
        alert("Не удалось создать комнату");
        return;
    }

    setPage("room");
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
    const [roomCode, setRoomCode] = useState("");

    
    props = props.props;

    return (
        <div className="global-container">

            {/* <button className="button-primary" onClick={() => setPage("game")}>В Игре</button>;   */}
            <div className={`modal centered-vertical-aligment ${checkLogging(props.loggingStatus) ? 'hide' : ''}`}>
                <EnterModal setLoggingStatus={props.setLoggingStatus} />
            </div>
            <div className={checkLogging(props.loggingStatus) ? "" : "blur"}>


                <Header variant={checkLogging(props.loggingStatus) ?
                    "logo-and-avatar-and-interactive"
                    : "logo-and-login-button"}
                    interact_action={() => createRoom(props.user_id, setPage)}
                    interact_label= {"Create lobby"}

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
                                onClick={() => joinRoomByCode(props.user_id, roomCode, setPage)}
                            >
                                Присоединиться
                            </button>


                            <span className="mx-1 text-gray-600">или</span>

                            <button
                                className="button-primary"
                                onClick={() => findRandomRoom(props.user_id, setPage)}
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
