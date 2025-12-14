import React from "react";

export default function PlayerCard(props: any) {
    switch (props.variant) {
        case "score":
            return (
            <ScoreCard username={props.username} score={props.score} />
            );
        case "lobby-common":
            return (
            <LobbyCard username={props.username}/>
            );
        default:
            return null;
    }
}

function LobbyCard(props: any) {
  return (
     <div className="score-card">
        <img
          src="src/assets/defaultavatar.jpg"
          alt="avatar"
          className="w-10 h-10 rounded-full border border-gray-300"
        />
          {props.username}  
           <img
          src="src/assets/defaultavatar.jpg"
          alt="avatar"
          className="w-10 h-10 rounded-full border border-gray-300"
        />
                
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