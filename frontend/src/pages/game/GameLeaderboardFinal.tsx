import React, { useEffect, useState } from "react";
import Header from "../../components/Header";
import PlayerCard from "../../components/Cards";
import type { CurrentAppState } from "../../App";
import { usePage } from "../../PageContext";
import fetchImageUrl from "../../components/ImageDownloader";
import defaultAvatarImg from "../../assets/defaultavatar.png";
import tronImg from "../../assets/tron.png";
export default function GameLeaderboardFinal(props: CurrentAppState) {
    const { setPage } = usePage();

    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }


    const playerList = props.room.players.sort((a, b) => (b.score ?? 0) - (a.score ?? 0)).map(
        player =>
            <PlayerCard variant="score" username={player.playerName} avatar={player.avatarUrl} score={player.score ?? 0} />);

    const [avatar, setAvatar] = useState<string>(defaultAvatarImg);
    const avatarUrl = props.room.players.sort((a, b) => (b.score ?? 0) - (a.score ?? 0)).at(0)?.avatarUrl;
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
        <div className="global-container">
            <Header variant="logo-and-timer" />
            <div className="main-container">

                <div className="row-template">
                    <div className="down-picture row-column">
                        <div className="centered-element">


                            <img
                                src={tronImg}
                                className="default-picture"
                            />
                            <img
                                src={avatar}
                                className="avatar-xl centered-avatar "
                            />
                            <div className="title-variant-2 centered-aligment">Победитель</div>

                        </div>
                    </div>
                    <div className="list-container row-column">
                        <div className="title-text-2">
                            Лидеры
                        </div>
                        {playerList}
                    </div>
                </div>
            </div>
        </div>
    );
}