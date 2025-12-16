import React, { useEffect, useState } from "react";
import fetchImageUrl from "./ImageDownloader";

export default function PlayerCard(props: any) {
    switch (props.variant) {
        case "score":
            return (
                <ScoreCard username={props.username} score={props.score} />
            );
        case "lobby-common":
            return (
                <LobbyViewerCard username={props.username} avatar={props.avatar} />
            );
        case "lobby-creator":
            return (
                <LobbyCreatorCard username={props.username} avatar={props.avatar} action={props.action} />
            );
        default:
            return null;
    }
}

function LobbyCreatorCard(props: any) {
    const [avatar, setAvatar] = useState<string>("src/assets/defaultavatar.jpg");

    useEffect(() => {
        let cancelled = false;

        fetchImageUrl(props.avatar).then((url) => {
            if (!cancelled) {
                setAvatar(url);
            }
        });

        return () => {
            cancelled = true;
        };
    }, [props.avatar]);

    return (
        <div className="score-card">
            <img
                src={avatar}
                alt="avatar"
                className="w-10 h-10 rounded-full border border-gray-300"
            />
            {props.username}
            <img
                src="src/assets/krest.png"
                alt="avatar"
                className="w-8 h-8 rounded-full border border-gray-300"
                onClick={props.action}
            />

        </div>
    );
}

function LobbyViewerCard(props: any) {
    const [avatar, setAvatar] = useState<string>("src/assets/defaultavatar.jpg");

    useEffect(() => {
        let cancelled = false;

        fetchImageUrl(props.avatar).then((url) => {
            if (!cancelled) {
                setAvatar(url);
            }
        });

        return () => {
            cancelled = true;
        };
    }, [props.avatar]);

    return (
        <div className="score-card">
            <img
                src={avatar}
                alt="avatar"
                className="w-10 h-10 rounded-full border border-gray-300"
            />
            {props.username}
            <div className="w-10 h-10 rounded-full"></div>
        </div>
    );
}

function ScoreCard(props: any) {
    return (
        <div className="score-card">
            <img
                src="src/assets/defaultavatar.jpg"
                alt="avatar"
                className="w-10 h-10 rounded-full border border-gray-300"
            />
            <div className="centered">{props.username}</div>

            <div className="score">{props.score}</div>

        </div>
    );
}