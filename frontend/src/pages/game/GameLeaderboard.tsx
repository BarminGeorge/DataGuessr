import React, { useEffect } from "react";
import Header from "../../components/Header";
import PlayerCard from "../../components/Cards";
import type { CurrentAppState } from "../../App";
import { usePage } from "../../PageContext";
import { gameHubService } from "../../apiUtils/HubServices";

export default function GameLeaderboard(props: CurrentAppState) {
    const { setPage } = usePage();

    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }


    useEffect(() => {
        if (!props.game) return;

        const offQuestion = gameHubService.onNewQuestion(data => {
            props.setQuestion(data);
            setPage("game_round");
        })

        const offLeaderboard = gameHubService.onShowLeaderBoard(data => {
            console.log(data);
            const scores = data.statistic.scores;
            props.setRoom(prev => ({
                ...prev,
                players: prev.players.map(pa => ({
                    ...pa,
                    score: scores[pa.playerId]?.points ?? 0
                }))
            }));
        });

        return () => {
            offQuestion();
            offLeaderboard();
        };
    });


    const playerList = props.room.players.sort((a, b) => (b.score ?? 0) - (a.score ?? 0)).map(
        player =>
            <PlayerCard variant="score" username={player.playerName} avatar={player.avatarUrl} score={player.score ?? 0} />);


    return (
        <div className="global-container">
            <Header variant="logo-and-timer" duration={10} />
            <div className="main-container">

                <div className="row-template">
                    <div className="down-picture row-column">
                        <div className="down-element">
                            <img
                                src="src/assets/defaultavatar.jpg"
                                className="default-picture"
                            />
                        </div>
                    </div>

                    <div className="list-container row-column">
                        <div className="title-text-2">
                            Лидеры
                        </div>


                        {playerList}


                    </div>
                    <div className="down-picture row-column">
                        <div className="up-element">
                            <img
                                src="src/assets/defaultavatar.jpg"
                                className="default-picture"
                            />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}