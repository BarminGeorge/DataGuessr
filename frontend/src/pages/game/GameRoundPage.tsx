import React, { useState } from "react";
import Header from "../../components/Header";
import Slider from '@mui/material/Slider';
import type { CurrentAppState } from "../../App";
import PacmanLoader from "react-spinners/PacmanLoader";

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

function QuestionScreen(props: any) {
    const [year, setYear] = useState(1966);

    return (
        <div className="main-container">
            <div className="main-container">
                <div className="secondary-container">
                    {/* question */}
                    <p className="title-accent-text">В каком году сделана данная фотография?</p>
                    <div className="picture-xl-container">
                        <img
                            src="src/assets/defaultavatar.jpg"
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
                </div>
            </div>
        </div>
    );
}

export default function GameRoundPage(props: CurrentAppState) {
    const [question, setQuestion] = useState(null);

    if (question)
    return (
        <div className="global-container">
            <Header variant="logo-and-timer" />
            <QuestionScreen />
        </div>
        );
    return (
        <div className="global-container">
            <Header variant="logo-and-timer" />
            <LoadingScreen />
        </div>
    );
}