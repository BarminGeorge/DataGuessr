import React from "react";

export default function PlayerCard(props: any) {
    switch (props.variant) {
        case "score":
            return (
                <ScoreCard username={props.username} score={props.score} />
            );
        case "lobby-common":
            return (
                <LobbyViewerCard username={props.username} />
            );
        case "lobby-creator":
            return (
                <LobbyCreatorCard username={props.username} action={props.action} />
            );
        default:
            return null;
    }
}

function LobbyCreatorCard(props: any) {
    return (
        <div className="score-card">
            <img
                src="src/assets/defaultavatar.jpg"
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
    return (
        <div className="score-card">
            <img
                src="src/assets/defaultavatar.jpg"
                alt="avatar"
                className="w-10 h-10 rounded-full border border-gray-300"
            />
            {props.username}

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