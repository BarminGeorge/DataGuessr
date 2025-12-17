import React, { useEffect, useState } from "react";
import Header from "../../components/Header";
import Slider from '@mui/material/Slider';
import { getPlayerId, type CurrentAppState } from "../../App";
import PacmanLoader from "react-spinners/PacmanLoader";
import { gameHubService } from "../../apiUtils/HubServices";
import { GameMode, type AnswerDto, type QuestionDto, type RoomDto, type PlayerDto } from "../../apiUtils/dto";
import type { NewQuestionNotification } from "../../apiUtils/notifications";
import fetchImageUrl from "../../components/ImageDownloader";

function valuetext(value: number) {
    return `${value}`;
}

function LoadingScreen() {
    return (
        <div>
            <Header variant="logo" />
            <div className="main-container">
                <div className="centered-empty-element">
                    <div className="centered">
                        <PacmanLoader />
                    </div>
                </div>
            </div>
        </div>
    );

}

async function sendAnswer(
    answer: any,
    gameId: string,
    playerId: string,
    questionId: string) {

    if (playerId == null)
        return;

    const result = await gameHubService.submitAnswer({
        gameId,
        playerId,
        questionId,
        answer
    });
    console.log(result);


    if (!result.success) {
        alert("Не удалось ответить");
        return;
    }

}



function QuestionDefaultAnswer(props: any) {
    const [year, setYear] = useState(1966);
    const [isDisabled, setIsDisabled] = useState(false);
    const playerId = getPlayerId(props);
    {/* slider */ }
    if (props.correctAnswer != null) {
        return (
            <div className="left-aligment">
                <Slider
                    aria-label="Small steps"
                    defaultValue={new Date(props.correctAnswer).getFullYear()}
                    getAriaValueText={valuetext}
                    step={1}
                    min={1850}
                    max={2025}
                    valueLabelDisplay="auto"
                    color="primary"
                    disabled
                    sx={{
                        '& .MuiSlider-thumb': {
                            borderRadius: '1px',
                        },
                    }}
                />

                <span className="accent-text">{year} год</span>
                <button className="button-primary" disabled>Ответить</button>
            </div >
        );
    }

    return (

        <div className="left-aligment">
            <div className="left-aligment">
            <Slider
                aria-label="Small steps"
                defaultValue={1966}
                getAriaValueText={valuetext}
                step={1}
                onChange={(_, value) => setYear(Number(value))}
                min={1850}
                max={2025}
                valueLabelDisplay="auto"
                color="secondary"
                sx={{
                    '& .MuiSlider-thumb': {
                        borderRadius: '1px',
                    },
                }}
            />

                <span className="accent-text">{year} год</span>
            </div>
            <button className="button-primary"
                onClick={() => {
                    const answer: AnswerDto = { "$type": "datetime", value: new Date(Date.UTC(year, 0, 1)) };
                    sendAnswer(answer, props.game.id, playerId, props.question.questionId);
                    setIsDisabled(true);

                }}
                disabled={isDisabled}>Ответить</button>
        </div >
    );
}

function QuestionBoolAnswer(props: any) {
    const [isDisabled, setIsDisabled] = useState(false);
    const playerId = getPlayerId(props);

    console.log(props.correctAnswer);
    if (props.correctAnswer != null) {
        return (
            <div className="centered-centered-h-aligment">
                <button className={`button-primary ${props.correctAnswer ? "correct" : ""}`} disabled>
                    Правда</button>

                <button className={`button-primary ${props.correctAnswer ? "" : "correct"}`} disabled>
                    Ложь</button>

            </div>
        );
    } else {

        return (
            <div className="centered-centered-h-aligment">
                <button className="button-primary"
                    onClick={() => {
                        const answer: AnswerDto = { "$type": "bool", value: true };
                        sendAnswer(answer, props.game.id, playerId, props.question.questionId);
                    }}
                    disabled={isDisabled}
                >
                    Правда</button>

                <button className="button-primary"
                    onClick={() => {
                        const answer: AnswerDto = { "$type": "bool", value: false };
                        sendAnswer(answer, props.game.id, playerId, props.question.questionId);
                        setIsDisabled(true);
                    }}
                    disabled={isDisabled}
                >
                    Ложь</button>
            </div>
        );
    }
}

function QuestionAnswer(props: any) {
    if (props.question.gameMode === GameMode.Default) {
        return (<QuestionDefaultAnswer {...props} />);
    } else if (props.question.gameMode === GameMode.BoolMode) {
        return (<QuestionBoolAnswer {...props} />);
    }
}

function QuestionScreen(props: any) {
    const [imageSrc, setimageSrc] = useState<string>("src/assets/defaultavatar.jpg");

    if (props.game == null) {
        return;
    }
    useEffect(() => {
        let cancelled = false;

        fetchImageUrl(props.question.imageUrl).then((url) => {
            if (!cancelled) {
                setimageSrc(url);
            }
        });

        return () => {
            cancelled = true;
        };
    }, [props.question]);

    console.log(props);
    return (
        <div>
            <Header variant="logo-and-timer" duration={props.game.questionDuration} />
            <div className="main-container">
                <div className="main-container">
                    <div className="secondary-container">
                        {/* question */}
                        <p className="title-accent-text">{props.question.formulation}</p>
                        <div className="picture-xl-container">
                            <img
                                src={imageSrc}
                                className="default-picture"
                            />

                        </div>
                    </div>
                    {/* slider */}

                    <QuestionAnswer {...props} />
                </div>
            </div>
        </div>
    );
}

export default function GameRoundPage(props: CurrentAppState) {
    const [question, setQuestion] = useState<NewQuestionNotification>();
    const [correctAnswer, setCorrectAnswer] = useState<any>(null);


    useEffect(() => {
        if (!props.game) return;

        const offQuestion = gameHubService.onNewQuestion(data => {
            console.log(data);
            setQuestion(data);
        });

        const offQuestionClosed = gameHubService.onQuestionClosed(data => {
            console.log(data);
            const timerId = setTimeout(() => {
                setQuestion(null);
            }, 5000);
            setCorrectAnswer(data.correctAnswer.value);
        });

        const offLeaderboard = gameHubService.onShowLeaderBoard(data => {
            console.log(data);
            const scores = data.statistic.scores;
            props.setRoom(prev => ({
                ...prev,
                players: prev.players.map(pa => ({
                    ...pa,
                    score: (pa.score ?? 0) + (scores[pa.playerId]?.points ?? 0)
                }))
            }));
            
            props.setPage("game_leaderboard");
        });

        return () => {
            offQuestion();
            offQuestionClosed();
            offLeaderboard();
        };
    }, [props.game]);


    if (question)
        return (
            <div className="global-container">

                <QuestionScreen {...question} correctAnswer={correctAnswer} {...props} />
            </div>
        );
    return (
        <div className="global-container">

            <LoadingScreen />
        </div>
    );
}