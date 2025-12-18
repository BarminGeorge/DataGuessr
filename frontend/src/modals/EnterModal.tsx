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

                    {/* Убрал лишние отступы, чтобы всё влезло */}
                    <div className="secondary-container" style={{ padding: '15px' }}>

                        <div className="left-aligment" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '15px', width: '100%' }}>

                            {/* Аватарка */}
                            <div style={{ position: 'relative', width: '80px', height: '80px' }}>
                                <img
                                    src={avatar}
                                    alt="avatar"
                                    className="w-10 h-10 rounded-full border border-gray-300"
                                    style={{ width: '100%', height: '100%', objectFit: 'cover', borderRadius: '50%' }}
                                />

                                <button
                                    className="button-secondary"
                                    onClick={handleChangeAvatar}
                                    style={{
                                        position: 'absolute',
                                        bottom: '-5px',
                                        right: '-5px',
                                        width: '28px',
                                        height: '28px',
                                        borderRadius: '50%',
                                        padding: 0,
                                        display: 'flex',
                                        justifyContent: 'center',
                                        alignItems: 'center',
                                        fontSize: '14px',
                                        border: '1px solid #ccc',
                                        backgroundColor: 'white',
                                        color: 'black',
                                        cursor: 'pointer',
                                        zIndex: 10
                                    }}
                                >↺
                                </button>
                            </div>

                            {/* СТРОКА ВВОДА И КНОПКА В ОДНУ ЛИНИЮ */}
                            <div style={{
                                display: 'flex',
                                flexDirection: 'row',
                                gap: '8px',
                                width: '100%',
                                alignItems: 'stretch' // Чтобы кнопка и инпут были одной высоты
                            }}>
                                <input
                                    type="text"
                                    className="text-input-primary"
                                    placeholder="Имя"
                                    value={playerName}
                                    onChange={(e) => setName(e.target.value)}
                                    // flex: 1 заставляет инпут занимать всё место, кроме кнопки. minWidth: 0 важен для flexbox.
                                    style={{ flex: 1, minWidth: 0, margin: 0, width: 'auto' }}
                                />

                                <button
                                    className="button-primary"
                                    onClick={() => handleGuest(playerName, avatar, props.setLoggingStatus, props)}
                                    // Убираем margin, делаем кнопку по ширине текста
                                    style={{ width: 'auto', whiteSpace: 'nowrap', padding: '0 15px', margin: 0 }}
                                >
                                    Играть
                                </button>
                            </div>

                        </div>
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

                        <div className="centered-aligment" style={{ marginTop: '20px' }}>
                            <button className="button-primary" onClick={() => setPage("login")}>Войти</button>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    );
}