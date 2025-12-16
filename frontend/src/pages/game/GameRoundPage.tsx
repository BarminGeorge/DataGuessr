import React, { useEffect, useState } from "react";
import Header from "../../components/Header";
import Slider from '@mui/material/Slider';
import { getPlayerId, type CurrentAppState } from "../../App";
import PacmanLoader from "react-spinners/PacmanLoader";
import { gameHubService } from "../../apiUtils/HubServices";
import type { AnswerDto, QuestionDto } from "../../apiUtils/dto";
import type { NewQuestionNotification } from "../../apiUtils/notifications";
import fetchImageUrl from "../../components/ImageDownloader";

function valuetext(value: number) {
    return `${value}`;
}

function LoadingScreen() {
    return (
        <div className="main-container">
            <div className="centered-empty-element">
                <div className="centered">
                    <PacmanLoader />
                </div>
            </div>
        </div>
    );

} 

async function sendAnswer(
    answer: AnswerDto,
    gameId: string,
    playerId: string,
    questionId: string) {

    if (playerId == null)
        return;

    const result = await gameHubService.submitAnswer({
        answer,
        gameId,
        playerId,
        questionId
    });
    console.log(result);

    if (!result.success || !result.resultObj) {
        alert("Не удалось ответить");
        return;
    }
}

function QuestionScreen(question: NewQuestionNotification, props: CurrentAppState) {
    const [year, setYear] = useState(1966);
    const [imageSrc, setimageSrc] = useState<string>("src/assets/defaultavatar.jpg");

    if (props.game == null) {
        return;
    }
    const playerId = getPlayerId(props);

    useEffect(() => {
        let cancelled = false;

        fetchImageUrl(question.imageUrl).then((url) => {
            if (!cancelled) {
                setimageSrc(url);
            }
        });

        return () => {
            cancelled = true;
        };
    }, [question.imageUrl]);


    return (
        <div className="main-container">
            <div className="main-container">
                <div className="secondary-container">
                    {/* question */}
                    <p className="title-accent-text">{question.formulation}</p>
                    <div className="picture-xl-container">
                        <img
                            src={imageSrc }
                            className="default-picture"
                        />

                    </div>
                </div>
                {/* slider */}
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
                    <button className="primary-button"
                        onClick={() => {
                            const answer: AnswerDto = { value: year };
                            sendAnswer(answer, props.game.id, playerId, question.questionId);
                        }}>
                    Ответить</button>
                </div>
            </div>
        </div>
    );
}

export default function GameRoundPage(props: CurrentAppState) {
    const [question, setQuestion] = useState<NewQuestionNotification>();


    useEffect(() => {
        if (!props.game) return;
        
        const offQuestion = gameHubService.onNewQuestion(data => {
            console.log(data);
            setQuestion(data);
        });

        return () => {
            offQuestion();
        };
    }, [props.game]);


    if (question)
    return (
        <div className="global-container">
            <Header variant="logo-and-timer" />
            <QuestionScreen {...question} {...props} />
        </div>
        );
    return (
        <div className="global-container">
            <Header variant="logo" />
            <LoadingScreen />
        </div>
    );
}