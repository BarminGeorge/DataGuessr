import React, { useEffect, useState } from "react";
import Header from "../../components/Header";
import PlayerCard from "../../components/Cards";
import type { CurrentAppState } from "../../App";
import { usePage } from "../../PageContext";
import fetchImageUrl from "../../components/ImageDownloader";
import defaultAvatarImg from "../../assets/defaultavatar.png";
import tronImg from "../../assets/tron.png";
import { gameHubService } from "../../apiUtils/HubServices";
import { leaveRoom } from "../../utils/RoomHubUtils";



export async function finishGame(
    userId: string | null,
    roomId: string | undefined,
) {

    if (userId == null || !roomId) {
        alert("Не удалось начать игру");
        return;
    }

    const result = await gameHubService.finishGame({
        userId: userId,
        roomId,
    });

    console.log(result);

    //if (!result.success) {
    //    alert("Не удалось перейти в лобби");
    //    return false;
    //}
    //return true;
}



function ExitButton(props: CurrentAppState) {
    if (props.user?.id === props.room.ownerId || true) {
        return (<button className="button-primary" onClick={() => leaveRoom(props.user?.id, props.room?.id, props.setPage, props.setRoom)}>Завершить</button>);
    } else {
        return (<label className="secondary-text">Ждём завершения...</label>);
    }

}
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


    useEffect(() => {
        if (!props.game) return;

        const offQuestion = gameHubService.onReturnToRoom(data => {
            console.log(data);
            props.setQuestion(null);
            props.setGame(null);
            props.setRoom(null);
            props.setPage("home")
        });
        return () => {
            offQuestion();
        };
    }, [props.game]);

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
                        <ExitButton {...props} />
                         </div>

                </div>
            </div>
        </div>
    );
}