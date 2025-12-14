import React from "react";
import '../App.css';
import { usePage } from "../PageContext";
import { CircularProgress } from "@mui/material";

export default function Header({ variant = "logo" }) {
    if (variant === "logo-and-login-button") {
        return (
            <HeaderWithLogoAndLoginButton />
        );
    }
    else if (variant === "logo") {
        return (
            <HeaderWithOnlyLogo />
        );
    } else if (variant === "logo-and-avatar-and-leave-room") {
        return (
            <HeaderWithLogoAndAvatarAndLeaveRoomButton />
        );
    } else if (variant === "logo-and-avatar-and-create-room") {
        return (
            <HeaderWithLogoAndAvatarAndCreateRoomButton />
        );
    } else if (variant === "logo-and-timer") {
        return (
            <HeaderWithLogoAndAvatarAndTimer />
        );
    }
}

function HeaderWithOnlyLogo() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>

        </div>
    );
}

function HeaderWithLogoAndLoginButton() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>

            <div className="flex items-center gap-3">
                <button className="button-primary" onClick={() => setPage("login")}>Login</button>
            </div>
        </div>
    );
}

function HeaderWithLogoAndAvatarAndCreateRoomButton() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>
            <div className="left-aligment">
                <div className="accent-title-text" onClick={() => setPage("room")}>Create lobby</div>
                <div className="flex items-center gap-3">
                    <img onClick={() => setPage("profile")}
                        src="src/assets/defaultavatar.jpg"
                        alt="avatar"
                        className="w-10 h-10 rounded-full border border-gray-300"
                    />
                </div>
            </div>
        </div>
    );
}

function HeaderWithLogoAndAvatarAndLeaveRoomButton() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>
            <div className="left-aligment">
                <div className="accent-title-text" onClick={() => setPage("home")}>Leave lobby</div>
                <div className="flex items-center gap-3">
                    <img onClick={() => setPage("profile")}
                        src="src/assets/defaultavatar.jpg"
                        alt="avatar"
                        className="w-10 h-10 rounded-full border border-gray-300"
                    />
                </div>
            </div>
        </div>
    );
}

function HeaderWithLogoAndAvatarAndRemoveRoomButton() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>
            <div className="left-aligment">
                <div className="accent-title-text" onClick={() => alert("remove lobby")}>Remove lobby</div>
                <div className="flex items-center gap-3">
                    <img onClick={() => setPage("profile")}
                        src="src/assets/defaultavatar.jpg"
                        alt="avatar"
                        className="w-10 h-10 rounded-full border border-gray-300"
                    />
                </div>
            </div>
        </div>
    );
}

function HeaderWithLogoAndAvatarAndTimer() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>
            <div className="flex items-center gap-3">
                <CircularProgress
                    variant="determinate"
                    size="2.2rem"
                    color="secondary"
                    value={67}
                    thickness={10}
                    sx={{

                    }}
                />
            </div>
        </div>
    );
}
