
import { usePage } from "../PageContext";
import { http } from "../api/http";
import { useState } from "react";
import defaultImage from './../assets/defaultavatar.png';
import { apiService } from "../apiUtils/endPointsServices";
import { LoggingStatus, type CurrentAppState } from "../App";
import type { UserDto } from "../apiUtils/dto";


async function handleGuest(
    playerName: string,
    avatarUrl: any,
    setLoggingStatus: (status: any) => void,
    props: CurrentAppState) {

    const avatarFile = await urlToFile(avatarUrl, "avatar.jpg");
    const data = { playerName, avatar: avatarFile };
    const result = await apiService.createGuest(data);

    if (!result.success) {
        console.error(result.message);
        return;
    }
    setLoggingStatus(LoggingStatus.Guest);
    const user: UserDto = { id : result.resultObj?.id || 
        "handleGuest error", avatarUrl, playerName
    }
    props.setUser(user);
}

async function urlToFile(url: string, filename: string): Promise<File> {
    const res = await fetch(url);
    const blob = await res.blob();
    return new File([blob], filename, { type: blob.type });
}


export default function EnterModal(props: any) {
    const { setPage } = usePage();
    const [avatar, setAvatar] = useState(defaultImage);
    const [playerName, setName] = useState("");

    return (
        <div className="modal-global-container">

            <div className="modal-container">
                <div className="centered-vertical-aligment">
                    <div className="secondary-container">

                        <div className="left-aligment">

                            <img
                                src={defaultImage}
                                alt="avatar"
                                className="w-10 h-10 rounded-full border border-gray-300"

                            />

                            <input
                                type="text"
                                className="text-input-primary"
                                placeholder={"Введите имя"}
                                onChange={(e) => setName(e.target.value)}
                            />
                        </div>
                    </div>
                    <div className="centered-aligment">
                        <button className="button-primary" onClick={() => handleGuest(playerName, avatar, props.setLoggingStatus, props)}>Продолжить как гость</button>
                    </div>
                </div>
            </div>

            <div className="title-text">или</div>



            <div className="modal-container">
                <div className="centered-vertical-aligment">
                    <div className="secondary-container">
                        <div className="secondary-text">
                            Войдите, для того чтобы:
                            <ul>
                                <li>Создавать лобби</li>
                                <li>Сохранять статистику</li>
                            </ul>
                        </div>

                        <div className="centered-aligment">
                            <button className="button-primary" onClick={() => setPage("login")}>Войти</button>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    );
}
