import React, { useEffect, useState } from "react";

import '../App.css';
import { usePage } from "../PageContext";
import { CircularProgress } from "@mui/material";
import fetchImageUrl from "./ImageDownloader";

export default function Header(props: any) {
    if (props.variant === "logo-and-login-button") {
        return (
            <HeaderWithLogoAndLoginButton />
        );
    } else if (props.variant === "logo-and-avatar-and-interactive") {
        return (
            <HeaderWithLogoAndAvatarAndInteractive props={props} />
        );
    } else if (props.variant === "logo-and-timer") {
        return (
            <HeaderWithLogoAndAvatarAndTimer />
        );
    } else {
        return (
            <HeaderWithOnlyLogo />
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

function HeaderWithLogoAndAvatarAndInteractive(props: any) {

    props = props.props;

    const { setPage } = usePage();
    const playerName = localStorage.getItem("player_name");
    const avatarUrl =
        localStorage.getItem("avatar_url") ?? "";

    const [avatar, setAvatar] = useState<string>("src/assets/defaultavatar.jpg");

    useEffect(() => {
        let cancelled = false;

        fetchImageUrl(avatarUrl).then((url) => {
            if (!cancelled) {
                setAvatar(url);
            }
        });

        return () => {
            cancelled = true;
        };
    }, [avatarUrl]);

    return (
        <div className="header-container">
            <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>
            <div className="left-aligment">

                <div className="accent-title-text"
                    onClick={props.interact_action}>{props.interact_label}</div>

                <div className="secondary-text">{playerName}</div>
                <div className="flex items-center gap35">

                    <img onClick={() => setPage("profile")}
                        src={avatar}
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
