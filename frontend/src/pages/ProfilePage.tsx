import React from "react";
import Header from "../components/Header";
import { useState, useEffect } from "react";
import { usePage } from "../PageContext";
import { validateUsername } from "../utils/validations";
import { apiService } from "../apiUtils/endPointsServices";
import type { CurrentAppState, LoggingStatus } from "../App";
import type { UserDto } from "../apiUtils/dto";

async function handleSaveProfile(
    newPlayerName: string,
    newAvatar: File | null,
    props: CurrentAppState,
    setErrorMessage: (msg: string) => void
) {
    const nameValidationError = validateUsername(newPlayerName);
    if (nameValidationError) {
        setErrorMessage(nameValidationError);
        return;
    }
    
    try {
        const data = { newPlayerName, newAvatar }; // TODO добавить id
        const result = await apiService.updateUser(data);

        if (!result.success) {
            setErrorMessage(result.message || "Ошибка при сохранении");
            return;
        }

        if (result.resultObj) {
            localStorage.setItem("player_name", result.resultObj.playerName);
            localStorage.setItem("avatar_url", result.resultObj.avatarUrl || "");

            const updatedUser: UserDto = {
                id: props.user?.id || result.resultObj.id,
                avatarUrl: result.resultObj.avatarUrl,
                playerName: result.resultObj.playerName
            };
            props.setUser(updatedUser);
        }

        props.setPage("home");
    } catch (error) {
        console.error("Ошибка при обновлении профиля:", error);
        setErrorMessage("Произошла ошибка при сохранении");
    }
}

export default function ProfilePage(props: CurrentAppState) {
    const { setPage } = usePage();

    // Состояния для редактируемых полей
    const [playerName, setPlayerName] = useState(props.user?.playerName || "");
    const [avatar, setAvatar] = useState<File | null>(null);
    const [avatarPreview, setAvatarPreview] = useState<string>(props.user?.avatarUrl || "");
    const [errorMessage, setErrorMessage] = useState<string>("");

    // Сбрасываем ошибку при изменении полей
    useEffect(() => {
        setErrorMessage("");
    }, [playerName, avatar]);

    return (
        <div className="global-container">
            <Header />
            <div className="main-container">
                <div className="title-text">
                    Параметры профиля
                </div>

                {/* Блок с текущей информацией */}
                <div className="secondary-container info-section">
                    <div className="current-avatar">
                        <img
                            src={avatarPreview || "/default-avatar.png"}
                            alt="Текущий аватар"
                            className="avatar-image"
                        />
                    </div>
                    <div className="current-info">
                        <p>Текущее имя: <strong>{props.user?.playerName}</strong></p>
                    </div>
                </div>

                {/* Поле для изменения имени */}
                <div className="secondary-container">
                    <input
                        type="text"
                        className="text-input-primary"
                        placeholder="Введите новое имя игрока"
                        value={playerName}
                        onChange={(e) => setPlayerName(e.target.value)}
                    />
                    <span className="accent-text">
                        {validateUsername(playerName)}
                    </span>
                </div>

                {/* Загрузка нового аватара */}
                <div className="secondary-container">
                    <label className="avatar-input-primary">
                        <input
                            type="file"
                            accept="image/*"
                            className="avatar-input-hidden"
                            onChange={(e) => {
                                const file = e.target.files?.[0] ?? null;
                                setAvatar(file);

                                // Создаем превью для нового аватара
                                if (file) {
                                    const previewUrl = URL.createObjectURL(file);
                                    setAvatarPreview(previewUrl);
                                } else {
                                    // Возвращаем старый аватар, если файл не выбран
                                    setAvatarPreview(props.user?.avatarUrl || "");
                                }
                            }}
                        />

                        <div className="avatar-preview">
                            {avatarPreview ? (
                                <img
                                    src={avatarPreview}
                                    alt="предпросмотр аватара"
                                />
                            ) : (
                                <span>?</span>
                            )}
                        </div>

                        <div className="avatar-text">
                            {avatar ? avatar.name : "Выберите новый аватар"}
                        </div>
                    </label>
                </div>

                {/* Сообщение об ошибке */}
                {errorMessage && (
                    <div className="error-message">
                        <span className="accent-text">{errorMessage}</span>
                    </div>
                )}

                {/* Кнопки действий */}
                <div className="button-group">
                    <button
                        className="button-secondary"
                        onClick={() => setPage("home")}
                    >
                        Отмена
                    </button>
                    <button
                        className="button-primary"
                        onClick={() => handleSaveProfile(playerName, avatar, props, setErrorMessage)}
                        disabled={!!validateUsername(playerName)}
                    >
                        Сохранить изменения
                    </button>
                </div>
            </div>
        </div>
    );
}