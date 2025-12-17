import { usePage } from "../PageContext";
import { useState } from "react";
import { apiService } from "../apiUtils/endPointsServices";
import { LoggingStatus, type CurrentAppState } from "../App";
import type { UserDto } from "../apiUtils/dto";

import defaultImage from './../assets/defaultavatar.png';
import av1 from './../assets/bulatov_avatar.png';
import av2 from './../assets/dimas_avatar.png';
import av3 from './../assets/gb_avatar.png';
import av4 from './../assets/ivolzok_avatar.png';
import av5 from './../assets/mefodii_avatar.png';
import av6 from './../assets/ravil_avatar.png';
import av7 from './../assets/yurlea_avarar.png';

const avatarList = [
    defaultImage,
    av1,
    av2,
    av3,
    av4,
    av5,
    av6,
    av7
];

async function handleGuest(
    playerName: string,
    avatarUrl: any,
    setLoggingStatus: (status: any) => void,
    props: CurrentAppState) {

    const avatarFile = await urlToFile(avatarUrl, "avatar.png");
    const data = { playerName, avatar: avatarFile };
    const result = await apiService.createGuest(data);

    if (!result.success) {
        console.error(result.message);
        return;
    }
    setLoggingStatus(LoggingStatus.Guest);
    const user: UserDto = {
        id: result.resultObj?.id || "handleGuest error",
        avatarUrl,
        playerName
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
    const [avatar, setAvatar] = useState(avatarList[0]);
    const [playerName, setName] = useState("");

    const handleChangeAvatar = () => {
        const currentIndex = avatarList.indexOf(avatar);
        const nextIndex = (currentIndex + 1) % avatarList.length;
        setAvatar(avatarList[nextIndex]);
    };

    return (
        <div className="modal-global-container">

            <div className="modal-container">
                <div className="centered-vertical-aligment">
                    <div className="secondary-container">

                        <div className="left-aligment" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '10px' }}>

                            <img
                                src={avatar}
                                alt="avatar"
                                className="w-10 h-10 rounded-full border border-gray-300"
                                style={{ width: '100px', height: '100px', objectFit: 'cover' }}
                            />

                            <button
                                className="button-secondary"
                                onClick={handleChangeAvatar}
                                style={{ width: '40px', height: '40px', fontSize: '20px', padding: 0, display: 'flex', justifyContent: 'center', alignItems: 'center' }}
                            >
                                ↻
                            </button>

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