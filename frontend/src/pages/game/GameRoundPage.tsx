import React, { useState } from "react";
import Header from "../../components/Header";
import Slider from '@mui/material/Slider';

function valuetext(value: number) {
  return `${value}`;
}


export default function GameRoundPage() {
  const [year, setYear] = useState(1966)
  return (
    <div className="global-container">
    <Header />
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
    />

    <span className="accent-text">{year} год</span>
    </div>
    </div>
    </div>
  );
}