import React, { useEffect, useState } from "react";
import Header from "../components/Header";
import { usePage } from "../PageContext";
import { validateUsername } from "../utils/validations";
import { apiService } from "../apiUtils/endPointsServices";
import type { CurrentAppState } from "../App";
import type { UpdateUserRequest, UserDto } from "../apiUtils/dto";
import fetchImageUrl from "../components/ImageDownloader";

async function handleUpdateProfile(
    playerName: string,
    avatar: File | null,
    props: CurrentAppState
) {
    // Предполагается, что в apiService есть метод updateProfile
    // Мы передаем ID текущего пользователя и новые данные


    const data: UpdateUserRequest = {
        userId: props.user?.id || "",
        playerName,
        avatar
    };
    
    const result = await apiService.updateUser(data);

    if (!result.success) {
        console.error(result.message);
        alert("Ошибка при обновлении профиля");
        return;
    }

    // Обновляем состояние пользователя в глобальном контексте
    const updatedUser: UserDto = {
        id: props.user?.id || "",
        avatarUrl: result.resultObj?.avatarUrl || "",
        playerName: playerName
    };

    props.setUser(updatedUser);
    props.setPage("home"); // Возвращаемся на главную после сохранения
}

export default function ProfilePage(props: CurrentAppState) {
    const { setPage } = usePage();

    // Инициализируем состояние текущими данными пользователя
    const [playerName, setPlayerName] = useState(props.user?.playerName);
    const [avatar, setAvatar] = useState<File | null>(null);
    const [previewUrl, setPreviewUrl] = useState<string | null>(props.user?.avatarUrl || null);


    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0] ?? null;
        setAvatar(file);
        if (file) {
            setPreviewUrl(URL.createObjectURL(file));
        }
    };

    return (
        <div className="global-container">
            <Header />
            <div className="main-container">
                <div className="title-text">
                    Редактирование профиля
                </div>

                {/* Поле изменения имени */}
                <div className="secondary-container">
                    <label className="secondary-text">Ваше имя</label>
                    <input
                        type="text"
                        className="text-input-primary"
                        value={playerName}
                        placeholder={"Введите новое имя"}
                        onChange={(e) => setPlayerName(e.target.value)}
                    />
                    <span className="accent-text">
                        {validateUsername(playerName)}
                    </span>
                </div>

                {/* Поле изменения аватарки */}
                <div className="secondary-container">
                    <label className="avatar-input-primary">
                        <input
                            type="file"
                            accept="image/*"
                            className="avatar-input-hidden"
                            onChange={handleFileChange}
                        />

                        <div className="avatar-preview">
                            {previewUrl ? (
                                <img
                                    src={previewUrl}
                                    alt="avatar preview"
                                />
                            ) : (
                                <span>?</span>
                            )}
                        </div>

                        <div className="avatar-text">
                            {avatar ? avatar.name : "Изменить фото"}
                        </div>
                    </label>
                </div>

                <div className="right-aligment">
                    <button
                        className="button-primary"
                        onClick={() => handleUpdateProfile(playerName, avatar, props)}
                    >
                        Сохранить изменения
                    </button>

                    <button
                        className="button-primary"
                        onClick={() => props.setPage("home")}
           
                    >
                        Отмена
                    </button>
                </div>
            </div>
        </div>
    );
}