import React, { useEffect, useState } from "react";

import '../App.css';
import { usePage } from "../PageContext";
import { CircularProgress } from "@mui/material";
import fetchImageUrl from "./ImageDownloader";
import type { CurrentAppState } from "../App";



export default function Header(props: any) {
    if (props.variant === "logo-and-login-button") {
        return (
            <HeaderWithLogoAndLoginButton />
        );
    } else if (props.variant === "logo-and-avatar-and-interactive") {
        return (
            <HeaderWithLogoAndAvatarAndInteractive {...props} />
        );
    } else if (props.variant === "logo-and-timer") {
        return (
            <HeaderWithLogoAndAvatarAndTimer time={props.duration} />
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
            <div className="title-text">FIITguesser</div>

        </div>
    );
}

function HeaderWithLogoAndLoginButton() {
    const { setPage } = usePage();
    return (
        <div className="header-container">
            <div className="title-text">FIITguesser</div>

            <div className="flex items-center gap-3">
                <button className="button-primary" onClick={() => setPage("login")}>Login</button>
            </div>
        </div>
    );
}

function HeaderWithLogoAndAvatarAndInteractive(props: any) {

    //props = props.props;

    const { setPage } = usePage();
    const playerName = props.playerName;
    const avatarUrl = props.avatarUrl ?? ""

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
            <div className="title-text">FIITguesser</div>
            <div className="left-aligment">

                <div className="accent-title-text"
                    onClick={props.interact_action}>{props.interact_label}</div>

                <div className="secondary-text">{playerName}</div>
                <div className="flex items-center gap35">

                    <img onClick={() => setPage("profile")}
                        src={avatar}
                        alt="avatar"
                        className="avatar-preview"
                    />
                </div>
            </div>
        </div>
    );
}

type Props = {
    time: number
};

function HeaderWithLogoAndAvatarAndTimer({ time }: Props) {
    const [seconds, setSeconds] = useState(time); // Состояние для хранения секунд

    useEffect(() => {
        if (seconds <= 0) return;

        const interval = window.setInterval(() => {
            setSeconds(prev => Math.max(prev - 0.2, 0));
        }, 200);

        return () => clearInterval(interval);
    }, [seconds]);

    const progress = Math.min(
        100,
        Math.max(0, (seconds / time) * 100)
    );



    return (
        <div className="header-container">
            <div className="title-text">FIITguesser</div>
            <div className="flex items-center gap-3">
                <CircularProgress
                    variant="determinate"
                    size="2.2rem"
                    color="secondary"
                    value={progress}
                    thickness={10}
                    sx={{

                    }}
                />
            </div>
        </div>
    );
}
